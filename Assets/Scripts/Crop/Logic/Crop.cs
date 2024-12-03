using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CropPlant{
    public class Crop : MonoBehaviour
    {
        public CropDetails cropDetails;

        private int harvestActionCount; //点击计数器

        public void ProcessToolAction(ItemDetails tool)
        {
            int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemId);
            if (requireActionCount==-1)
            {
                return;
            }

            //判断是否有动画 树木

            //点击计数器
            if (harvestActionCount < requireActionCount)
            {
                harvestActionCount++;

                //播放粒子

                //播放声音
            }
            if(harvestActionCount>=requireActionCount)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    //生成农作物
                    SpawnHarvestItems();
                }
            }

        }

        /// <summary>
        /// 生成收获农作物
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0;i<cropDetails.producedItemId.Length;i++)
            {
                int amountToProduce;    //产量

                if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
                {
                    //固定产量
                    amountToProduce = cropDetails.producedMinAmount[i];
                }
                else
                {
                    //随机产量
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
                }

                for (int j = 0;j<amountToProduce;j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHarvestAtPlayerPositionEvent(cropDetails.producedItemId[i]);
                    }
                }
            }

           
        }
    }
}
