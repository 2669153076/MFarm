using MFarm.Inventory;
using MFarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

namespace MFarm.Inventory
{
    /// <summary>
    /// 道具管理类
    /// </summary>
    public class ItemMgr : Singleton<ItemMgr>, ISaveable
    {
        public Item itemPerfab; //真正的场景中物体预制体
        public Item itemBouncePerfab; //角色扔道具时自由落体物体预制体
        [HideInInspector] public Transform itemParent;

        private Dictionary<string, List<SceneItem>> sceneItemDic = new Dictionary<string, List<SceneItem>>(); //保存场景中的物品列表字典
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDic = new Dictionary<string, List<SceneFurniture>>();    //保存场景中的家具列表字典
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.InstantiateItemInSceneEvent += OnInstantiateItemInScene;
            EventHandler.DropItemInSceneEvent += OnDropItemInSceneEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInSceneEvent -= OnInstantiateItemInScene;
            EventHandler.DropItemInSceneEvent -= OnDropItemInSceneEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        /// <summary>
        /// 获取当前激活场景中的道具
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> curSceneItemList = new List<SceneItem>();
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    id = item.itemId,
                    pos = new SerializableVector3(item.transform.position)
                };
                curSceneItemList.Add(sceneItem);
            }

            if (sceneItemDic.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneItemDic[SceneManager.GetActiveScene().name] = curSceneItemList;
            }
            else
            {
                sceneItemDic.Add(SceneManager.GetActiveScene().name, curSceneItemList);
            }

        }

        /// <summary>
        /// 重新生成场景中的道具
        /// </summary>
        private void ReCreateAllItems()
        {
            List<SceneItem> curSceneItemList = new List<SceneItem>();

            if (sceneItemDic.TryGetValue(SceneManager.GetActiveScene().name, out curSceneItemList))
            {
                if (curSceneItemList != null)
                {
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in curSceneItemList)
                    {
                        Item newItem = Instantiate(itemPerfab, item.pos.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.id);
                    }
                }
            }
        }
        /// <summary>
        /// 获取当前激活场景中的所有家具
        /// </summary>
        private void GetAllSceneFurnitures()
        {
            List<SceneFurniture> curSceneFurnitureList = new List<SceneFurniture>();
            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    id = item.itemId,
                    pos = new SerializableVector3(item.transform.position)
                };
                if (item.GetComponent<Box>())
                {
                    sceneFurniture.boxIndex = item.GetComponent<Box>().index;
                }
                curSceneFurnitureList.Add(sceneFurniture);
            }

            if (sceneFurnitureDic.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneFurnitureDic[SceneManager.GetActiveScene().name] = curSceneFurnitureList;
            }
            else
            {
                sceneFurnitureDic.Add(SceneManager.GetActiveScene().name, curSceneFurnitureList);
            }
        }
        /// <summary>
        /// 重新生成场景中的家具
        /// </summary>
        private void ReBuildFurnitures()
        {
            List<SceneFurniture> curSceneFurnitureList = new List<SceneFurniture>();

            if (sceneFurnitureDic.TryGetValue(SceneManager.GetActiveScene().name, out curSceneFurnitureList))
            {
                if (curSceneFurnitureList != null)
                {
                    foreach (var sceneFurniture in curSceneFurnitureList)
                    {
                        var blueprint = InventoryMgr.Instance.blueprintDataList_SO.GetBlueprintDetails(sceneFurniture.id);
                        var buildItem = Instantiate(blueprint.buildPrefab, sceneFurniture.pos.ToVector3(), Quaternion.identity, itemParent);
                        if (buildItem.GetComponent<Box>())
                        {
                            buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }

        private void OnInstantiateItemInScene(int id, Vector3 pos)
        {
            var item = Instantiate(itemBouncePerfab, pos, Quaternion.identity, itemParent);
            item.itemId = id;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
        }
        private void OnDropItemInSceneEvent(int id, Vector3 mousePos, E_ItemType itemType)
        {
            if (itemType == E_ItemType.Seed)
            {
                return;
            }
            var item = Instantiate(itemBouncePerfab, PlayerTransform.position, Quaternion.identity, itemParent);
            item.itemId = id;
            var dir = (mousePos - PlayerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }
        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFurnitures();
        }
        private void OnAfterSceneLoadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            ReCreateAllItems();
            ReBuildFurnitures();
        }
        private void OnBuildFurnitureEvent(int itemId, Vector3 mouseWorldPos)
        {
            var blueprint = InventoryMgr.Instance.blueprintDataList_SO.GetBlueprintDetails(itemId);
            var buildItem =  Instantiate(blueprint.buildPrefab,mouseWorldPos, Quaternion.identity, itemParent);
            if(buildItem.GetComponent<Box>())
            {
                //buildItem.GetComponent<Box>().index = InventoryMgr.Instance.BoxDataDicAmount;
                buildItem.GetComponent<Box>().InitBox(InventoryMgr.Instance.BoxDataDicAmount);
            }
        }
        private void OnStartNewGameEvent(int obj)
        {
            this.sceneItemDic.Clear();
            this.sceneFurnitureDic.Clear();
        }


        public GameSaveData GenerateSaveData()
        {
            GetAllSceneItems();
            GetAllSceneFurnitures();
            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDic = this.sceneItemDic;
            saveData.furnitureDic = this.sceneFurnitureDic;

            return saveData;

        }

        public void RestoreData(GameSaveData data)
        {
            this.sceneItemDic = data.sceneItemDic;
            this.sceneFurnitureDic= data.furnitureDic;

            ReCreateAllItems();
            ReBuildFurnitures();
        }
    }
}