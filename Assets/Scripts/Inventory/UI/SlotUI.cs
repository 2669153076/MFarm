using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory{
    /// <summary>
    /// 格子UI
    /// </summary>
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField]private Image slotImage;    //道具图标
        [SerializeField]private Text amountText;    //数量
        [SerializeField]public Image slotHighlightImage;   //高亮
        [SerializeField]private Button button;

        [Header("格子类型")]
        public E_SlotType slotType;

        public bool isSelected; //是否被选中
        [HideInInspector]public int curSlotIndex;  //当前格子索引

        [HideInInspector]public ItemDetails itemDetails;    //被选中的道具信息
        [HideInInspector]public int itemAmount; //被选中的道具数量

        public InventoryUI InventoryUI => GetComponentInParent<InventoryUI>();
        public E_InventoryLocation Location
        {
            get
            {
                return slotType switch
                {
                    E_SlotType.Bag=>E_InventoryLocation.Player,
                    E_SlotType.Box=>E_InventoryLocation.Box,
                    _=>E_InventoryLocation.None,
                };
            }
        }

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 将格子变为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if(isSelected)
            {
                isSelected = false;

                InventoryUI.UpdateBagHighlight(-1); //清空之后格子高亮关闭

                EventHandler.CallItemSelectedEvent(itemDetails, false);
            }

            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        /// <summary>
        /// 更新格子信息
        /// </summary>
        /// <param name="itemDetails">道具信息</param>
        /// <param name="amount">道具数量</param>
        public void UpdateSlot(ItemDetails itemDetails,int amount)
        {
            this.itemDetails = itemDetails;
            slotImage.sprite = this.itemDetails.itemIcon;
            this.itemAmount = amount;
            amountText.text = this.itemAmount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(itemDetails == null)
            {
                return;
            }
            isSelected = !isSelected;
            InventoryUI.UpdateBagHighlight(curSlotIndex);

            //if (slotType == E_SlotType.Bag&&itemDetails.canCarried)       
            if (slotType == E_SlotType.Bag) 
            {
                //将背包中选中的物品举起来
                EventHandler.CallItemSelectedEvent(this.itemDetails, isSelected);
            }

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                InventoryUI.dragItemImage.enabled = true;
                InventoryUI.dragItemImage.sprite = slotImage.sprite;
                InventoryUI.dragItemImage.SetNativeSize();

                isSelected = true;
                InventoryUI.UpdateBagHighlight(curSlotIndex);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            InventoryUI.dragItemImage.transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            InventoryUI.dragItemImage.enabled = false;

            if(eventData.pointerCurrentRaycast.gameObject != null )
            {
                if(eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                {
                    return;
                }
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.curSlotIndex;

                if(slotType == E_SlotType.Bag&&targetSlot.slotType == E_SlotType.Bag)   //当前选中的是背包内的物品 并且 目标格子也是背包格子（只可以在背包内交换）
                {
                    //交换物品
                    InventoryMgr.Instance.SwapItem(curSlotIndex, targetIndex);
                }else if (slotType == E_SlotType.Shop&&targetSlot.slotType == E_SlotType.Bag)       //购买
                {
                    EventHandler.CallShowTradeUIEvent(itemDetails, false);

                }else if(slotType == E_SlotType.Bag&&targetSlot.slotType == E_SlotType.Shop)    //出售
                {
                    EventHandler.CallShowTradeUIEvent(itemDetails, true);
                }else if(slotType!=E_SlotType.Shop && targetSlot.slotType != E_SlotType.Shop && slotType != targetSlot.slotType)
                {
                    InventoryMgr.Instance.SwapItem(Location, curSlotIndex, targetSlot.Location, targetIndex);
                }
            }
            //else  //如果将物品移到地图中
            //{
            //    if (itemDetails.canDropped)   //扔掉物品
            //    {
            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //        EventHandler.CallInstantiateItemInScene(itemDetails.itemId, pos);
            //    }
            //}
            //清空所有高亮显示
            InventoryUI.UpdateBagHighlight(-1);
        }

    }
}
