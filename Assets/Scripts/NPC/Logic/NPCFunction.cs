using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;
    private bool isOpen;

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    /// <summary>
    /// 打开背包
    /// 被DialogueController拖曳调用
    /// </summary>
    public void OpenShop()
    {
        isOpen = true; 
        EventHandler.CallBaseBagOpenEvent(E_SlotType.Shop,shopData);
        EventHandler.CallUpdateGameStateEvent(E_GameState.Pause);
    }

    public void CloseShop()
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(E_SlotType.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(E_GameState.Playing);
    }
}
