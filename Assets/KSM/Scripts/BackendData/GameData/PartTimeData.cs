using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;

namespace BackendData.GameData
{
    public class PartTimeData : Base.GameData
    {
        public int Type { get; private set; }
        public string RemainTimer { get; private set; }

        public override string GetTableName()
        {
            return "PartTimeData";
        }

        public override string GetColumnName()
        {
            return null;
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add("Type", Type);
            param.Add("RemainTimer", RemainTimer);

            return param;
        }

        protected override void InitializeData()
        {
            Type = -1;
            RemainTimer = string.Empty;
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            Type = int.Parse(gameDataJson["Type"].ToString());
            RemainTimer = gameDataJson["RemainTimer"].ToString();
        }
        
        //=============================================================
        public void SetPartTime(int type)
        {
            IsChangedData = true;
            Type = type;

            switch (type)
            {
                case -1:
                    RemainTimer = string.Empty;
                    break;
                
                //10분
                case 0:
                    RemainTimer = GameManager.Instance.nowTime.AddMinutes(10).ToString();
                    break;
                
                //24시간
                case 1:
                    RemainTimer = GameManager.Instance.nowTime.AddDays(1).ToString();
                    break;
                
                case 2:
                    RemainTimer = "~";
                    break;
            }
        }
        
        public void AddPartTime(int type)
        {
            IsChangedData = true;
            Type = type;

            switch (type)
            {
                //2시간
                case 0:
                    RemainTimer = DateTime.Parse(RemainTimer).AddHours(2).ToString();
                    break;
                
                //24시간
                case 1:
                    RemainTimer = DateTime.Parse(RemainTimer).AddDays(1).ToString();
                    break;
            }
        }
    }
}
