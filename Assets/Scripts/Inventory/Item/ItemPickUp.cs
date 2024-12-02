using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory{
    /// <summary>
    /// 拾取物品
    /// </summary>
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            Item item = collider.GetComponent<Item>();

            if (item != null)
            {
                if (item.itemDetails.canPickedUp)
                {
                    InventoryMgr.Instance.AddItem(item, true);
                }
            }
        }
    }
}