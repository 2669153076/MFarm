using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory
{
    /// <summary>
    /// 道具管理类
    /// </summary>
    public class ItemMgr : Singleton<ItemMgr>
    {
        public Item itemPerfab; //真正的场景中物体预制体
        public Item itemBouncePerfab; //角色扔道具时自由落体物体预制体
        [HideInInspector] public Transform itemParent;

        private Dictionary<string, List<SceneItem>> sceneItemDic = new Dictionary<string, List<SceneItem>>(); //保存场景中的物品列表字典
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDic = new Dictionary<string, List<SceneFurniture>>();    //保存场景中的家具列表字典
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;
        private void OnEnable()
        {
            EventHandler.InstantiateItemInSceneEvent += OnInstantiateItemInScene;
            EventHandler.DropItemInSceneEvent += OnDropItemInSceneEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInSceneEvent -= OnInstantiateItemInScene;
            EventHandler.DropItemInSceneEvent -= OnDropItemInSceneEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
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
                SceneFurniture sceneItem = new SceneFurniture
                {
                    id = item.itemId,
                    pos = new SerializableVector3(item.transform.position)
                };
                curSceneFurnitureList.Add(sceneItem);
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
                        OnBuildFurnitureEvent(sceneFurniture.id,sceneFurniture.pos.ToVector3());
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

        }

    }
}