using System.Collections;
using System.Collections.Generic;
using LitJson;
using Spine;
using UnityEngine;

namespace BackendData.Chart.Percent
{
    public class Item
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int[] Case = new int[7];
        public int[] Version = new int[3];

        public Item(JsonData json)
        {
            Name = json["Name"].ToString();
            Type = json["Type"].ToString();
            for (int i = 0; i < 7; i++)
            {
                if (i < 3)
                    Version[i] = int.Parse(json["Ver_" + (i + 1)].ToString());
                Case[i] = int.Parse(json["Case_" + (i + 1)].ToString());
            }
        }
    }

    public class PercentChart : Base.Chart
    {
        //각 차트의 row정보를 담는 변수
        public readonly List<Item> percentSheet = new();

        public override string GetChartFileName()
        {
            return "PercentSheet";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item item = new Item(eachItem);
                
                percentSheet.Add(item);
            }
        }
    }
}
