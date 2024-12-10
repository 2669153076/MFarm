using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CropPlant{
    /// <summary>
    /// 收货
    /// </summary>
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;


        /// <summary>
        /// 初始化农作物信息
        /// </summary>
        /// <param name="id"></param>
        public void InitCropData(int id)
        {
            cropDetails = CropMgr.Instance.GetCropDetails(id);
        }

        /// <summary>
        /// 生成果实
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0; i < cropDetails.producedItemId.Length; i++)
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
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }

                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {   //在角色身上生成
                        EventHandler.CallHarvestAtPlayerPositionEvent(cropDetails.producedItemId[i]);
                    }
                    else
                    {
                        //在世界地图中生成
                        var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX), transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                        EventHandler.CallInstantiateItemInSceneEvent(cropDetails.producedItemId[i], spawnPos);

                    }
                }
            }
        }
    }
}