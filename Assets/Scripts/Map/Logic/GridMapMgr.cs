using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace GridMap{
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

        private Dictionary<string,TileDetails> tileDetailsDic = new Dictionary<string,TileDetails>(); //根据坐标存储对应的格子信息

        private Grid currentGrid;   //当前地图的Grid



        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }
        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }
        private void Start()
        {
            foreach (var mapdata in mapDataList)
            {
                InitTileDetailsDic(mapdata);
            }
        }

        /// <summary>
        /// 获取当前鼠标网格坐标的tile信息
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x+"x"+mouseGridPos.y+"y"+SceneManager.GetActiveScene().name;
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

                if(GetTileDetails(key) != null )
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

                if(GetTileDetails(key)!=null )
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
        private TileDetails GetTileDetails(string key)
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
            Debug.Log(digTileMap);
            if(digTileMap!=null)
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
            if(waterTileMap!=null)
            {
                waterTileMap.SetTile(pos, waterTile);
            }
        }


        private void OnExecuteActionAfterAnimation(Vector3 mousePos, ItemDetails itemDetails)
        {
            var currentGridPos = currentGrid.WorldToCell(mousePos);
            var currentTile = GetTileDetailsOnMousePosition(currentGridPos);

            if(currentTile != null)
            {
                //WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case E_ItemType.None:
                        break;
                    case E_ItemType.Seed:
                        break;
                    case E_ItemType.Commodity:
                        EventHandler.CallDropItemInSceneEvent(itemDetails.itemId, mousePos);
                        break;
                    case E_ItemType.Furniture:
                        break;
                    case E_ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDig = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        break;
                    case E_ItemType.ChopTool:
                        break;
                    case E_ItemType.BreakTool:
                        break;
                    case E_ItemType.ReapTool:
                        break;
                    case E_ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceDig = 0;
                        break;
                    case E_ItemType.CollectTool:
                        break;
                    case E_ItemType.ReapableScenery:
                        break;
                }
            }
        }
        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTileMap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
        }


    }
}