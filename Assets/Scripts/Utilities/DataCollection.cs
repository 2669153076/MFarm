using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品信息类
/// </summary>
[System.Serializable]
public class ItemDetails
{
    public int itemId;
    public string itemName;
    public E_ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDesctription;
    public int itemUseRadius;
    public bool canPickedUp;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0f, 1f)] public float sellPercentage;
}

/// <summary>
/// 背包道具结构体
/// </summary>
[System.Serializable]
public struct InventoryItem
{
    public int itemId;
    public int itemAmount;
}