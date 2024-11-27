using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件中心
/// </summary>
public static class EventHandler
{
    public static event Action<E_InventoryLocation, List<InventoryItem>> UpdateInventoryUI; //更新库存UI事件
    public static event Action<int, Vector3> InstantiateItemInScene;    //在场景中生成道具

    public static void CallUpdateInventoryUI(E_InventoryLocation e_InventoryLocation,List<InventoryItem> inventoryItemList)
    {
        UpdateInventoryUI?.Invoke(e_InventoryLocation, inventoryItemList);
    }
    public static void CallInstantiateItemInScene(int id,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(id, pos);
    }
}
