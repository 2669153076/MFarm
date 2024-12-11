using MFarm.GridMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant{
    /// <summary>
    /// 作物生成管理器
    /// 初始化场景使用，在编辑模式下放置作物
    /// 挂载在作物身上
    /// </summary>
    public class CropGenerator : MonoBehaviour
    {
        private Grid curGrid;
        public int seedItemId;  //种子id
        public int growthDays;  //已经长了几天

        private void Awake()
        {
            curGrid = FindObjectOfType<Grid>();
        }
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += OnGenerateCropEvent;
        }
        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= OnGenerateCropEvent;
        }

        /// <summary>
        /// 生成作物
        /// </summary>
        private void GenerateCrop()
        {
            Vector3Int cropGridPos =curGrid.WorldToCell(transform.position);

            if(seedItemId != 0 )
            {
                var tile = GridMapMgr.Instance.GetTileDetailsOnMousePosition(cropGridPos);
                if(tile == null )
                {
                    tile = new TileDetails();
                    tile.gridPos.x = cropGridPos.x;
                    tile.gridPos.y = cropGridPos.y;
                }
                tile.daysSinceWatered = -1;
                tile.seedItemId = seedItemId;
                tile.growthDays = growthDays;

                GridMapMgr.Instance.UpdateTileDetails(tile);
            }
        }

        
        private void OnGenerateCropEvent()
        {
            GenerateCrop();
        }
    }
}