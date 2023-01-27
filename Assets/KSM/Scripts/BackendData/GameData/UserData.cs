using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;

namespace BackendData.GameData
{
    public class UserData : Base.GameData
    {
        public int Level { get; private set; }
        public int Exp { get; private set; }
        public int Gold { get; private set; }
        public int Diamond { get; private set; }
        public int Tutorial { get; private set; }
        
        //데이터가 존재하지 않을 경우 초기값 설정
        protected override void InitializeData()
        {
            Level = 1;
            Exp = 0;
            Diamond = 100;
            Gold = 1000;
            Tutorial = 0;
            
            #if UNITY_EDITOR
            Level = 50;
            Exp = 0;
            Diamond = 9999;
            Gold = 9000;
            Tutorial = 0;
            #endif
        }
        
        //Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        //서버에서 데이터를 불러오는 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수 참고
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            Level = int.Parse(gameDataJson["Level"].ToString());
            Exp = int.Parse(gameDataJson["Exp"].ToString());

            Gold = int.Parse(gameDataJson["Gold"].ToString());
            Diamond = int.Parse(gameDataJson["Diamond"].ToString());
            Tutorial = int.Parse(gameDataJson["Tutorial"].ToString());
        }
        
        //테이블 이름 설정 함수
        public override string GetTableName()
        {
            return "UserData";
        }

        //컬럼 이름 설정 함수
        public override string GetColumnName()
        {
            return null;
        }

        //데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
        public override Param GetParam()
        {
            Param param = new Param();
            
            param.Add("Level", Level);
            param.Add("Exp", Exp);
            param.Add("Gold", Gold);
            param.Add("Diamond", Diamond);
            param.Add("Tutorial", Tutorial);

            return param;
        }

        //====================================
        public void SetGold(int value)
        {
            IsChangedData = true;
            Gold = value;
        }

        public void SetDiamond(int value)
        {
            IsChangedData = true;
            Diamond = value;
        }

        public void AddGold(int value)
        {
            IsChangedData = true;
            Gold += value;
        }

        public void AddDiamond(int value)
        {
            IsChangedData = true;
            Diamond += value;
        }

        public void AddLevel(int value)
        {
            IsChangedData = true;
            Level += value;
        }

        public void AddExp(int value)
        {
            IsChangedData = true;
            Exp += value;
        }

        public void SetTutorial(int index)
        {
            IsChangedData = true;
            Tutorial = index;
        }
    }
}
