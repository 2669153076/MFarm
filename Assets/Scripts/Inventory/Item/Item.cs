using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory{
    public class Item : MonoBehaviour
    {
        public int itemId;
        private SpriteRenderer spriteRenderer;
        [HideInInspector]public ItemDetails itemDetails;
        private BoxCollider2D coll;

        

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            Init(itemId);
        }

        public void Init(int id)
        {
            itemId = id;
            itemDetails = InventoryMgr.Instance.GetItemDetails(id);
            if(itemDetails!=null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite!=null?itemDetails.itemOnWorldSprite:itemDetails.itemIcon;

                //修改碰撞体尺寸
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x,spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.x / 2);
            }
        }

       
    }
}