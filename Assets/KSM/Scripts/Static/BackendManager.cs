using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using BackendData.Base;
using UnityEngine;

/*
 * BackendManager
 *
 * 1. 로그인 씬에서 초기화 진행
 * 2. 로그인 후 캐싱
 */
public class BackendManager : MonoBehaviour
{
    //뒤끝 콘솔에 업로드한 차트 데이터만 모아놓은 클래스
    public class BackendChart
    {
        public readonly BackendData.Chart.AllChart ChartInfo = new();
        public readonly BackendData.Chart.Harvest.HarvestChart Harvest = new();
        public readonly BackendData.Chart.Percent.PercentChart Percent = new();
        public readonly BackendData.Chart.Balance.BalanceChart Balance = new();
        public readonly BackendData.Chart.Animal.AnimalChart Animal = new();
        public readonly BackendData.Chart.Quest.QuestChart Quest = new();
    }
    
    //게임 정보 관리 데이터만 모아놓은 클래스
    public class BackendGameData
    {
        public readonly BackendData.GameData.UserData UserData = new();
        public readonly BackendData.GameData.FieldData FieldData = new();
        public readonly BackendData.GameData.InventoryData InventoryData = new();
        public readonly BackendData.GameData.ProfileData ProfileData = new();
        public readonly BackendData.GameData.ShopData ShopData = new();
        public readonly BackendData.GameData.MartData MartData = new();
        public readonly BackendData.GameData.AnimalData AnimalData = new();
        public readonly BackendData.GameData.PetData PetData = new();
        public readonly BackendData.GameData.PartTimeData PartTimeData = new();
        public readonly BackendData.GameData.QuestData QuestData = new();
        public readonly BackendData.GameData.WeatherData WeatherData = new();
        
        public readonly Dictionary<string, GameData> GameDataList = new Dictionary<string, GameData>();
        public BackendGameData()
        {
            GameDataList.Add("1", UserData);
            GameDataList.Add("2", FieldData);
            GameDataList.Add("3", InventoryData);
            GameDataList.Add("4", ProfileData);
            GameDataList.Add("5", ShopData);
            GameDataList.Add("6", MartData);
            GameDataList.Add("7", AnimalData);
            GameDataList.Add("8", PetData);
            GameDataList.Add("9", PartTimeData);
            GameDataList.Add("10", QuestData);
            GameDataList.Add("11", WeatherData);
        }
    }

    public BackendChart backendChart = new();
    public BackendGameData backendGameData = new();
    public void Initialize()
    {
        var initialBro = Backend.Initialize(true);

        if (initialBro.IsClientRequestFailError())
        {
            StaticManager.UI.AlertUI.OpenUI("네트워크에 연결이 되어 있지 않습니다.", () => StaticManager.Instance.ChangeScene("1. Login"));
        }

        if (initialBro.IsSuccess())
        {
            Debug.LogWarning("뒤끝 초기화가 완료되었습니다.");
            CreateSendQueueMgr();
            SetErrorHandler();
        }

        else
        {
            StaticManager.UI.AlertUI.OpenUI("네트워크에 연결이 되어 있지 않습니다.", () => StaticManager.Instance.ChangeScene("1. Login"));
        }
    }

    void Update()
    {
        if (Backend.IsInitialized)
        {
            Backend.AsyncPoll();
            Backend.ErrorHandler.Poll();
        }
    }
    
    //모든 뒤끝 함수에서 에러 발생 시, 각 에러에 따라 호출해주는 핸들러
    private void SetErrorHandler()
    {
        Backend.ErrorHandler.InitializePoll(true);

        //서버 점검 에러 발생 시
        Backend.ErrorHandler.OnMaintenanceError = () =>
        {
            Debug.LogError("점검 에러 발생!");
            StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(13));
        };

        //403 에러 발생 시
        Backend.ErrorHandler.OnTooManyRequestError = () =>
        {
            StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(14));
        };

        //액세스 토큰 만료 후 리프레시 토큰 실패 시
        Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () =>
        {
            StaticManager.UI.AlertUI.OpenUI(StaticManager.Langauge.Localize(15));
        };
    }

    //SendQueue를 관리해주는 SendQueue 매니저 생성
    private void CreateSendQueueMgr()
    {
        var obj = new GameObject();
        obj.name = "SendQueueMgr";
        obj.transform.SetParent(this.transform);
        obj.AddComponent<SendQueueMgr>();
    }

    //업데이트가 발생한 이후에 호출에 대한 응답을 반환해주는 대리자 함수
    public delegate void AfterUpdateFunc(BackendReturnObject callback);
    
    //값이 바뀐 데이터가 있을 시 저장
    public void UpdateAllGameData(AfterUpdateFunc afterUpdateFunc)
    {
        string info = string.Empty;
        
        //바뀥 데이터가 몇 개 있는지 체크
        List<GameData> gameDatas = new List<GameData>();

        foreach(var gameData in backendGameData.GameDataList)
        {
            if (gameData.Value.IsChangedData)
            {
                info += gameData.Value.GetTableName() + "\n";
                gameDatas.Add(gameData.Value);
            }
        }

        if (gameDatas.Count <= 0)
            afterUpdateFunc(null);

        else if(gameDatas.Count == 1)
        {
            //하나라면 찾아서 해당 테이블만 업데이트
            foreach(var gameData in gameDatas)
            {
                if (gameData.IsChangedData)
                {
                    gameData.Update(callback =>
                    {
                        //성공할 경우 데이터 변경 여부를 false로 변경
                        if (callback.IsSuccess())
                        {
                            gameData.IsChangedData = false;
                        }
                        else
                        {
                            Debug.LogError(callback.ToString() + "\n" + info);
                        }
                        Debug.LogError($"UpdateV2 : {callback}\n업데이트 테이블 : \n{info}");

                        if (afterUpdateFunc != null)
                            afterUpdateFunc(callback);  //지정한 대리자 함수 호출
                    });
                }
            }
        }

        else
        {
            //2개 이상이라면 트랜잭션에 묶어서 업데이트
            //단 10개 이상이면 트랜잭션 실패 주의
            List<TransactionValue> transactionList = new List<TransactionValue>();

            //변경된 데이터만큼 트랜잭션 추가
            foreach(var gameData in gameDatas)
            {
                transactionList.Add(gameData.GetTransactionValue());
            }

            SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, transactionList, callback =>
            {
                Debug.LogError($"Backend.BMember.TransactionWriteV2 : {callback}");

                if (callback.IsSuccess())
                {
                    foreach(var data in gameDatas)
                    {
                        data.IsChangedData = false;
                    }
                }
                else
                {
                    Debug.LogError(callback.ToString() + "\n" + info);
                }

                if (afterUpdateFunc != null)
                    afterUpdateFunc(callback);
            });
        }
    }
    
    //로딩에서 할당할 뒤끝 정보 클래스 초기화
    public void InitGameData()
    {
        backendGameData = new();
    }
}
