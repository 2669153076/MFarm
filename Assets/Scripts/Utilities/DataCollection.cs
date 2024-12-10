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

/// <summary>
/// 可序列化的Vector3
/// </summary>
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

/// <summary>
/// 场景中的物品
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int id;
    public SerializableVector3 pos;
}

/// <summary>
/// 地图Tile属性
/// </summary>
[System.Serializable]
public class TileProperty
{   
    /// <summary>
    /// 瓦片坐标
    /// </summary>
    public Vector2Int tileCoordinate;
    /// <summary>
    /// 瓦片类型
    /// </summary>
    public E_GridType gridType;
    /// <summary>
    /// 网格信息值是否改变
    /// 能否挖掘、能否扔道具等等根据该值变化
    /// </summary>
    public bool boolTypeValue;
}

/// <summary>
/// 瓦片格子内的信息
/// </summary>
[System.Serializable]
public class TileDetails 
{
    public Vector2Int gridPos;
    public bool canDig; //能否挖掘
    public bool canDropItem;    //能否扔道具
    public bool canPlaceFurniture;  //能否放家具
    public bool isNPCObstacle;  //是否是NPC
    public int daysSinceDig = -1;   //自挖掘以来的天数
    public int daysSinceWatered = -1;   //自浇水以来的天数
    public int seedItemId = -1; //种下的种子id
    public int growthDays = -1; //种子成长了多少天
    public int daysSinceLastHarvest = -1;   //距离上一次收割过了多少天
}
/// <summary>
/// NPC坐标
/// </summary>
[System.Serializable]
public class NPCPosition
{
    public Transform npc;
    public string startScene;
    public Vector3 pos;
}
/// <summary>
/// 场景路径
/// 从入口到目标点
/// </summary>
[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int toGridCell;
}
/// <summary>
/// 场景路线
/// 从上一个场景到目标场景
/// </summary>
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string toSceneName;
    public List<ScenePath> scenePathList;   //上一个场景到目标场景要经过的路线
}

/// <summary>
/// 图纸信息
/// </summary>
[System.Serializable]
public class BlueprintDetails
{
    public int id;
    public InventoryItem[] resourceItem; //需要的资源
    public GameObject buildPrefab;  //建造的物品的预制体
}
/// <summary>
/// 场景中家具
/// </summary>
[System.Serializable]
public class SceneFurniture
{
    public int id;
    public SerializableVector3 pos;
    public int boxIndex;
}
/// <summary>
/// 灯光信息
/// </summary>
[System.Serializable]
public class LightDetails
{
    public E_Season season; //季节
    public E_LightShift lightShift; //什么时辰
    public Color lightColor;    //光颜色
    public float lightIntensity;   //光强度
}

/// <summary>
/// 音效信息
/// </summary>
[System.Serializable]
public class SoundDetails
{
    public E_SoundName soundName;
    public AudioClip soundClip;
    [Range(0.1f, 1.5f)]
    public float soundPitchMin;  //音调
    [Range(0.1f, 1.5f)]
    public float soundPitchMax;  //音调
    [Range(0.1f, 1f)]
    public float soundVolume;    //音量
}
/// <summary>
/// 场景音乐
/// </summary>
[System.Serializable]
public class SceneSoundItem
{
    public string sceneName;
    public E_SoundName ambient; //音效
    public E_SoundName music;   //背景音乐

}
