using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;

namespace BackendData.Base
{
    //==================================================================
    //게임 데이터의 불러오기와 쓰기에 대한 공통적인 로직을 가진 클래스
    //==================================================================
    public abstract class GameData : Normal
    {
        private string inDate;

        public string GetInDate()
        {
            return inDate;
        }
        
        public bool IsChangedData { get; set; }

        public abstract string GetTableName();      //자식 객체가 설정한 테이블 이름 가져오는 함수
        public abstract string GetColumnName();     //자식 객체가 설정한 컬럼 이름 가져오는 함수
        public abstract Param GetParam();           //자식 객체가 설정한 업데이트에 사용될 Param을 가져오는 함수
        protected abstract void InitializeData();   //자식 객체가 설정한 데이터 초기화를 해주는 함수
        
        protected abstract void SetServerDataToLocal(JsonData gameDataJson);    //자식 객체가 설정한 불러오기 함수 호출 이후 각 테이블에 맞게 파싱하는 함수

        public void BackendGameDataLoad(AfterBackendLoadFunc afterBackendLoadFunc)
        {
            string tableName = GetTableName();
            string columnName = GetColumnName();

            bool isSuccess = false;
            string errorInfo = string.Empty;
            
            //[뒤끝] 내 게임정보 불러오기 함수
            SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback =>
            {
                try
                {
                    Debug.LogWarning($"Backend.GameData.GetMyData({tableName}) : {callback}");
                    if (callback.IsSuccess())
                    {
                        //불러온 데이터가 하나라도 존재할 경우
                        if (callback.FlattenRows().Count > 0)
                        {

                            //이후 업데이트에 사용될 각 데이터의 inDate값 저장
                            inDate = callback.FlattenRows()[0]["inDate"].ToString();

                            //Dictionary 등 데이터 저장을 위해 컬럼 값을 설정했을 경우
                            if (string.IsNullOrEmpty(columnName))
                            {
                                SetServerDataToLocal(callback.FlattenRows()[0]);
                            }
                            else
                            {
                                //설정하지 않았을 경우(UserData)
                                //ColumnName까지 진입한 후, Json을 리턴
                                SetServerDataToLocal(callback.FlattenRows()[0][columnName]);
                            }

                            isSuccess = true;
                            //불러오기가 끝난 이후에 호출되는 함수 호출
                            afterBackendLoadFunc(isSuccess, errorInfo);
                        }
                        else
                        {
                            //불러올 데이터가 없을 경우, 서버에 존재하지 않을 경우
                            //데이터 생성
                            BackendInsert(afterBackendLoadFunc);
                        }
                    }
                    else
                    {
                        //데이터 존재 여부 상관없이 에러가 발생했을 경우(서버에 데이터가 존재할 수도 있음)
                        errorInfo = callback.ToString();
                        afterBackendLoadFunc(isSuccess, errorInfo);
                    }
                }
                catch (Exception e)
                {
                    //예외가 발생했을 경우
                    //파싱 실패 뜸
                    errorInfo = e.ToString();
                    afterBackendLoadFunc(isSuccess, errorInfo);
                }
            });
        }
        
        //서버에 데이터가 존재하지 않을 경우, 데이터 새로 삽입
        private void BackendInsert(AfterBackendLoadFunc afterBackendLoadFunc)
        {
            string tableName = GetTableName();
            bool isSuccess = false;
            string errorInfo = string.Empty;
            
            //데이터 초기화(각 자식 객체가 설정)
            InitializeData();
            
            //[뒤끝] 게임 정보 삽입 함수
            SendQueue.Enqueue(Backend.GameData.Insert, tableName, GetParam(), callback =>
            {
                try
                {
                    Debug.LogWarning($"Backend.GameData.Insert({tableName}) : {callback}");

                    if (callback.IsSuccess())
                    {
                        isSuccess = true;
                        inDate = callback.GetInDate();
                    }
                    else
                    {
                        errorInfo = callback.ToString();
                    }
                }
                catch (Exception e)
                {
                    errorInfo = e.ToString();
                }

                finally
                {
                    afterBackendLoadFunc(isSuccess, errorInfo);
                }
            });
        }
        
        //해당 테이블에 업데이트 할 데이터 트랜잭션(한 번에 여러 테이블 저장)으로 만들어 리턴
        public TransactionValue GetTransactionValue()
        {
            return TransactionValue.SetUpdateV2(GetTableName(), GetInDate(), Backend.UserInDate, GetParam());
        }

        public void BackendGameDataLoadByTransaction(JsonData gameDataJson, AfterBackendLoadFunc afterBackendLoadFunc)
        {
            string errorInfo = string.Empty;

            try
            {
                //이후 업데이트에 사용될 각 데이터의 inDate 값 저장
                inDate = gameDataJson["inDate"].ToString();

                //Dictionary 등 데이터 저장을 위해 컬럼 값을 설정 했을 경우
                if (string.IsNullOrEmpty(GetColumnName()))
                {
                    SetServerDataToLocal(gameDataJson);
                }
                else
                {
                    SetServerDataToLocal(gameDataJson[GetColumnName()]);
                }

                afterBackendLoadFunc(true, errorInfo);
            }
            catch (Exception e)
            {
                errorInfo = e.ToString();
                afterBackendLoadFunc(false, errorInfo);
            }
        }
        
        //업데이트가 완료된 이후 리턴 값과 함께 호출되는 함수
        public delegate void AfterCallback(BackendReturnObject callback);
        
        //해당 테이블에 데이터 업데이트
        public void Update(AfterCallback afterCallback)
        {
            SendQueue.Enqueue(Backend.GameData.UpdateV2, GetTableName(), GetInDate(), Backend.UserInDate, GetParam(), callback =>
            {
                Debug.LogWarning($"Backend.GameData.UpdateV2({GetTableName()}, {GetInDate()}, {Backend.UserInDate} : {callback}");
                afterCallback(callback);
            });
        }
    }
}
