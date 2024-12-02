using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品信息数据
/// </summary>
[CreateAssetMenu(fileName = "ItemDataList_SO",menuName ="Inventory/ItemDataList")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;
}
