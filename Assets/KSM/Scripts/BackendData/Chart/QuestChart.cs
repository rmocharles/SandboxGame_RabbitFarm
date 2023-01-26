using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Quest
{
    public class Item
    {
        public int Upgrade { get; private set; }
        public int Reward { get; private set; }
        public string Type { get; private set; }
        public int MaxUpgrade { get; private set; }
        public int MaxReward { get; private set; }

        public Item(JsonData json)
        {
            Upgrade = int.Parse(json["Per_Upgrade"].ToString());
            Reward = int.Parse(json["Per_Reward"].ToString());
            Type = json["Type"].ToString();
            MaxUpgrade = int.Parse(json["Max_Upgrade"].ToString());
            MaxReward = int.Parse(json["Max_Reward"].ToString());
        }
    }

    public class QuestChart : Base.Chart
    {
        public readonly List<Item> questSheet = new();

        public override string GetChartFileName()
        {
            return "QuestSheet";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item item = new Item(eachItem);
                
                questSheet.Add(item);
            }
        }
    }
}
