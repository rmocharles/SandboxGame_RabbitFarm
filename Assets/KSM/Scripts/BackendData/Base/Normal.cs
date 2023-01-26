using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BackendData.Base
{
    //================================================================================
    //차트, 게임 정보 등 일괄적으로 동일한 로직을 수행하기 위해 만드는 베이스 클래스
    //================================================================================
    public class Normal
    {
        
        //Loading에서 사용하는 불러오기가 완료된 이후에 호출되는 함수
        public delegate void AfterBackendLoadFunc(bool isSuccess, string errorInfo);
        
        //기본적인 형식
        public virtual void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc)
        {
            afterBackendLoadFunc(true, string.Empty);
        }
    }
}
