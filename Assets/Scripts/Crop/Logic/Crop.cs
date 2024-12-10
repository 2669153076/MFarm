using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CropPlant{
    /// <summary>
    /// 生成的作物
    /// </summary>
    public class Crop : MonoBehaviour
    {
        public CropDetails cropDetails;

        private int harvestActionCount; //点击计数器
        public TileDetails tileDetails; //格子信息

        private Animator animator;
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;
        public bool CanHarvest => tileDetails.growthDays>= cropDetails.TotalGrowDays;

        /// <summary>
        /// 使用工具
        /// </summary>
        /// <param name="tool"></param>
        public void ProcessToolAction(ItemDetails tool,TileDetails tileDetails)
        {
            this.tileDetails = tileDetails;

            int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemId);
            if (requireActionCount==-1)
            {
                return;
            }

            animator = GetComponentInChildren<Animator>();

            //点击计数器
            if (harvestActionCount < requireActionCount)
            {
                harvestActionCount++;

                //判断是否有动画 树木
                if(animator != null&&this.cropDetails.hasAnimation)
                {
                    if(PlayerTransform.position.x<transform.position.x)
                    {
                        animator.SetTrigger("RotateRight");
                    }
                    else
                    {
                        animator.SetTrigger("RotateLeft");
                    }
                }

                //播放粒子
                if (this.cropDetails.hasParticalEffect)
                {
                    EventHandler.CallParticleEffectEvent(this.cropDetails.particaleEffectType, transform.position + this.cropDetails.effectPos);
                }

                //播放声音
                if(this.cropDetails.soundEffectName != E_SoundName.None)
                {
                    EventHandler.CallPlaySoundEvent(this.cropDetails.soundEffectName);
                }
            }
            if (harvestActionCount>=requireActionCount)
            {
                if (cropDetails.generateAtPlayerPosition||!cropDetails.hasAnimation)
                {
                    //生成农作物
                    SpawnHarvestItems();
                }
                else if(cropDetails.hasAnimation)
                {
                    if (PlayerTransform.position.x < transform.position.x)
                    {
                        animator.SetTrigger("FallingRight");
                    }
                    else
                    {
                        animator.SetTrigger("FallingLeft");
                    }
                    EventHandler.CallPlaySoundEvent(E_SoundName.TreeFalling);
                    StartCoroutine(HarvestAfterAnimation());
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

            if (tileDetails != null)
            {
                tileDetails.daysSinceLastHarvest++;
                if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
                {
                    tileDetails.growthDays = cropDetails.TotalGrowDays - cropDetails.daysToRegrow;
                    //刷新种子
                    EventHandler.CallRefreshCurrentMap();
                }
                else
                {
                    tileDetails.daysSinceLastHarvest = -1;
                    tileDetails.seedItemId = -1;

                    //tileDetails.daysSinceDig = -1;
                }

                Destroy(gameObject);
            }
           
        }
        /// <summary>
        /// 创建转换物体
        /// </summary>
        private void CreateTransferCrop()
        {
            tileDetails.seedItemId = cropDetails.transferItemId;
            tileDetails.daysSinceLastHarvest = -1;
            tileDetails.growthDays = 0;

            EventHandler.CallRefreshCurrentMap();
        }

        private IEnumerator HarvestAfterAnimation()
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                yield return null;
            }

            SpawnHarvestItems();

            //转换新物体
            if(cropDetails.transferItemId>0)
            {
                CreateTransferCrop();
            }

        }
    }
}
