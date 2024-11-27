using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具类型
/// </summary>
public enum E_ItemType
{
    None,
    //种子、商品、家具、
    Seed, Commodity, Furniture,
    //锄头工具、砍伐工具、破坏工具、收割工具、浇水工具、收集工具、
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool,
    //可收割风景、
    ReapableScenery,
}

/// <summary>
/// 格子类型
/// </summary>
public enum E_SlotType
{
    None,
    Bag,Box,Shop
}

/// <summary>
/// 库存位置
/// </summary>
public enum E_InventoryLocation
{
    None,
    Player,Box
}