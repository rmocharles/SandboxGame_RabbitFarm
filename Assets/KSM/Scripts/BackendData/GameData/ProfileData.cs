using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BackendData.GameData
{
    public class ProfileData : Base.GameData
    {
        private Dictionary<int, bool> profileDic = new Dictionary<int, bool>();
        public IReadOnlyDictionary<int, bool> Dictionary => (IReadOnlyDictionary<int, bool>)profileDic.AsReadOnlyCollection();
        
        public int Represent { get; private set; }

        public override string GetColumnName()
        {
            return null;
        }

        public override string GetTableName()
        {
            return "ProfileData";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("ProfileData", profileDic);
            param.Add("Profile_Represent", Represent);

            return param;
        }

        protected override void InitializeData()
        {
            profileDic.Clear();

            for (int i = 0; i < 12; i++)
                profileDic.Add(i, i < 7);

            Represent = 1;
        }
        
        //Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        //서버에서 데이터를 불러오는 함수는  BackendData.Base.GameData 의 BackendGameDataLoad() 함수 참고
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for(int i = 0; i < 12; i++)
                profileDic.Add(i, Boolean.Parse(gameDataJson["ProfileData"][i].ToString()));

            Represent = int.Parse(gameDataJson["Profile_Represent"].ToString());
        }
        
        //======================================
        public void SetRerpesentImage(int code)
        {
            IsChangedData = true;
            
            Represent = code;
            GameManager.Instance.profileButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Profile/profile_" + (code + 1).ToString("D2"));
        }

        public void AddRepresentImage(int code)
        {
            IsChangedData = true;
            
            Represent = code;
            profileDic[code] = true;
            GameManager.Instance.profileButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Profile/profile_" + (code + 1).ToString("D2"));
        }
    }
}
