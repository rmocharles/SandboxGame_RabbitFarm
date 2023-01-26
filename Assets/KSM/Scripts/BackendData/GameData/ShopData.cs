using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    //===========================================================
    //ShopData 테이블의 데이터를 담당하는 클래스
    //===========================================================
    public class ShopData : Base.GameData
    {
        public string[] packageItem =
        {
            "Package_0",
            "Package_1"
        };

        public string[] diamondItem =
        {
            "Diamond_0",
            "Diamond_1",
            "Diamond_2",
            "Diamond_3"
        };

        public string[] goldItem =
        {
            "Gold_0",
            "Gold_1"
        };

        public string[] petItem =
        {
            "Pet_0",
            "Pet_1",
            "Pet_2",
            "Pet_3"
        };

        public Dictionary<string, int> shopDic = new Dictionary<string, int>();
        public IReadOnlyDictionary<string, int> Dictionary => (IReadOnlyDictionary<string, int>)shopDic.AsReadOnlyCollection();

        public override string GetColumnName()
        {
            return "ShopInfo";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), shopDic);

            return param;
        }

        public override string GetTableName()
        {
            return "ShopData";
        }

        protected override void InitializeData()
        {
            shopDic.Clear();
            
            for(int i = 0; i < packageItem.Length; i++)
                shopDic.Add(packageItem[i], 0);

            for (int i = 0; i < diamondItem.Length; i++)
                shopDic.Add(diamondItem[i], 0);
            
            for(int i = 0; i < goldItem.Length; i++)
                shopDic.Add(goldItem[i], 0);

            for (int i = 0; i < petItem.Length; i++)
                shopDic.Add(petItem[i], 0);
        }

        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for(int i = 0; i < packageItem.Length; i++)
                shopDic.Add(packageItem[i], int.Parse(gameDataJson[packageItem[i]].ToString()));
            
            for(int i = 0; i < diamondItem.Length; i++)
                shopDic.Add(diamondItem[i], int.Parse(gameDataJson[diamondItem[i]].ToString()));
            
            for(int i = 0; i < goldItem.Length; i++)
                shopDic.Add(goldItem[i], int.Parse(gameDataJson[goldItem[i]].ToString()));
            
            for(int i = 0; i < petItem.Length; i++)
                shopDic.Add(petItem[i], int.Parse(gameDataJson[petItem[i]].ToString()));
        }
        
        //===================================================================
        public void PurchaseItem(string itemName)
        {
            IsChangedData = true;
            shopDic[itemName]++;
            
            //패키지를 구매했을 경우
            if (itemName == packageItem[0] || itemName == packageItem[1])
            {
                
            }
            else
            {
                
            }
        }

        public int GetItem(string itemName)
        {
            return shopDic[itemName];
        }
    }
}
