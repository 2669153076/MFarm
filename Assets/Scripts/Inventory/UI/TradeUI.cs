using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public Text itemName;
        public InputField tradeAmount;
        public Button submitBtn;
        public Button cancelBtn;

        private ItemDetails itemDetail;
        private bool isSellTrade;
        private void Awake()
        {
            cancelBtn.onClick.AddListener(CancelTrade);
            submitBtn.onClick.AddListener(TradeItem);
        }

        /// <summary>
        /// 设置交易UI信息
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="isSell"></param>
        public void SetTradeUI(ItemDetails itemDetails, bool isSell)
        {
            this.itemDetail = itemDetails;
            this.itemIcon.sprite = itemDetails.itemIcon;
            this.itemName.text = itemDetails.itemName;
            isSellTrade = isSell;
            this.tradeAmount.text = string.Empty;
        }

        /// <summary>
        /// 进行交易
        /// </summary>
        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);
            InventoryMgr.Instance.TradeItem(itemDetail, amount,isSellTrade);
            CancelTrade();
        }

        /// <summary>
        /// 取消交易
        /// </summary>
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}