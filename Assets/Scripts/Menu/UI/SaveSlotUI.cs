using MFarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始主界面UI的存档读取按钮
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;

    private DataSlot currentData;

    private int Index => transform.GetSiblingIndex();

    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }

    private void OnEnable()
    {
        SetUpSlotUI();
    }

    private void SetUpSlotUI()
    {
        currentData = SaveLoadMgr.Instance.dataSlotList[Index];

        if(currentData != null )
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {

            dataTime.text = string.Empty;
            dataScene.text = string.Empty;
        }
    }

    private void LoadGameData()
    {
        if(currentData != null )
        {
            SaveLoadMgr.Instance.Load(this.Index);
        }
        else
        {
            EventHandler.CallStartNewGameEvent(this.Index);
        }
    }
}
