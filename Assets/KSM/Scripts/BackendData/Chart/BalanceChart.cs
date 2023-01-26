using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Balance
{
    public class Item
    {
        public int Exp { get; private set; }
        public int Carrot { get; private set; }
        public int Potato { get; private set; }
        public int Tomato { get; private set; }
        public int Cucumber { get; private set; }
        public int Peach { get; private set; }
        public int Apple { get; private set; }
        public int Pumpkin { get; private set; }
        public int Pear { get; private set; }
        public int Cherry { get; private set; }

        public Item(JsonData json)
        {
            Exp = int.Parse(json["Exp"].ToString());
            Carrot = int.Parse(json["Adjustment_Carrot"].ToString());
            Potato = int.Parse(json["Adjustment_Potato"].ToString());
            Tomato = int.Parse(json["Adjustment_Tomato"].ToString());
            Cucumber = int.Parse(json["Adjustment_Cucumber"].ToString());
            Peach = int.Parse(json["Adjustment_Peach"].ToString());
            Apple = int.Parse(json["Adjustment_Apple"].ToString());
            Pumpkin = int.Parse(json["Adjustment_Pumpkin"].ToString());
            Pear = int.Parse(json["Adjustment_Pear"].ToString());
            Cherry = int.Parse(json["Adjustment_Cherry"].ToString());
        }
    }

    public class BalanceChart : Base.Chart
    {
        public int MaxLevel { get; private set; }

        //각 차트의 row 정보를 담는 변수
        public readonly List<Item> balanceSheet = new();

        public override string GetChartFileName()
        {
            return "BalanceSheet";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            int count = 0;
            foreach (JsonData eachItem in json)
            {
                Item item = new Item(eachItem);
                
                balanceSheet.Add(item);
                count++;
            }

            MaxLevel = count;
        }
    }
}