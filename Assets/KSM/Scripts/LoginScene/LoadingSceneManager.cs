using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using TMPro;
using UnityEngine;

public partial class LoginSceneManager
{
    [SerializeField] private TMP_Text loadingText;

    private int currentLoadingCount;
    private int maxLoadingCount;

    private delegate void BackendLoadStep();

    private readonly Queue<BackendLoadStep> initializeStep = new Queue<BackendLoadStep>();

    public void InitializeLoading()
    {
        versionObject.SetActive(true);
        copyRightObject.SetActive(true);
        
        initializeStep.Clear();
        
        //트랜잭션으로 불러온 후, 안불러질 경우 각자 Get 함수로 불러오는 함수
        initializeStep.Enqueue(() => {ShowDataName("1"); TransactionRead(NextStep);});
        
        //차트 정보 불러오기 함수
        initializeStep.Enqueue(() => {ShowDataName("2"); StaticManager.Backend.backendChart.ChartInfo.BackendLoad(NextStep);});
        initializeStep.Enqueue(() => {ShowDataName("3"); StaticManager.Backend.backendChart.Harvest.BackendChartDataLoad(NextStep);});
        initializeStep.Enqueue(() => {ShowDataName("4"); StaticManager.Backend.backendChart.Percent.BackendChartDataLoad(NextStep);});
        initializeStep.Enqueue(() => { ShowDataName("5"); StaticManager.Backend.backendChart.Balance.BackendChartDataLoad(NextStep);});
        initializeStep.Enqueue(() => { ShowDataName("6"); StaticManager.Backend.backendChart.Animal.BackendChartDataLoad(NextStep);});
        initializeStep.Enqueue(() => { ShowDataName("7"); StaticManager.Backend.backendChart.Quest.BackendChartDataLoad(NextStep);});
        
        //카운트 설정
        maxLoadingCount = initializeStep.Count;
        currentLoadingCount = 0;
        
        //뒤끝 데이터 초기화
        StaticManager.Backend.InitGameData();
        
        //Queue에 저장된 함수 순차적으로 실행
        NextStep(true, string.Empty);
    }

    private void ShowDataName(string text)
    {
        loginButtonGroup.SetActive(false);
        loadingText.gameObject.SetActive(true);

        string info = $"{text}...({currentLoadingCount} / {maxLoadingCount})";
        //loadingText.text = info;
        
        StaticManager.UI.SetLoading(true);
    }

    //각 뒤끝 함수를 호출하는 BackendGameDataLoad에서 실행한 결과를 처리하는 함수
    //성공하면 다음 스텝으로 이동, 실패하면 에러 UI 띄움
    private void NextStep(bool isSuccess, string errorInfo)
    {
        if (isSuccess)
        {
            currentLoadingCount++;

            if (initializeStep.Count > 0)
                initializeStep.Dequeue().Invoke();
            else
                GameStart();
        }
        else
            StaticManager.UI.AlertUI.OpenUI(errorInfo);
    }
    
    //트랜잭션 읽기 함수
    private void TransactionRead(BackendData.Base.Normal.AfterBackendLoadFunc func)
    {
        bool isSuccess = false;
        string errorInfo = String.Empty;
        
        //트랜잭션 리스트 생성
        List<TransactionValue> transactionList = new List<TransactionValue>();
        
        //게임 테이블 데이터만큼 트랜잭션 불러오기
        foreach (var gameData in StaticManager.Backend.backendGameData.GameDataList)
        {
            transactionList.Add(gameData.Value.GetTransactionValue());
        }
        
        //[뒤끝] 트랜잭션 읽기 함수
        SendQueue.Enqueue(Backend.GameData.TransactionReadV2, transactionList, callback =>
        {
            try
            {
                Debug.LogWarning($"Backend.GameData.TransactionReadV2 : {callback}");

                //데이터를 모두 불러왔을 경우
                if (callback.IsSuccess())
                {
                    JsonData gameDataJson = callback.GetFlattenJSON()["Responses"];

                    int index = 0;

                    foreach (var gameData in StaticManager.Backend.backendGameData.GameDataList)
                    {
                        initializeStep.Enqueue(() =>
                        {
                            ShowDataName(gameData.Key);

                            //불러온 데이터를 로컬에서 파싱
                            gameData.Value.BackendGameDataLoadByTransaction(gameDataJson[index++], NextStep);
                        });

                        maxLoadingCount++;
                    }

                    isSuccess = true;
                }

                else
                {
                    //트랜잭션으로 데이터를 찾지 못하여 에러가 발생 시 개별로 GetMyData 호출
                    foreach (var gameData in StaticManager.Backend.backendGameData.GameDataList)
                    {
                        initializeStep.Enqueue(() =>
                        {
                            ShowDataName(gameData.Key);
                            gameData.Value.BackendGameDataLoad(NextStep);
                        });

                        maxLoadingCount++;
                    }

                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                errorInfo = e.ToString();
            }

            finally
            {
                func.Invoke(isSuccess, errorInfo);
            }
        });
    }

    private void GameStart()
    {
        StaticManager.UI.SetLoading(false);
        //loadingText.text = StaticManager.Langauge.Localize(11);
        initializeStep.Clear();
        
        StaticManager.Instance.ChangeScene("2. Game", FadeUI.FadeType.ChangeToBlack, 2f);
    }
}
