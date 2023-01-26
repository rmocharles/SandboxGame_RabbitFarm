using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Harvest
{
    public class Item
    {
        public string HarvestName { get; private set; }
        public string HarvestName_EN { get; private set; }
        public int RequireLevel { get; private set; }
        public int Price { get; private set; }
        public int[] Reward = new int[3];
        public int[] Exp = new int[3];
        public int CoolTime { get; private set; }
        
        public string Info { get; private set; }
        public string Info_EN { get; private set; }
        public string Effect { get; private set; }
        public string Effect_EN { get; private set; }


        public Item(JsonData json)
        {
            HarvestName = json["Name_ko"].ToString();
            HarvestName_EN = json["Name_en"].ToString();
            RequireLevel = int.Parse(json["RequireLevel"].ToString());
            Price = int.Parse(json["Price"].ToString());
            Exp[0] = int.Parse(json["First_Exp"].ToString());
            Reward[0] = int.Parse(json["First_Reward"].ToString());
            Exp[1] = int.Parse(json["Second_Exp"].ToString());
            Reward[1] = int.Parse(json["Second_Reward"].ToString());
            Exp[2] = int.Parse(json["Third_Exp"].ToString());
            Reward[2] = int.Parse(json["Third_Reward"].ToString());
            CoolTime = int.Parse(json["CoolTime"].ToString());
            Info = json["Info_ko"].ToString();
            Info_EN = json["Info_en"].ToString();
            Effect = json["Effect_ko"].ToString();
            Effect_EN = json["Effect_en"].ToString();
        }
    }

    public class HarvestChart : Base.Chart
    {
        //각 차트의 row 정보를 담는 변수
        public readonly List<Item> harvestSheet = new();

        public override string GetChartFileName()
        {
            return "HarvestSheet";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item item = new Item(eachItem);
                
                harvestSheet.Add(item);
            }
        }
    }
}
