using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Item item = collision.gameObject.GetComponent<Item>();

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