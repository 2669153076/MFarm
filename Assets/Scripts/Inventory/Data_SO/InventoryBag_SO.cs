using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包数据
/// </summary>
[CreateAssetMenu(fileName ="InventoryBag_SO",menuName ="Inventory/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> inventoryItemList;

    public InventoryItem GetInventoryItem(int id)
    {
        return inventoryItemList.Find(i=>i.itemId == id);
    }
}
