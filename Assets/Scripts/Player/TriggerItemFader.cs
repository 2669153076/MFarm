using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触碰物品alpha缓入缓出
/// 使人物不会被树或者草等完全挡住
/// </summary>
public class TriggerItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();

        if(faders.Length > 0)
        {
            foreach(var item in faders)
            {
                item.FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();

        if (faders.Length > 0)
        {
            foreach (var item in faders)
            {
                item.FadeIn();
            }
        }
    }
}
