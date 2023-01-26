using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    public class PetData : Base.GameData
    {
        private Dictionary<int, bool> petDic = new Dictionary<int, bool>();
        public IReadOnlyDictionary<int, bool> Dictionary => (IReadOnlyDictionary<int, bool>)petDic.AsReadOnlyCollection();
        
        public override string GetTableName()
        {
            return "PetData";
        }

        public override string GetColumnName()
        {
            return "PetInfo";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), petDic);

            return param;
        }

        protected override void InitializeData()
        {
            petDic.Clear();

            for (int i = 0; i < 5; i++)
            {
                if(i == 0)
                    petDic.Add(i, true);
                else
                {
                    petDic.Add(i, false);
                }
            }
            
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for (int i = 0; i < 5; i++)
            {
                petDic.Add(i, Boolean.Parse(gameDataJson[i].ToString()));
            }
        }
        
        //===============================================================

        public void SetPet(int number, bool isOpen)
        {
            IsChangedData = true;
            petDic[number] = isOpen;
        }
    }
}
