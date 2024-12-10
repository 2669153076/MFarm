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
    //锄头、斧头、十字镐、镰刀、水壶、篮子、
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool,
    //可收割风景（环境物品）、
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
/// 库存位置类型
/// </summary>
public enum E_InventoryLocation
{
    None,
    //玩家（即背包），box
    Player,Box,Shop,
}

/// <summary>
/// 做的动作类型
/// </summary>
public enum E_PartType
{
    None,
    //举起、锄地、破坏
    Carry,Hoe,Break,Water,Collect,Chop,Reap,
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

/// <summary>
/// 季节
/// </summary>
public enum E_Season
{ 
    None,
    Spring, Summer, Autumn, Winter
}

/// <summary>
/// grid瓦片类型
/// </summary>
public enum E_GridType { 
    None,
    //挖坑、扔东西、添家具、NPC障碍
    Diggable,DropItem,PlaceFurniture,NPCObstacle
}
/// <summary>
/// 特效类型
/// </summary>
public enum E_ParticaleEffectType
{
    None,
    //落叶1、落叶2、石头、庄稼
    LeavesFalling01, LeavesFalling02,Rock,ReapableScenery,
}

/// <summary>
/// 游戏状态
/// </summary>
public enum E_GameState
{
    None,
    Playing, Pause
}
/// <summary>
/// 什么时候的灯光
/// </summary>
public enum E_LightShift
{
    None,
    //早、晚
    Morning,Night,
}
/// <summary>
/// 音效名字
/// </summary>
public enum E_SoundName
{
    None,
    //轻脚步、重脚步
    FootStepSoft,FootStepHard,
    //斧头、十字镐、锄头、镰刀、水壶、篮子
    Axe,Pickaxe,Hoe,Reap,Water,Basket,
    //砍树、捡起、种植、树倒下、穿过杂草
    Chop,Pickup,Plant,TreeFalling,Rustle,
    //乡村1、乡村2、平静BGM1、平静BGM2、平静BGM3、室内1
    AmbientCountryside1, AmbientCountryside2,MusicCalm1,MusicCalm2,MusicCalm3,AmbientIndoor1,
}


