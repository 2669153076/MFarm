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
    //背包、box、商店
    Bag,Box,Shop
}

/// <summary>
/// 库存位置
/// </summary>
public enum E_InventoryLocation
{
    None,
    //玩家，box
    Player,Box
}

/// <summary>
/// 做的动作类型
/// </summary>
public enum E_PartType
{
    None,
    //举起、锄地、破坏
    Carry,Hoe,Break
}
/// <summary>
/// 实现动作的部分
/// </summary>
public enum E_PartName
{
    None,
    //身体、头发、胳膊、工具
    Body,Hair,Arm,Tool
}

//季节
public enum E_Season
{ 
    None,
    Spring, Summer, Autumn, Winter
}
