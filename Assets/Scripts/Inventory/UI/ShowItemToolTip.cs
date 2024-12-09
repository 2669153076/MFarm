using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    /// <summary>
    /// 显示提示面板
    /// </summary>
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI=>GetComponentInParent<InventoryUI>();

        private void Awake()
        {
            slotUI=GetComponent<SlotUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetupToolTip(slotUI.itemDetails, slotUI.slotType);

                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2 (1, 0);   //设置锚点
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;  //设置位置

                if(slotUI.itemDetails.itemType == E_ItemType.Furniture) //如果物品类型是蓝图
                {
                    inventoryUI.itemToolTip.resourcePanel.SetActive(true);
                    inventoryUI.itemToolTip.SetResourcePanel(slotUI.itemDetails.itemId);
                }
                else
                {
                    inventoryUI.itemToolTip.resourcePanel.SetActive(false);
                }
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }
    }
}