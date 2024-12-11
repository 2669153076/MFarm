using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slot;
        private bool canUsed;   //能否被使用

        private void Awake()
        {
            slot = GetComponent<SlotUI>();
        }
        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }
        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }

        private void Update()
        {
            if(Input.GetKeyDown(key)&& canUsed)
            {
                if(slot.itemDetails != null)
                {
                    slot.isSelected = !slot.isSelected;
                    if(slot.isSelected )
                    {
                        slot.InventoryUI.UpdateBagHighlight(slot.curSlotIndex);
                    }
                    else
                    {
                        slot.InventoryUI.UpdateBagHighlight(-1);
                    }
                    EventHandler.CallItemSelectedEvent(slot.itemDetails,slot.isSelected); 
                }
            }
        }

        private void OnUpdateGameStateEvent(E_GameState state)
        {
            canUsed = state == E_GameState.Playing;
        }
    }
}