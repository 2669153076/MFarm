using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueprintDataList_SO", menuName = "Inventory/BlueprintDataList")]
public class BlueprintDataList_SO : ScriptableObject
{
    public List<BlueprintDetails> blueprintDetailsList;

    public BlueprintDetails GetBlueprintDetails(int itemId)
    {
        return blueprintDetailsList.Find(i=>i.id == itemId);
    }
}
