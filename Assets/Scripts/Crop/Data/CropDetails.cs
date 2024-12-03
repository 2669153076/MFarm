using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 作物信息
/// </summary>
[System.Serializable]
public class CropDetails 
{
    [Header("种子Id")]
    public int seedItemId;
    [Header("不同阶段需要的天数")]
    public int[] growthDays;
    public int TotalGrowDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }   //总生长天数
    [Header("不同生长阶段物品的Prefabs")]
    public GameObject[] growthPrefabs;  //预制体
    public Sprite[] growthSprites;      //精灵图片
    [Header("可种植的季节")]
    public E_Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemId;
    [Header("每种工具使用的次数")]
    public int[] requireActionCount;    //不同工具砍伐同一物品所需要使用的次数
    [Header("收割后转换为新物品的Id")]
    public int transferItemId;

    [Space]
    [Header("收割的果实信息")]
    public int[] producedItemId;    //产出的物品Id
    public int[] producedMinAmount; //最低产量
    public int[] producedMaxAmount; //最高产量
    public Vector2 spawnRadius; //产物掉落出的半径
    [Header("再次生长时间")]
    public int daysToRegrow;    //再生天数
    public int regrowTimes;     //还能再生得次数
    [Header("Options")]
    public bool generateAtPlayerPosition;   //在player身上生成
    public bool hasAnimation;   //有没有动画
    public bool hasParticalEffect;  //有没有粒子特效


    /// <summary>
    /// 工具是否可用
    /// </summary>
    /// <param name="toolId">工具ID</param>
    /// <returns>
    /// true 可用 </br>
    /// false 不可用
    /// </returns>
    public bool CheckToolAvailable(int toolId)
    {
        foreach (var tool in harvestToolItemId)
        {
            if(tool == toolId)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取对应工具需要请求的次数
    /// </summary>
    /// <param name="toolId">工具Id</param>
    /// <returns>对应工具需要请求的次数</returns>
    public int GetTotalRequireCount(int toolId)
    {
        for (int i = 0; i < harvestToolItemId.Length; i++)
        {
            if (harvestToolItemId[i] == toolId)
            {
                return requireActionCount[i];
            }
        }
        return -1;
    }
}
