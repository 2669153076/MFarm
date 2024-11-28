using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件中心
/// </summary>
public static class EventHandler
{
    public static event Action<E_InventoryLocation, List<InventoryItem>> UpdateInventoryUIEvent; //更新库存UI事件
    public static event Action<int, Vector3> InstantiateItemInSceneEvent;    //在场景中生成道具
    public static event Action<ItemDetails, bool> ItemSelectedEvent;    //物品被选中

    public static void CallUpdateInventoryUI(E_InventoryLocation e_InventoryLocation,List<InventoryItem> inventoryItemList)
    {
        UpdateInventoryUIEvent?.Invoke(e_InventoryLocation, inventoryItemList);
    }
    public static void CallInstantiateItemInScene(int id,Vector3 pos)
    {
        InstantiateItemInSceneEvent?.Invoke(id, pos);
    }

    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    } 
}
