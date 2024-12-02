using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// 库存管理类
    /// </summary>
    public class InventoryMgr : Singleton<InventoryMgr>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag_SO;

        private void OnEnable()
        {
            EventHandler.DropItemInSceneEvent += OnDropItemInSceneEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemInSceneEvent -= OnDropItemInSceneEvent;
        }
        private void Start()
        {
            EventHandler.CallUpdateInventoryUIEvent(E_InventoryLocation.Player, playerBag_SO.inventoryItemList);
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
        /// <param name="toDestory">是否删除</param>
        public void AddItem(Item item, bool toDestory = true)
        {
            var index = GetItemIndexInBag(item.itemId);
            AddItemByIndex(item.itemId, index, 1);

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
        /// 检查背包是否有空位
        /// </summary>
        /// <returns>
        /// true 有空位</br>
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
        /// -1 背包中没有该物品</br>
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
        /// 移除背包内物品
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <param name="amount">要移除的数量</param>
        private void RemoveItemByIndex(int id,int amount)
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


        private void OnDropItemInSceneEvent(int id, Vector3 pos)
        {
            RemoveItemByIndex(id, 1);
        }
    }
}

