using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;
        public InventoryBag_SO boxBagData;

        public GameObject mouseIcon;
        private bool canOpen = false;
        private bool isOpen;

        public int index;

        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }
        }

        private void Update()
        {
            if (!isOpen && canOpen && Input.GetKeyDown(KeyCode.F))
            {
                EventHandler.CallBaseBagOpenEvent(E_SlotType.Box, boxBagData);
                isOpen = true;
            }
            if(!canOpen && isOpen)
            {
                EventHandler.CallBaseBagCloseEvent(E_SlotType.Box, boxBagData);
                isOpen = false;
            }
            if(isOpen&&Input.GetKeyDown(KeyCode.Escape))
            {
                EventHandler.CallBaseBagCloseEvent(E_SlotType.Box, boxBagData);
                isOpen = false;
            }
        }

        /// <summary>
        /// 初始化箱子信息
        /// 建造时调用
        /// </summary>
        public void InitBox(int boxIndex)
        {
            this.index  = boxIndex;
            var key = this.name + index;
            if (InventoryMgr.Instance.GetBoxDataList(key) != null)
            {
                boxBagData.inventoryItemList = InventoryMgr.Instance.GetBoxDataList(key);
            }
            else
            {
                InventoryMgr.Instance.AddBoxDataDic(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }
    }
}