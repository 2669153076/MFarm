using MFarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    /// <summary>
    /// 库存管理类
    /// </summary>
    public class InventoryMgr : Singleton<InventoryMgr>, ISaveable
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag_SO;
        [Header("建造蓝图")]
        public BlueprintDataList_SO blueprintDataList_SO;
        [Header("箱子")]
        private InventoryBag_SO currentBoxBag_SO;

        public InventoryBag_SO playerBagTemp;

        private Dictionary<string,List<InventoryItem>> boxDataDic = new Dictionary<string,List<InventoryItem>>();   //对应场景中所有箱子数据
        public int BoxDataDicAmount => boxDataDic.Count;

        public string GUID => GetComponent<DataGUID>().guid;

        public int playerMoney; //角色持有金钱

        private void OnEnable()
        {
            EventHandler.DropItemInSceneEvent += OnDropItemInSceneEvent;
            EventHandler.HarvestAtPlayerPositionEvent += OnHarvestAtPlayerPositionEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }
        private void OnDisable()
        {
            EventHandler.DropItemInSceneEvent -= OnDropItemInSceneEvent;
            EventHandler.HarvestAtPlayerPositionEvent -= OnHarvestAtPlayerPositionEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
            //EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);
        }

        /// <summary>
        /// 通过id获取道具信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int id)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemId == id);
        }

        /// <summary>
        /// 往背包中添加物品
        /// </summary>
        /// <param name="item">道具</param>
        /// <param name="toDestory">是否删除对应道具实体</param>
        public void AddItem(Item item, bool toDestory = true,int amount = 1)
        {
            var index = GetItemIndexInBag(item.itemId);
            AddItemByIndex(item.itemId, index, amount);

            if (toDestory)
            {
                Destroy(item.gameObject);

            }

            //更新UI
            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);
        }
        /// <summary>
        /// 交换物品位置
        /// </summary>
        /// <param name="fromIndex">当前格子</param>
        /// <param name="targetIndex">目标格子</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag_SO.inventoryItemList[fromIndex];
            InventoryItem targetItem = playerBag_SO.inventoryItemList[targetIndex];
            if (targetItem.itemId != 0)
            {
                playerBag_SO.inventoryItemList[fromIndex] = targetItem;
                playerBag_SO.inventoryItemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag_SO.inventoryItemList[targetIndex] = currentItem;
                playerBag_SO.inventoryItemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);
        }
        /// <summary>
        /// 跨容器交换物品
        /// </summary>
        /// <param name="locationFrom">来自于背包类型</param>
        /// <param name="fromIndex">来自于索引</param>
        /// <param name="locationTarget">目标背包类型</param>
        /// <param name="targetIndex">目标索引</param>
        public void SwapItem(E_InventoryLocation locationFrom,int fromIndex, E_InventoryLocation locationTarget,int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if(targetItem.itemId!=0&&currentItem.itemId!=targetItem.itemId) //有不相同的两个物品
                {
                    //交换
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }else if(currentItem.itemId == 0)   //相同的两个物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else //目标格子为空
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }

                EventHandler.CallUpdateInventoryUIEvent(locationFrom,currentList);
                EventHandler.CallUpdateInventoryUIEvent(locationTarget,targetList);
            }
        }
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns>
        /// true 有空位<br/>
        /// false 无空位
        /// </returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag_SO.inventoryItemList.Count; i++)
            {
                if (playerBag_SO.inventoryItemList[i].itemId == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取物品在背包中的位置
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// -1 背包中没有该物品<br/>
        /// i 物品在背包中的索引
        /// </returns>
        private int GetItemIndexInBag(int id)
        {
            for (int i = 0; i < playerBag_SO.inventoryItemList.Count; i++)
            {
                if (playerBag_SO.inventoryItemList[i].itemId == id)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 通过索引往背包中添加物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="index">物品索引</param>
        /// <param name="amount">物品数量</param>
        private void AddItemByIndex(int id, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())    //背包中无该物品 && 背包有空位
            {
                var item = new InventoryItem { itemId = id, itemAmount = amount };
                for (int i = 0; i < playerBag_SO.inventoryItemList.Count; i++)
                {
                    if (playerBag_SO.inventoryItemList[i].itemId == 0)
                    {
                        playerBag_SO.inventoryItemList[i] = item;
                        break;
                    }
                }
            }
            else if (index != -1)    //背包中有该物品
            {
                int currentAmount = playerBag_SO.inventoryItemList[index].itemAmount + amount;
                var item = new InventoryItem { itemId = id, itemAmount = currentAmount };
                playerBag_SO.inventoryItemList[index] = item;
            }
        }
        /// <summary>
        /// 通过Id移除背包内物品
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="amount">要移除的数量</param>
        private void RemoveItemById(int id,int amount)
        {
            var index = GetItemIndexInBag(id);

            if (playerBag_SO.inventoryItemList[index].itemAmount > amount)
            {
                var newAmount = playerBag_SO.inventoryItemList[index].itemAmount - amount;
                var item = new InventoryItem { itemId=id, itemAmount = newAmount };
                playerBag_SO.inventoryItemList[index] = item;

            }
            else if(playerBag_SO.inventoryItemList[index].itemAmount == amount)
            {
                var item = new InventoryItem();
                playerBag_SO.inventoryItemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);
        }
        /// <summary>
        /// 交易
        /// </summary>
        /// <param name="item">物品信息</param>
        /// <param name="amount">数量</param>
        /// <param name="isSell">是否出售</param>
        public void TradeItem(ItemDetails item,int amount,bool isSell)
        {
            int cost = item.itemPrice * amount;
            int index = GetItemIndexInBag(item.itemId);
            if (isSell)
            {
                if (playerBag_SO.inventoryItemList[index].itemAmount>=amount)
                {
                    RemoveItemById(item.itemId, amount);
                   cost =  (int)(cost*item.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney-cost>=0)
            {
                if (CheckBagCapacity())
                {
                    AddItemByIndex(item.itemId, index, amount);
                    playerMoney -= cost;
                }
            }

            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);

        }
        /// <summary>
        /// 检查背包中是否有对应数量的道具
        /// </summary>
        /// <param name="id">图纸id</param>
        /// <returns></returns>
        public bool CheckStock(int id)
        {
            var blueprintDetails = blueprintDataList_SO.GetBlueprintDetails(id);
            foreach (var resourceItem in blueprintDetails.resourceItem)
            {
                var itemStock = playerBag_SO.GetInventoryItem(resourceItem.itemId);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 根据位置类型获取数据列表
        /// </summary>
        /// <param name="location">背包类型</param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(E_InventoryLocation location)
        {
            return location switch
            {
                E_InventoryLocation.Player=>playerBag_SO.inventoryItemList,
                E_InventoryLocation.Box=>currentBoxBag_SO.inventoryItemList,
                _=>null,
            };
        }
        /// <summary>
        /// 查找对应箱子数据
        /// </summary>
        /// <param name="key">名字+索引</param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDic.ContainsKey(key))
            {
                return boxDataDic[key];
            }
            return null;
        }
        /// <summary>
        /// 往字典中添加箱子数据
        /// </summary>
        /// <param name="box">箱子数据</param>
        public void AddBoxDataDic(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDic.ContainsKey(key))
            {
                boxDataDic.Add(key, box.boxBagData.inventoryItemList);
            }
        }


        private void OnDropItemInSceneEvent(int id, Vector3 pos, E_ItemType itemType)
        {
            RemoveItemById(id, 1);
        }
        private void OnHarvestAtPlayerPositionEvent(int itemId)
        {
            var index = GetItemIndexInBag(itemId);  //获取物品在背包中的索引
            AddItemByIndex(itemId, index, 1);   //添加物品到背包中

            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);
        }
        private void OnBuildFurnitureEvent(int itemId, Vector3 mouseWorldPos)
        {
            RemoveItemById(itemId, 1);
            var blueprint = InventoryMgr.Instance.blueprintDataList_SO.GetBlueprintDetails(itemId);
            foreach (var item in blueprint.resourceItem)
            {
                RemoveItemById(item.itemId, item.itemAmount);
            }
        }
        private void OnBaseBagOpenEvent(E_SlotType type, InventoryBag_SO boxData)
        {
            currentBoxBag_SO = boxData;
        }
        private void OnStartNewGameEvent(int obj)
        {
            this.playerBag_SO = Instantiate(this.playerBagTemp);
            this.playerMoney = Settings.playerStartMoney;
            this.boxDataDic.Clear();
            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, this.playerBag_SO.inventoryItemList);
        }


        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = playerMoney;

            saveData.inventoryItemDic = new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryItemDic.Add(playerBag_SO.name, playerBag_SO.inventoryItemList);
            foreach (var item in boxDataDic)
            {
                saveData.inventoryItemDic.Add(item.Key, item.Value);
            }
            return saveData;
        }

        public void RestoreData(GameSaveData data)
        {
            this.playerMoney = data.playerMoney;
            this.playerBag_SO = Instantiate(this.playerBagTemp);
            this.playerBag_SO.inventoryItemList = data.inventoryItemDic[playerBag_SO.name];
            foreach (var item in data.inventoryItemDic)
            {
                if (this.boxDataDic.ContainsKey(item.Key))
                {
                    this.boxDataDic[item.Key] = item.Value;
                }
            }
            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, this.playerBag_SO.inventoryItemList);
        }
    }
}

