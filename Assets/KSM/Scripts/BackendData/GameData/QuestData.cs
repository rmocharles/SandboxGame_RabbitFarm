using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    public class QuestData : Base.GameData
    {
        public class Item
        {
            public int Level;
            public int Count;

            public Item(int level, int count)
            {
                Level = level;
                Count = count;
            }
        }
        
        public Dictionary<int, Item> questDic = new Dictionary<int, Item>();
        public IReadOnlyDictionary<int, Item> Dictionary => (IReadOnlyDictionary<int, Item>)questDic.AsReadOnlyCollection();
        public override string GetTableName()
        {
            return "QuestData";
        }

        public override string GetColumnName()
        {
            return "QuestInfo";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), questDic);

            return param;
        }

        protected override void InitializeData()
        {
            questDic.Clear();

            for (int i = 0; i < 8; i++)
            {
                Item item = new Item(1, 0);
                questDic.Add(i, item);
            }
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for (int i = 0; i < 8; i++)
            {
                int level = int.Parse(gameDataJson[i]["Level"].ToString());
                int count = int.Parse(gameDataJson[i]["Count"].ToString());
                questDic.Add(i, new Item(level, count));
            }
        }
        
        //========================================================
        public void SetQuest(int index, int level)
        {
            IsChangedData = true;
            //다음단계
            if (level == -1)
            {
                questDic[index].Level++;
            }
            else
                questDic[index].Level = level;
        }

        public void SetCount(int index, int count)
        {
            IsChangedData = true;
            questDic[index].Count = count;
        }

        public void AddCount(int index, int count)
        {
            IsChangedData = true;
            questDic[index].Count += count;
        }
    }

}