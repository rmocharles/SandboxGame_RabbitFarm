using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;

namespace BackendData.GameData
{
    public class WeatherData : Base.GameData
    {
        public int Type = 0;
        public string RemainTime = string.Empty;
        
        public override string GetTableName()
        {
            return "WeatherData";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("Type", Type);
            param.Add("RemainTime", RemainTime);

            return param;
        }

        protected override void InitializeData()
        {
            Type = 0;
            RemainTime = string.Empty;
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            Type = int.Parse(gameDataJson["Type"].ToString());
            RemainTime = gameDataJson["RemainTime"].ToString();
        }
        
        //======================================================================
        public void SetType(int type)
        {
            IsChangedData = true;

            Type = type;
            switch (type)
            {
                case 0:
                    RemainTime = string.Empty;
                    break;
            
                case 1:
                    RemainTime = DateTime.UtcNow.AddMinutes(5).ToString();
                    break;
            
                case 2:
                    RemainTime = DateTime.UtcNow.AddMinutes(5).ToString();
                    break;
            }
        }
    }
    
    
}
