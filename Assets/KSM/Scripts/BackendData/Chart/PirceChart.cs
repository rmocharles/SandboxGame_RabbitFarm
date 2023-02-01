using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Price
{
    public class Item
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int Price { get; private set; }

        public Item(JsonData json)
        {
            Name = json["Name"].ToString();
            Type = json["Type"].ToString();
            Price = int.Parse(json["Price"].ToString());
        }
    }
    public class PriceChart : Base.Chart
    {
        public readonly List<Item> priceSheet = new List<Item>();
        
        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item item = new Item(eachItem);
                
                priceSheet.Add(item);
            }
        }

        public override string GetChartFileName()
        {
            return "PriceSheet";
        }
        
        //===========================================
        public string GetType(string name)
        {
            for (int i = 0; i < priceSheet.Count; i++)
            {
                if (priceSheet[i].Name == name)
                    return priceSheet[i].Type;
            }

            return null;
        }
        
        public int GetPrice(string name)
        {
            for (int i = 0; i < priceSheet.Count; i++)
            {
                if (priceSheet[i].Name == name)
                    return priceSheet[i].Price;
            }

            return 0;
        }
    }
    
    
}
