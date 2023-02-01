using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.GameData
{
    //===========================================================
    //InventoryData 테이블의 데이터를 담당하는 클래스
    //===========================================================
    public class InventoryData : Base.GameData
    {
        public string[,] harvestItem =
        {
            { "Carrot", "Giant_Carrot", "Rainbow_Carrot" },
            { "Potato", "Giant_Potato", "Rainbow_Potato" },
            { "Tomato", "Giant_Tomato", "Rainbow_Tomato" },
            { "Cucumber", "Giant_Cucumber", "" },
            { "Peach", "Honey_Peach", "" },
            { "Apple", "Honey_Apple", "" },
            { "Pumpkin", "", "" },
            { "Pear", "", "" },
            { "Cherry", "", "" }
        };

        public string[,] animalItem =
        {
            { "Milk", "Super_Milk" },
            { "Egg", "Golden_Egg" }
        };

        public string Fertilizer = "Fertilizer";
        
        //Inventory의 각 정보를 담는 Dictionary
        private Dictionary<string, int> inventoryDic = new Dictionary<string, int>();
        //다른 클래스에서 Add, Delete 등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, int> Dictionary => (IReadOnlyDictionary<string, int>)inventoryDic.AsReadOnlyCollection();

        public override string GetColumnName()
        {
            return "InventoryInfo";
        }

        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), inventoryDic);

            return param;
        }

        public override string GetTableName()
        {
            return "InventoryData";
        }
        
        //데이터가 존재하지 않을 경우 초기값 설정
        protected override void InitializeData()
        {
            inventoryDic.Clear();

            for (int i = 0; i < harvestItem.GetLength(0); i++)
            {
                for (int j = 0; j < harvestItem.GetLength(1); j++)
                {
                    if(!string.IsNullOrEmpty(harvestItem[i, j]))
                        inventoryDic.Add(harvestItem[i, j], 0);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                inventoryDic.Add(animalItem[i, 0], 0);
                inventoryDic.Add(animalItem[i, 1], 0);
            }
            
#if UNITY_EDITOR
            inventoryDic.Add(Fertilizer, 99);
#else
            inventoryDic.Add(Fertilizer, 3);
#endif
            
        }
        
        //Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        //서버에서 데이터를 불러오는 함수는  BackendData.Base.GameData 의 BackendGameDataLoad() 함수 참고
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for (int i = 0; i < harvestItem.GetLength(0); i++)
            {
                for (int j = 0; j < harvestItem.GetLength(1); j++)
                {
                    if (!string.IsNullOrEmpty(harvestItem[i, j]))
                    {
                        string itemName = harvestItem[i, j];
                        int itemCount = int.Parse(gameDataJson[harvestItem[i, j]].ToString());

                        inventoryDic.Add(itemName, itemCount);
                    }
                }
            }

            for (int i = 0; i < animalItem.GetLength(0); i++)
            {
                for (int j = 0; j < animalItem.GetLength(1); j++)
                {
                    string itemName = animalItem[i, j];
                    int itemCount = int.Parse(gameDataJson[animalItem[i, j]].ToString());

                    inventoryDic.Add(itemName, itemCount);
                }
            }

            inventoryDic.Add(Fertilizer, int.Parse(gameDataJson["Fertilizer"].ToString()));
        }
        
        //====================================================
        public void SetItem(string itemName, int itemCount)
        {
            IsChangedData = true;
            inventoryDic[itemName] = itemCount;
        }

        public void SetItem(int[] array, int itemCount)
        {
            IsChangedData = true;
            inventoryDic[harvestItem[array[0], array[1]]] = itemCount;
        }

        public void AddItem(string itemName, int itemCount)
        {
            IsChangedData = true;
            inventoryDic[itemName] += itemCount;
        }

        public void AddItem(int itemCode, int itemCount)
        {
            if (itemCode >= 0 && itemCode < 9)
            {
                inventoryDic[harvestItem[itemCode, 0]] += itemCount;
            }

            else if (itemCode >= 9 && itemCode < 15)
            {
                inventoryDic[harvestItem[itemCode - 9, 1]] += itemCount;
            }
            
            else if (itemCode >= 15 && itemCode < 18)
            {
                inventoryDic[harvestItem[itemCode - 15, 2]] += itemCount;
            }
            
            else if (itemCode >= 18 && itemCode < 20)
            {
                inventoryDic[animalItem[itemCode - 18, 0]] += itemCount;
            }
            
            else if (itemCode >= 20 && itemCode < 22)
            {
                inventoryDic[animalItem[itemCode - 20, 1]] += itemCount;
            }
        }

        public void AddItem(int[] array, int itemCount)
        {
            IsChangedData = true;
            Debug.LogError(array[0]);
            Debug.LogError(array[1]);
            inventoryDic[harvestItem[array[0], array[1]]] += itemCount;
        }

        public int GetItemCount(string itemName)
        {
            return inventoryDic[itemName];
        }

        public int GetItemCount(int itemCode)
        {
            if (itemCode >= 0 && itemCode < 9)
            {
                return inventoryDic[harvestItem[itemCode, 0]];
            }

            else if (itemCode >= 9 && itemCode < 15)
            {
                return inventoryDic[harvestItem[itemCode - 9, 1]];
            }
            
            else if (itemCode >= 15 && itemCode < 18)
            {
                return inventoryDic[harvestItem[itemCode - 15, 2]];
            }
            
            else if (itemCode >= 18 && itemCode < 20)
            {
                return inventoryDic[animalItem[0, itemCode - 18]];
            }
            
            else if (itemCode >= 20 && itemCode < 22)
            {
                return inventoryDic[animalItem[1, itemCode - 20]];
            }

            return -1;
        }

        public int GetHarvestItemCount(int itemCode, int more = 0)
        {
            return inventoryDic[harvestItem[itemCode, more]];
        }
        
        public int GetAnimalItemCount(int itemCode, int more = 0)
        {
            return inventoryDic[animalItem[itemCode, more]];
        }
    }
}
