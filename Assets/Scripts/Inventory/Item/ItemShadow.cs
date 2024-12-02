using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;
        public SpriteRenderer shadowSprite;

        private void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            shadowSprite.sprite = itemSprite.sprite;
            shadowSprite.color = new Color(0, 0, 0, 0.3f);
        }
    }
}