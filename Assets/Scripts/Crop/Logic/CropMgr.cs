using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CropPlant
{
    public class CropMgr : Singleton<CropMgr>
    {
        public CropDataList_SO cropData;    //作物数据库

        private Transform cropParent;   //作物父类，作物都会生成在该物体下
        private Grid currentGrid;   //当前Grid，用于获取地图信息
        private E_Season currentSeason; //当前季节

        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }


        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }

        /// <summary>
        /// 获取作物种子信息
        /// </summary>
        /// <param name="id">种子id</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int id)
        {
            return cropData.cropDetailsList.Find(i=>i.seedItemId == id);
        }
        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="cropDetails">作物种子信息</param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails cropDetails)
        {
            
            for (int i = 0;i<cropDetails.seasons.Length;i++)
            {
                if (cropDetails.seasons[i] == currentSeason)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnPlantSeedEvent(int itemId, TileDetails tileDetails)
        {
            CropDetails currentCropDetails = GetCropDetails(itemId);

            if(currentCropDetails!=null&&SeasonAvailable(currentCropDetails)&&tileDetails.seedItemId == -1) //当前作物信息不为空&&季节合适&&格子中没有作物种子
            {
                //播种
                //格子中作物id为该作物种子id
                //成长天数为0
                tileDetails.seedItemId = itemId;
                tileDetails.growthDays = 0;
                //显示农作物
                DisplayCropPlant(tileDetails, currentCropDetails);
            }
            else if(tileDetails.seedItemId !=-1)   //如果当前格子中有种子，刷新地图
            {
                //显示农作物
                DisplayCropPlant(tileDetails, currentCropDetails);
            }
        }
        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails">瓦片信息</param>
        /// <param name="cropDetails">作物信息</param>
        private void DisplayCropPlant(TileDetails tileDetails,CropDetails cropDetails)
        {
            int growthStages =cropDetails.growthDays.Length;    //成长阶段
            int currentStage = 0;   //当前阶段
            int dayCounter = cropDetails.TotalGrowDays; //总成长天数
              
            //倒序计算当前成长阶段
            for (int i = growthStages-1;i>=0;i--)
            {
                if (tileDetails.growthDays >= dayCounter)   //距播种日期天数大于等于总成长天数
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];    //总成长天数减去当前阶段所需成长天数
            }

            //获取当前阶段Prefabs
            GameObject cropPrefabs = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            Vector3 pos = new Vector3(tileDetails.gridPos.x + 0.5f, tileDetails.gridPos.y + 0.5f, 0);
            GameObject cropInstance = Instantiate(cropPrefabs,pos,Quaternion.identity,cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
        }



        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }
        private void OnGameDayEvent(int day, E_Season season)
        {
            currentSeason = season;
        }
    }
}