using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Animal
{
    public class Item
    {
        public int Level { get; private set; }
        public int Chicken_Upgrade { get; private set; }
        public int Chicken_Special { get; private set; }
        public int Chicken_Speed { get; private set; }
        public int Cow_Upgrade { get; private set; }
        public int Cow_Special { get; private set; }
        public int Cow_Speed { get; private set; }

        public Item(JsonData json)
        {
            Level = int.Parse(json["Level"].ToString());
            Chicken_Upgrade = int.Parse(json["Chicken_Upgrade"].ToString());
            Chicken_Special = int.Parse(json["Chicken_Special"].ToString());
            Chicken_Speed = int.Parse(json["Chicken_Speed"].ToString());
            Cow_Upgrade = int.Parse(json["Cow_Upgrade"].ToString());
            Cow_Special = int.Parse(json["Cow_Special"].ToString());
            Cow_Speed = int.Parse(json["Cow_Speed"].ToString());
        }
    }

    public class AnimalChart : Base.Chart
    {
        public readonly List<Item> animalSheet = new();

        public override string GetChartFileName()
        {
            return "AnimalSheet";
        }

        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item item = new Item(eachItem);
                
                animalSheet.Add(item);
            }
        }
    }
}
