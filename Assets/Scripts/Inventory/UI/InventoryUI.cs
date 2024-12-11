using DG.Tweening;
using MFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    /// <summary>
    /// 库存UI
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("背包")]
        [SerializeField] private SlotUI[] playerSlots;  //背包格子数组，需要拖动赋值
        [SerializeField] private GameObject bagUI;  //背包UI
        public Image dragItemImage; //可拖动的图片
        private bool bagIsOpened;   //背包是否打开
        public ItemToolTip itemToolTip; //物品提示UI

        [Space]
        [Header("通用")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        [SerializeField] private List<SlotUI> baseBagSlotList;

        [Space]
        [Header("交易UI")]
        public TradeUI tradeUI;
        public Text playerMoneyText;

        [Space]
        [Header("箱子")]
        public GameObject boxSlotPrefab;


        private void Start()
        {
            bagUI.gameObject.SetActive(false);
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].curSlotIndex = i;
            }
            bagIsOpened = bagUI.activeInHierarchy;
            dragItemImage.enabled = false;

            baseBag.SetActive(false);
            tradeUI.gameObject.SetActive(false);
            playerMoneyText.text = InventoryMgr.Instance.playerMoney.ToString();

        }
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUIEvent += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUIEvent += OnShowTradeUIEvent;
        }
        private void OnDisable()
        {
            EventHandler.UpdateInventoryUIEvent -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUIEvent -= OnShowTradeUIEvent;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
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
        /// <param name="index">高亮索引格子，传入-1 关闭所有高亮</param>
        public void UpdateBagHighlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.curSlotIndex == index)
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
                case E_InventoryLocation.Shop:
                    for (int i = 0; i < baseBagSlotList.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryMgr.Instance.GetItemDetails(list[i].itemId);
                            baseBagSlotList[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlotList[i].UpdateEmptySlot();
                        }
                    }

                    break;
            }
            playerMoneyText.text = InventoryMgr.Instance.playerMoney.ToString();
        }

        private void OnBeforeSceneUnloadEvent()
        {
            UpdateBagHighlight(-1);
        }

        private void OnBaseBagOpenEvent(E_SlotType type, InventoryBag_SO bagData)
        {
            GameObject prefab = type switch
            {
                E_SlotType.Shop => shopSlotPrefab,
                E_SlotType.Box => boxSlotPrefab,
                _ => null,
            };

            baseBag.SetActive(true);
            baseBagSlotList = new List<SlotUI>();
            //创建物品
            for (int i = 0; i < bagData.inventoryItemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.curSlotIndex = i;
                baseBagSlotList.Add(slot);
            }
            //强制刷新
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());

            if (type == E_SlotType.Shop)
            {
                bagUI.gameObject.SetActive(true);
                bagIsOpened = true;
                bagUI.gameObject.transform.DOLocalMoveX(180, 0.5f).SetEase(Ease.OutQuad);
            }


            //更新UI显示
            OnUpdateInventoryUI(E_InventoryLocation.Shop, bagData.inventoryItemList);
        }
        private void OnBaseBagCloseEvent(E_SlotType type, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateBagHighlight(-1);

            foreach (var slot in baseBagSlotList)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlotList.Clear();


            if (type == E_SlotType.Shop)
            {
                bagUI.gameObject.SetActive(false);
                bagIsOpened = false;
                bagUI.gameObject.transform.DOLocalMoveX(0, 2).SetEase(Ease.OutQuad);
            }
        }

        private void OnShowTradeUIEvent(ItemDetails itemDetails, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetTradeUI(itemDetails, isSell);
        }
    }
}