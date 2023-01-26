using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    public class AnimalInfo
    {
        public int Upgrade;
        public string RemainTimer;

        public AnimalInfo(int Upgrade, string RemainTimer)
        {
            this.Upgrade = Upgrade;
            this.RemainTimer = RemainTimer;
        }
    }

    public class AnimalData : Base.GameData
    {
        private Dictionary<string, AnimalInfo> animalDic = new Dictionary<string, AnimalInfo>();
        public IReadOnlyDictionary<string, AnimalInfo> Dictionary => (IReadOnlyDictionary<string, AnimalInfo>)animalDic.AsReadOnlyCollection();

        public override string GetTableName()
        {
            return "AnimalData";
        }

        public override string GetColumnName()
        {
            return "AnimalInfo";
        }

        protected override void InitializeData()
        {
            animalDic.Clear();
            
            animalDic.Add("Cow", new AnimalInfo(-1, string.Empty));
            animalDic.Add("Chicken", new AnimalInfo(-1, string.Empty));
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), animalDic);

            return param;
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            animalDic.Add("Cow", new AnimalInfo(int.Parse(gameDataJson["Cow"]["Upgrade"].ToString()), gameDataJson["Cow"]["RemainTimer"].ToString()));
            animalDic.Add("Chicken", new AnimalInfo(int.Parse(gameDataJson["Chicken"]["Upgrade"].ToString()), gameDataJson["Chicken"]["RemainTimer"].ToString()));
        }
        
        //============================================================
        public void SetAnimal(string name, int upgradeLevel)
        {
            IsChangedData = true;
            animalDic[name].Upgrade = upgradeLevel;
        }

        public void SetAnimal(string name, string remainTimer)
        {
            IsChangedData = true;
            animalDic[name].RemainTimer = remainTimer;
        }
    }
}
