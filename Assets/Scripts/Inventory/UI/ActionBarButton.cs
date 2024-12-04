using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slot;

        private void Awake()
        {
            slot = GetComponent<SlotUI>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(key))
            {
                if(slot.itemDetails != null)
                {
                    slot.isSelected = !slot.isSelected;
                    if(slot.isSelected )
                    {
                        slot.InventoryUI.UpdateBagHighlight(slot.slotIndex);
                    }
                    else
                    {
                        slot.InventoryUI.UpdateBagHighlight(-1);
                    }
                    EventHandler.CallItemSelectedEvent(slot.itemDetails,slot.isSelected); 
                }
            }
        }
    }
}