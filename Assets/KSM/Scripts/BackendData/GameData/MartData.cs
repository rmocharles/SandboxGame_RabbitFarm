using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    public class MartData : Base.GameData
    {
        public class Item
        {
            public bool IsOpen;
            public int ItemCode;
            public int ItemCount;

            public Item(bool isOpen, int itemCode, int itemCount)
            {
                this.IsOpen = isOpen;
                this.ItemCode = itemCode;
                this.ItemCount = itemCount;
            }
        }
        
        private Dictionary<int, Item> martDic = new Dictionary<int, Item>();

        public IReadOnlyDictionary<int, Item> Dictionary => (IReadOnlyDictionary<int, Item>)martDic.AsReadOnlyCollection();

        public override string GetColumnName()
        {
            return "MartInfo";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), martDic);

            return param;
        }

        public override string GetTableName()
        {
            return "MartData";
        }

        protected override void InitializeData()
        {
            martDic.Clear();

            for (int i = 0; i < 15; i++)
            {
                Item item = new Item(i > 1 && i < 6, -1, 0);
                martDic.Add(i, item);
            }
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for (int i = 0; i < 15; i++)
            {
                bool isOpen = Boolean.Parse(gameDataJson[i]["IsOpen"].ToString());
                int itemCode = int.Parse(gameDataJson[i]["ItemCode"].ToString());
                int itemCount = int.Parse(gameDataJson[i]["ItemCount"].ToString());
                martDic.Add(i, new Item(isOpen, itemCode, itemCount));
            }
        }
        
        //==========================================================
        public bool IsExistItem(int tableNumber, int itemCode)
        {
            return martDic[tableNumber].ItemCode == itemCode && martDic[tableNumber].ItemCount > 0;
        }

        public void SetItem(int tableNumber, int itemCode, int itemCount)
        {
            IsChangedData = true;
            martDic[tableNumber].ItemCode = itemCode;
            martDic[tableNumber].ItemCount = itemCount;
        }

        public void AddItem(int tableNumber, int itemCount)
        {
            IsChangedData = true;
            martDic[tableNumber].ItemCount += itemCount;
        }

        public void SetOpen(int tableNumber, bool isOpen)
        {
            IsChangedData = true;
            martDic[tableNumber].IsOpen = isOpen;
        }
    }
}
