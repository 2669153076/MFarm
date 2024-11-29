using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMgr : MonoBehaviour
{
    public Item itemPerfab;
    [HideInInspector]public Transform itemParent;

    private void OnEnable()
    {
        EventHandler.InstantiateItemInSceneEvent += OnInstantiateItemInScene;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateItemInSceneEvent -= OnInstantiateItemInScene;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnInstantiateItemInScene(int id, Vector3 pos)
    {
        var item = Instantiate(itemPerfab,pos,Quaternion.identity,itemParent);
        item.itemId = id;
    }

    private void OnAfterSceneLoadEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
    }

}
