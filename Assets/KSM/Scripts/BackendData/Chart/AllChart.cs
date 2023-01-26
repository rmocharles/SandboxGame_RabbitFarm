using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackendData.Base;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart
{
    //==================================
    //각 종 차트에 대한 차트 정보를 불러오는 클래스
    //==================================
    public class AllChart : Normal
    {
        //차트의 파일 id를 관리하는 Dictionary
        private readonly Dictionary<string, string> chartDictionary = new();
        
        //다른 클래스에서 Add, Delete 등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, string> Dictionary => (IReadOnlyDictionary<string, string>)chartDictionary.AsReadOnlyCollection();
        
        //서버에서 데이터를 불러와 파싱하는 함수
        public override void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc)
        {
            bool isSuccess = false;
            string errorInfo = string.Empty;
            
            //[뒤끝] 차트 정보 불러오기 함수
            SendQueue.Enqueue(Backend.Chart.GetChartList, callback =>
            {
                try
                {
                    Debug.Log($"Backend.Chart.GetChartList : {callback}");

                    if (!callback.IsSuccess())
                        throw new Exception(callback.ToString());

                    JsonData json = callback.FlattenRows();

                    for (int i = 0; i < json.Count; i++)
                    {
                        string chartName = json[i]["chartName"].ToString();
                        string selectChartFileId = json[i]["selectedChartFileId"].ToString();

                        if (chartDictionary.ContainsKey(chartName))
                        {
                            Debug.Log($"동일한 차트 키 값이 존재합니다 : {chartName} - {selectChartFileId}");
                        }

                        else
                        {
                            chartDictionary.Add(chartName, selectChartFileId);
                        }
                    }

                    isSuccess = true;
                }

                catch (Exception e)
                {
                    errorInfo = e.Message;
                }
                finally
                {
                    afterBackendLoadFunc(isSuccess, errorInfo);
                }
            });
        }
    }
}
