using Dialogue;
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
    public static event Action<int, Vector3, E_ItemType> DropItemInSceneEvent;    //在场景中扔道具
    public static event Action<ItemDetails, bool> ItemSelectedEvent;    //物品被选中
    public static event Action<int, int, int, E_Season> GameMinuteEvent;   //游戏根据分钟更新UI等事件
    public static event Action<int, int, int, int, E_Season> GameHourEvent; //游戏根据小时更新UI等事件
    public static event Action<int, E_Season> GameDayEvent; //根据每一天更新游戏中的内容事件
    public static event Action<string, Vector3> TransitionEvent;    //触碰目标加载场景事件
    public static event Action BeforeSceneUnloadEvent;  //卸载场景前事件
    public static event Action AfterSceneLoadEvent;   //加载场景后事件
    public static event Action<Vector3> MoveToPositionEvent;    //移动位置事件
    public static event Action<Vector3, ItemDetails> MouseClickedEvent; //鼠标按下事件
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimationEvent; //在动画之后执行的事件
    public static event Action<int, TileDetails> PlantSeedEvent;    //播种事件
    public static event Action<int> HarvestAtPlayerPositionEvent;   //在玩家位置生成农作物
    public static event Action RefreshCurrentMapEvent;   //刷新当前地图 
    public static event Action<E_ParticaleEffectType, Vector3> ParticleEffectEvent;  //生成特效
    public static event Action GenerateCropEvent;   //生成作物，刷新地图前
    public static event Action<DialoguePiece> ShowDialogueEvent;    //显示对话事件 
    public static event Action<E_SlotType, InventoryBag_SO> BaseBagOpenEvent;   //通用背包开启事件
    public static event Action<E_SlotType, InventoryBag_SO> BaseBagCloseEvent;   //通用背包开启事件
    public static event Action <E_GameState> UpdateGameStateEvent;  //更新游戏状态事件
    public static event Action<ItemDetails, bool> ShowTradeUIEvent; //显示交易UI
    public static event Action<int,Vector3> BuildFurnitureEvent;  //建造事件
    public static event Action<E_Season, E_LightShift, float> LightShiftChangeEvent;    //灯光改变事件
    public static event Action<SoundDetails> InitSoundEffectEvent;   //初始化音效事件
    public static event Action<E_SoundName> PlaySoundEvent; //播放音效事件


    /// <summary>
    /// 更新库存UI事件
    /// 背包、商店等等
    /// </summary>
    /// <param name="e_InventoryLocation">库存类型</param>
    /// <param name="inventoryItemList">库存中的道具信息列表</param>
    public static void CallUpdateInventoryUIEvent(E_InventoryLocation e_InventoryLocation, List<InventoryItem> inventoryItemList)
    {
        UpdateInventoryUIEvent?.Invoke(e_InventoryLocation, inventoryItemList);
    }
    /// <summary>
    /// 在场景中生成物品事件
    /// 砍树、割草等等产物在物品坐标随机范围内生成
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="pos">在场景中坐标</param>
    public static void CallInstantiateItemInSceneEvent(int id, Vector3 pos)
    {
        InstantiateItemInSceneEvent?.Invoke(id, pos);
    }
    /// <summary>
    /// 往场景中扔道具事件
    /// </summary>
    /// <param name="itemId">道具id</param>
    /// <param name="mousePos">鼠标坐标</param>
    /// <param name="itemType">道具类型</param>
    public static void CallDropItemInSceneEvent(int itemId, Vector3 mousePos, E_ItemType itemType)
    {
        DropItemInSceneEvent?.Invoke(itemId, mousePos, itemType);
    }
    /// <summary>
    /// 道具被选中事件
    /// </summary>
    /// <param name="itemDetails">道具id</param>
    /// <param name="isSelected">是否被选中</param>
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }
    /// <summary>
    /// 分钟变化事件
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
    /// 小时变化事件
    /// </summary>
    /// <param name="hour">小时</param>
    /// <param name="day">天</param>
    /// <param name="month">月</param>
    /// <param name="year">年</param>
    /// <param name="season">季节</param>
    public static void CallGameHourEvent(int hour, int day, int month, int year, E_Season season)
    {
        GameHourEvent?.Invoke(hour, day, month, year, season);
    }
    /// <summary>
    /// 一天变化事件
    /// </summary>
    /// <param name="day">天</param>
    /// <param name="season">季节</param>
    public static void CallGameDayEvent(int day, E_Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }
    /// <summary>
    /// 传送事件
    /// </summary>
    /// <param name="sceneName">目标场景</param>
    /// <param name="pos">对应传送后的位置</param>
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }
    /// <summary>
    /// 场景卸载前事件
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    /// <summary>
    /// 场景加载后事件
    /// </summary>
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }
    /// <summary>
    /// 移动角色位置事件
    /// 场景加载后调用
    /// </summary>
    /// <param name="pos">目标位置</param>
    public static void CallMoveToPositionEvent(Vector3 pos)
    {
        MoveToPositionEvent?.Invoke(pos);
    }
    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="mousePos">鼠标在场景中的位置</param>
    /// <param name="itemDetails">UI中道具信息</param>
    public static void CallMouseClickedEvent(Vector3 mousePos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(mousePos, itemDetails);
    }
    /// <summary>
    /// 动画完成后事件
    /// 点击背包道具后再点击场景 执行对应的方法
    /// </summary>
    /// <param name="mousePos">鼠标在场景中的位置</param>
    /// <param name="itemDetails">被选择的道具信息</param>
    public static void CallExecuteActionAfterAnimationEvent(Vector3 mousePos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimationEvent?.Invoke(mousePos, itemDetails);
    }
    /// <summary>
    /// 播种事件
    /// </summary>
    /// <param name="id">种子id</param>
    /// <param name="tileDetails">格子信息</param>
    public static void CallPlantSeedEvent(int id, TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(id, tileDetails);
    }
    /// <summary>
    /// 在角色位置收获事件
    /// 即物品直接添加到背包中
    /// </summary>
    /// <param name="itemId">物品id</param>
    public static void CallHarvestAtPlayerPositionEvent(int itemId)
    {
        HarvestAtPlayerPositionEvent?.Invoke(itemId);
    }
    /// <summary>
    /// 刷新当前地图事件
    /// </summary>
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMapEvent?.Invoke();
    }
    /// <summary>
    /// 产生特效事件
    /// </summary>
    /// <param name="type">特效类型</param>
    /// <param name="pos">生成位置</param>
    public static void CallParticleEffectEvent(E_ParticaleEffectType type, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(type, pos);
    }
    /// <summary>
    /// 生成作物事件
    /// 在地图中预先放置一堆作物，初始化时调用
    /// </summary>
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }
    /// <summary>
    /// 显示对话事件
    /// </summary>
    /// <param name="dialoguePiece"></param>
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }
    /// <summary>
    /// 通用背包开启事件
    /// </summary>
    /// <param name="slotType">背包类型</param>
    /// <param name="bag_SO">背包内数据</param>
    public static void CallBaseBagOpenEvent(E_SlotType slotType,InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }
    /// <summary>
    /// 通用背包关闭事件
    /// </summary>
    /// <param name="slotType">背包类型</param>
    /// <param name="bag_SO">背包内数据</param>
    public static void CallBaseBagCloseEvent(E_SlotType slotType,InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }
    /// <summary>
    /// 更新游戏状态事件
    /// </summary>
    /// <param name="gamestate">游戏状态</param>
    public static void CallUpdateGameStateEvent(E_GameState gamestate)
    {
        UpdateGameStateEvent?.Invoke(gamestate);
    }
    /// <summary>
    /// 显示交易UI界面
    /// </summary>
    /// <param name="itemDetails">选中的道具信息</param>
    /// <param name="isSell">是否出售</param>
    public static void CallShowTradeUIEvent(ItemDetails itemDetails,bool isSell)
    {
        ShowTradeUIEvent?.Invoke(itemDetails, isSell);
    }
    /// <summary>
    /// 建造事件
    /// </summary>
    /// <param name="id">图纸物品id</param>
    /// <param name="pos">鼠标对应的场景中的坐标</param>
    public static void CallBuildFurnitureEvent(int id,Vector3 pos)
    {
        BuildFurnitureEvent?.Invoke(id,pos);
    }
    /// <summary>
    /// 灯光改变事件
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">白天还是夜晚</param>
    /// <param name="timeDifferent">时间差</param>
    public static void CallLightShiftChangeEvent(E_Season season,E_LightShift lightShift,float timeDifferent)
    {
        LightShiftChangeEvent?.Invoke(season, lightShift, timeDifferent);
    }
    /// <summary>
    /// 初始化音效事件
    /// </summary>
    /// <param name="soundDetails">音效信息</param>
    public static void CallInitSoundEffectEvent(SoundDetails soundDetails)
    {
        InitSoundEffectEvent?.Invoke(soundDetails);
    }
    /// <summary>
    /// 播放音效事件
    /// </summary>
    /// <param name="soundName">音效名字</param>
    public static void CallPlaySoundEvent(E_SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }
}
