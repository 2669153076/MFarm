using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save{
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;    //场景名字
        public Dictionary<string, SerializableVector3> characterPosDic; //人物名、人物坐标
        public Dictionary<string,List<SceneItem>> sceneItemDic; //场景名、场景中道具
        public Dictionary<string,List<SceneFurniture>> furnitureDic;    //场景名、场景家具
        public Dictionary<string ,TileDetails> tileDetailsDic;  //场景名、地图网格信息
        public Dictionary<string, bool> firstLoadDic;   //场景名、是否第一次加载
        public Dictionary<string,List<InventoryItem>> inventoryItemDic; //库存名、库存物品信息
        public Dictionary<string, int> timeDic; //时间名（月、日、年...）、时间数值
        public int playerMoney; //角色金钱
        public string targetScene;  //NPC目标场景
        public bool interactable;   //NPC是否可以互动
        public int animationInstanceId; //NPC动画Id
        
    }
}