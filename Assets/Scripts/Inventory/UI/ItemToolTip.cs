using Inventory;
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
    public GameObject resourcePanel;
    [SerializeField] private Image resourceItemPrefab;

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

    /// <summary>
    /// 设置建造资源面板
    /// </summary>
    /// <param name="blueprintDetails">蓝图信息</param>
    public void SetResourcePanel(BlueprintDetails blueprintDetails)
    {
        for (int i = 0; i < resourcePanel.transform.childCount; i++)
        {
            Destroy(resourcePanel.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < blueprintDetails.resourceItem.Length; i++)
        {
            Image resourceItem = Instantiate(resourceItemPrefab, resourcePanel.transform);
            resourceItem.sprite = InventoryMgr.Instance.GetItemDetails(blueprintDetails.id).itemIcon;
            resourceItem.transform.GetChild(0).GetComponent<Text>().text = blueprintDetails.resourceItem[i].itemAmount.ToString();
        }
    }
    public void SetResourcePanel(int id)
    {
        for (int i = 0; i < resourcePanel.transform.childCount; i++)
        {
            Destroy(resourcePanel.transform.GetChild(i).gameObject);
        }
        var blueprintDetails = InventoryMgr.Instance.blueprintDataList_SO.GetBlueprintDetails(id);
        for (int i = 0; i < blueprintDetails.resourceItem.Length; i++)
        {
            Image resourceItem = Instantiate(resourceItemPrefab, resourcePanel.transform);
            resourceItem.sprite = InventoryMgr.Instance.GetItemDetails(blueprintDetails.resourceItem[i].itemId).itemIcon;
            resourceItem.transform.GetChild(0).GetComponent<Text>().text = blueprintDetails.resourceItem[i].itemAmount.ToString();
        }
    }
}
