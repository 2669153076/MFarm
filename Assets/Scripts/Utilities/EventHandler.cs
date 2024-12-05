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
    public static event Action<int, Vector3,E_ItemType> DropItemInSceneEvent;    //在场景中扔道具
    public static event Action<ItemDetails, bool> ItemSelectedEvent;    //物品被选中
    public static event Action<int, int,int, E_Season> GameMinuteEvent;   //游戏根据分钟更新UI等事件
    public static event Action<int, int, int, int, E_Season> GameDateEvent; //游戏根据日期更新UI等事件
    public static event Action<int, E_Season> GameDayEvent; //根据每一天更新游戏中的内容事件
    public static event Action<string, Vector3> TransitionEvent;    //触碰目标加载场景事件
    public static event Action BeforeSceneUnloadEvent;  //卸载场景前事件
    public static event Action AfterSceneLoadEvent;   //加载场景后事件
    public static event Action<Vector3> MoveToPositionEvent;    //移动位置事件
    public static event Action<Vector3, ItemDetails> MouseClickedEvent; //鼠标按下事件
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation; //在动画之后执行的事件
    public static event Action<int, TileDetails> PlantSeedEvent;    //播种事件
    public static event Action<int> HarvestAtPlayerPositionEvent;   //在玩家位置生成农作物
    public static event Action RefreshCurrentMap;   //刷新当前地图 
    public static event Action<E_ParticaleEffectType,Vector3> ParticleEffectEvent;  //生成特效
    public static event Action GenerateCropEvent;   //生成作物，刷新地图前

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e_InventoryLocation"></param>
    /// <param name="inventoryItemList"></param>
    public static void CallUpdateInventoryUIEvent(E_InventoryLocation e_InventoryLocation, List<InventoryItem> inventoryItemList)
    {
        UpdateInventoryUIEvent?.Invoke(e_InventoryLocation, inventoryItemList);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    public static void CallInstantiateItemInSceneEvent(int id, Vector3 pos)
    {
        InstantiateItemInSceneEvent?.Invoke(id, pos);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <param name="itemType"></param>
    public static void CallDropItemInSceneEvent(int id,Vector3 pos,E_ItemType itemType)
    {
        DropItemInSceneEvent?.Invoke(id,pos,itemType);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minute">分钟</param>
    /// <param name="hour">小时</param>
    /// <param name="day">天</param>
    /// <param name="season">季节</param>
    public static void CallGameMinuteEvent(int minute, int hour, int day, E_Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="day"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <param name="season"></param>
    public static void CallGameDateEvent(int hour, int day, int month, int year, E_Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    /// <param name="season"></param>
    public static void CallGameDayEvent(int day,E_Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="pos"></param>
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }
    /// <summary>
    /// 
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    /// <summary>
    /// 
    /// </summary>
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    public static void CallMoveToPositionEvent(Vector3 pos)
    {
        MoveToPositionEvent?.Invoke(pos);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePos"></param>
    /// <param name="itemDetails"></param>
    public static void CallMouseClickedEvent(Vector3 mousePos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(mousePos, itemDetails);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePos"></param>
    /// <param name="itemDetails"></param>
    public static void CallExecuteActionAfterAnimation(Vector3 mousePos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(mousePos, itemDetails);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tileDetails"></param>
    public static void CallPlantSeedEvent(int id, TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(id, tileDetails);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    public static void CallHarvestAtPlayerPositionEvent(int itemId)
    {
        HarvestAtPlayerPositionEvent?.Invoke(itemId);
    }
    /// <summary>
    /// 
    /// </summary>
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pos"></param>
    public static void CallParticleEffectEvent(E_ParticaleEffectType type,Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(type, pos);
    }
    /// <summary>
    /// 
    /// </summary>
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }
}
