using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 物品提示UI
/// </summary>
public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text sellMoneyText;
    [SerializeField] private GameObject bottomPart;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置提示面板的信息
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="slotType"></param>
    public void SetupToolTip(ItemDetails itemDetails,E_SlotType slotType)
    {
        nameText .text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDesctription;

        if (itemDetails.itemType == E_ItemType.Seed || itemDetails.itemType == E_ItemType.Commodity || itemDetails.itemType == E_ItemType.Furniture)
        {
            bottomPart.SetActive(true);

            var price = itemDetails.itemPrice;
            if(slotType == E_SlotType.Bag)
            {
                price = (int)(price*itemDetails.sellPercentage);
            }
            sellMoneyText.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>()); //强制刷新
    }

    /// <summary>
    /// 获取物品类型
    /// </summary>
    /// <param name="itemType">物品类型</param>
    /// <returns>指定字符</returns>
    private string GetItemType(E_ItemType itemType)
    {
        return itemType switch
        {
            E_ItemType.None => "无",
            E_ItemType.Seed => "种子",
            E_ItemType.Commodity => "商品",
            E_ItemType.Furniture => "家具",
            E_ItemType.BreakTool => "工具",
            E_ItemType.ChopTool => "工具",
            E_ItemType.CollectTool => "工具",
            E_ItemType.HoeTool => "工具",
            E_ItemType.ReapTool => "工具",
            E_ItemType.WaterTool => "工具",
            _=>"无"
        };
    }
}
