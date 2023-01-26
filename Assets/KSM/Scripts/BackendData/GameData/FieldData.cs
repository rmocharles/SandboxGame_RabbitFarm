using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    //===================================================================================
    //FieldData 테이블의 데이터를 담당하는 클래스
    //===================================================================================

    public class FieldInfo
    {
        public int FieldLevel;
        public int HarvestCode;
        public string RemainTimer;

        public FieldInfo(int fieldLevel, int harvestCode, string remainTimer)
        {
            this.FieldLevel = fieldLevel;
            this.HarvestCode = harvestCode;
            this.RemainTimer = remainTimer;
        }
    }

    public class FieldData : Base.GameData
    {
        //Field의 각 정보를 담는 Dictionary
        private Dictionary<int, FieldInfo> fieldDic = new Dictionary<int, FieldInfo>();
        //다른 클래스에서 Add, Delete 등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<int, FieldInfo> Dictionary => (IReadOnlyDictionary<int, FieldInfo>)fieldDic.AsReadOnlyCollection();
        
        //테이블 이름 설정 함수
        public override string GetTableName()
        {
            return "FieldData";
        }
        
        //컬럼 이름 설정 함수
        public override string GetColumnName()
        {
            return "FieldData";
        }
        
        //데이터가 존재하지 않을 경우, 초기값 설정
        protected override void InitializeData()
        {
            fieldDic.Clear();

            for (int i = 0; i < 9; i++)
            {
                //처음 밭만 오픈, 나머지는 잠금
                fieldDic.Add(i, new FieldInfo(i == 0 ? 0 : -1, -1, String.Empty));
            }
        }
        
        //데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
        //Dictionary 하나만 삽입
        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), fieldDic);

            return param;
        }
        
        //Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        //서버에서 데이터를 불러오는 함수는  BackendData.Base.GameData 의 BackendGameDataLoad() 함수 참고
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for (int i = 0; i < gameDataJson.Count; i++)
            {
                int fieldLevel = int.Parse(gameDataJson[i]["FieldLevel"].ToString());
                int harvestCode = int.Parse(gameDataJson[i]["HarvestCode"].ToString());
                string remainTimer = gameDataJson[i]["RemainTimer"].ToString();
                
                fieldDic.Add(i, new FieldInfo(fieldLevel, harvestCode, remainTimer));
            }
        }
        
        //========================================
        public void SetField(int fieldNumber, int fieldLevel)
        {
            IsChangedData = true;
            fieldDic[fieldNumber].FieldLevel = fieldLevel;
        }
        public void SetField(int fieldNumber, int fieldLevel, int harvestCode)
        {
            IsChangedData = true;
            fieldDic[fieldNumber].FieldLevel = fieldLevel;
            fieldDic[fieldNumber].HarvestCode = harvestCode;
        }
        public void SetField(int fieldNumber, int fieldLevel, int harvestCode, string remainTimer)
        {
            IsChangedData = true;
            fieldDic[fieldNumber].FieldLevel = fieldLevel;
            fieldDic[fieldNumber].HarvestCode = harvestCode;
            fieldDic[fieldNumber].RemainTimer = remainTimer;
        }

        public void SetField(int fieldNumber, string dateTime)
        {
            IsChangedData = true;

            fieldDic[fieldNumber].RemainTimer = dateTime;
        }
    }
}
