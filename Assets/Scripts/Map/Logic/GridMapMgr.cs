using CropPlant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace GridMap
{
    /// <summary>
    /// 瓦片地图管理器
    /// </summary>
    public class GridMapMgr : Singleton<GridMapMgr>
    {
        [Header("种地瓦片切换信息")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTileMap;
        private Tilemap waterTileMap;
        [Header("地图信息")]
        public List<MapData_SO> mapDataList;

        private Dictionary<string, TileDetails> tileDetailsDic = new Dictionary<string, TileDetails>(); //根据坐标存储对应的格子信息

        private Grid currentGrid;   //当前地图的Grid

        private E_Season currentSeason; //当前季节

        private Dictionary<string,bool> firstLoadDic = new Dictionary<string,bool>();   //场景是否是第一次被加载

        private List<ReapItem> itemsInRadius;   //可收割道具列表

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += OnRefreshCurrentMap;
        }
        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= OnRefreshCurrentMap;
        }


        private void Start()
        {
            foreach (var mapdata in mapDataList)
            {
                firstLoadDic.Add(mapdata.sceneName, true);
                InitTileDetailsDic(mapdata);
            }
        }

        /// <summary>
        /// 获取当前鼠标网格坐标的tile信息
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }

        /// <summary>
        /// 初始化地图信息
        /// </summary>
        /// <param name="mapData"></param>
        private void InitTileDetailsDic(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tilePropertieList)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridPos = new Vector2Int(tileProperty.tileCoordinate.x, tileProperty.tileCoordinate.y)
                };

                //字典的key
                string key = tileDetails.gridPos.x + "x" + tileDetails.gridPos.y + "y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case E_GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case E_GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case E_GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case E_GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                {
                    tileDetailsDic[key] = tileDetails;
                }
                else
                {
                    tileDetailsDic.Add(key, tileDetails);
                }
            }
        }

        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key">x坐标+x+y坐标+y+地图名</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDic.ContainsKey(key))
            {
                return tileDetailsDic[key];
            }
            return null;
        }

        /// <summary>
        /// 设置挖掘地图瓦片
        /// </summary>
        /// <param name="tileDetails">该瓦片信息</param>
        private void SetDigGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridPos.x, tileDetails.gridPos.y, 0);
            if (digTileMap != null)
            {
                digTileMap.SetTile(pos, digTile);
            }
        }
        /// <summary>
        /// 设置浇水地图瓦片
        /// </summary>
        /// <param name="tileDetails">该瓦片信息</param>
        private void SetWaterGround(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridPos.x, tileDetails.gridPos.y, 0);
            if (waterTileMap != null)
            {
                waterTileMap.SetTile(pos, waterTile);
            }
        }

        /// <summary>
        /// 更新tile字典信息
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridPos.x + "x" + tileDetails.gridPos.y + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDic.ContainsKey(key))
            {
                tileDetailsDic[key] = tileDetails;
            }
            else
            {
                tileDetailsDic.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 根据对应场景绘制对应地图
        /// </summary>
        /// <param name="sceneName"></param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDic)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDig > -1)
                    {
                        SetDigGround(tileDetails);
                    }
                    if (tileDetails.daysSinceWatered > -1)
                    {
                        SetWaterGround(tileDetails);
                    }
                    if(tileDetails.seedItemId > -1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemId, tileDetails);
                    }
                }
            }
        }

        /// <summary>
        /// 刷新地图
        /// </summary>
        private void RefreshMap()
        {
            if (digTileMap != null)
            {
                //清除挖坑
                digTileMap.ClearAllTiles();
            }
            if (waterTileMap != null)
            {
                //清除浇水
                waterTileMap.ClearAllTiles();
            }

            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// 通过物理方法判断鼠标点击位置的农作物
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);

            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop = colliders[i].GetComponent<Crop>();
                }
            }
            return currentCrop;
        }

        /// <summary>
        /// 返回工具范围内的杂草
        /// </summary>
        /// <param name="tool">工具信息</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos,ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();

            Collider2D[] colliders = new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)   //范围内有碰撞体
            {
                for (int i = 0;i < colliders.Length;i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())  //碰撞体是可收割物体
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);
                        }
                    }
                }
            }

            return itemsInRadius.Count > 0;
        }
        /// <summary>
        /// 根据场景名字构建网格范围，输出范围和原点
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="gridDimensions">地图范围</param>
        /// <param name="gridOrigin">地图左下角原点</param>
        /// <returns>是否有该场景信息</returns>
        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in mapDataList)
            {
                if(mapData.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;
                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;

                    return true;
                }
            }
            return false;
        }

        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var currentGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(currentGridPos);

            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);
                //WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case E_ItemType.None:
                        break;
                    case E_ItemType.Seed:   //种子
                        EventHandler.CallPlantSeedEvent(itemDetails.itemId,currentTile);
                        EventHandler.CallDropItemInSceneEvent(itemDetails.itemId, mouseWorldPos,itemDetails.itemType);
                        break;
                    case E_ItemType.Commodity:  //商品
                        EventHandler.CallDropItemInSceneEvent(itemDetails.itemId, mouseWorldPos,itemDetails.itemType);
                        break;
                    case E_ItemType.Furniture:  //家具
                        break;
                    case E_ItemType.HoeTool:    //锄头
                        SetDigGround(currentTile);
                        currentTile.daysSinceDig = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        break;
                    case E_ItemType.BreakTool:  //十字镐
                    case E_ItemType.ChopTool:   //斧头
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case E_ItemType.ReapTool:   //镰刀
                        var reapCount = 0;
                        for (int i = 0; i < itemsInRadius.Count; i++)
                        {
                            EventHandler.CallParticleEffectEvent(E_ParticaleEffectType.ReapableScenery, itemsInRadius[i].transform.position + Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();
                            Destroy(itemsInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                            {
                                break;
                            }
                        }
                        break;
                    case E_ItemType.WaterTool:  //水壶
                        SetWaterGround(currentTile);
                        currentTile.daysSinceDig = 0;
                        break;
                    case E_ItemType.CollectTool:    //篮子
                        //Crop currentCrop = GetCropObject(mouseWorldPos);
                        //执行收割方法
                        currentCrop?.ProcessToolAction(itemDetails,currentTile);
                        break;
                    case E_ItemType.ReapableScenery:
                        break;
                }
            }

            UpdateTileDetails(currentTile);
        }
        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTileMap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            //DisplayMap(SceneManager.GetActiveScene().name);
            if (firstLoadDic[SceneManager.GetActiveScene().name])
            {
                EventHandler.CallGenerateCropEvent();   //预先生成农作物
                firstLoadDic[SceneManager.GetActiveScene().name] = false;
            }
            RefreshMap();
        }

        private void OnGameDayEvent(int day, E_Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDic)
            {
                if (tile.Value.daysSinceWatered > -1)  //如果浇了水
                {
                    //每天土地都会干，都可以浇水
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDig > -1)   //如果土地被开垦
                {
                    //增加该土地自从被开垦以来的天数
                    tile.Value.daysSinceDig++;
                }
                if (tile.Value.daysSinceDig > 5 && tile.Value.seedItemId == -1) //如果被开垦且没有农作物
                {
                    //当土地被开垦了5天并且该土地上没有种子，则将土地还原为开垦前的样子
                    tile.Value.daysSinceDig = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if(tile.Value.seedItemId !=-1)      //如果播种了
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }


        private void OnRefreshCurrentMap()
        {
            RefreshMap();
        }

    }
}