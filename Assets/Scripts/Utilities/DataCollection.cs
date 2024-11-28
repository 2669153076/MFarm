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
    public bool canPickedUp;    //被拾取
    public bool canDropped;     //被扔掉
    public bool canCarried;     //被举起
    public int itemPrice;
    [Range(0f, 1f)] public float sellPercentage;    //出售物品时的折扣
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

/// <summary>
/// 动画状态机类型
/// </summary>
[System.Serializable]
public class AnimatorType
{
    public E_PartName partName;
    public E_PartType partType;
    public AnimatorOverrideController animatorOverrideController;
}