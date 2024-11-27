using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory{
    /// <summary>
    /// 库存UI
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private SlotUI[] playerSlots;  //背包格子数组，需要拖动赋值
        [SerializeField] private GameObject bagUI;  //背包UI
        public Image dragItemImage; //可拖动的图片
        private bool bagIsOpened;   //背包是否打开
        public ItemToolTip itemToolTip; //物品提示UI

        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagIsOpened = bagUI.activeInHierarchy;
            dragItemImage.enabled = false;
        }
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        }
        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                SwitchBagUIActive();
            }
        }

        /// <summary>
        /// 切换背包显示隐藏
        /// 在场景中被bagbutton调用
        /// </summary>
        public void SwitchBagUIActive()
        {
            bagIsOpened = !bagIsOpened;
            bagUI.SetActive(bagIsOpened);
        }

        /// <summary>
        /// 背包格子高亮
        /// </summary>
        /// <param name="index"></param>
        public void UpdateBagHighlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if(slot.isSelected&&slot.slotIndex == index)
                {
                    slot.slotHighlightImage.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighlightImage.gameObject.SetActive(false);
                }
            }
        }

        private void OnUpdateInventoryUI(E_InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case E_InventoryLocation.None:
                    break;
                case E_InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryMgr.Instance.GetItemDetails(list[i].itemId);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case E_InventoryLocation.Box:
                    break;
            }
        }

    }
}