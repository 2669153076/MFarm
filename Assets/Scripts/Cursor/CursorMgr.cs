using CropPlant;
using GridMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 鼠标管理类
/// 切换光标
/// </summary>
public class CursorMgr : Singleton<CursorMgr>
{
    public Sprite normal, seed, tool, commodity;    //类型 普通、种子、工具、商品

    private Sprite currentSprite;   //当前鼠标图片
    private Image cursorImage;  //鼠标图片组件
    private RectTransform cursorCanvas; //鼠标Canvas

    private Camera mainCamera;
    private Grid currentGrid;   //当前场景的Grid

    private Vector3 mouseWorldPos;  //世界坐标
    private Vector3Int mouseGridPos;    //网格坐标
    private bool cursorEnable; //鼠标是否可用
    private bool cursorPositionValid;   //鼠标坐标位置是否可用

    private ItemDetails currentItemDetails; //当前选中的物品信息
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }
    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main; 
    }

    private void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnable)
        {
            //如果没有与UI交互并且鼠标可用
            //则鼠标光标为指定的图片
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            //如果与UI交互了 则鼠标光标为默认图片
            SetCursorImage(normal);
        }
    }

    /// <summary>
    /// 设置鼠标光标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 判断是否与UI交互
    /// </summary>
    /// <returns>
    /// true 与UI交互了<br/>
    /// false 没有与UI交互
    /// </returns>
    private bool InteractWithUI()
    {
        //如果当前输入事件系统不为空 && 鼠标交互点位于UI界面上
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// 检查是否可以进行交互
    /// 是否可以扔物品、挖坑等等
    /// </summary>
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);
        if(Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItemDetails.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItemDetails.itemUseRadius)
        {
            //距离大于物品可扔取范围，则不可扔取
            SetCursorInValid();
            return;
        }

        TileDetails currentTile = GridMapMgr.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if(currentTile != null)
        {
            CropDetails currentCropDetails = CropMgr.Instance.GetCropDetails(currentTile.seedItemId);
            Crop currentCrop = GridMapMgr.Instance.GetCropObject(mouseWorldPos);
            //WORKFLOW:补充物品类型的判断
            switch (currentItemDetails.itemType)
            {
                case E_ItemType.None:
                    break;
                case E_ItemType.Seed:   //种子
                    if(currentTile.daysSinceDig>-1&&currentTile.seedItemId == -1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.Commodity:  //商品
                    if (currentTile.canDropItem)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.Furniture:  //家具
                    break;
                case E_ItemType.HoeTool:    //锄头
                     if (currentTile.canDig)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.BreakTool:  //十字镐
                case E_ItemType.ChopTool:   //斧头
                    if (currentCrop!=null)
                    {
                        if (currentCrop.CanHarvest && currentCrop.cropDetails.CheckToolAvailable(currentItemDetails.itemId))
                        {
                            SetCursorValid();
                        }
                        else 
                        { 
                            SetCursorInValid(); 
                        }
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.ReapTool:   //镰刀
                    if (GridMapMgr.Instance.HaveReapableItemsInRadius(mouseWorldPos,currentItemDetails))
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.WaterTool:  //水壶
                    if (currentTile.daysSinceDig > -1 && currentTile.daysSinceWatered == -1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.CollectTool:    //篮子
                    if (currentCropDetails != null)
                    {
                        if (currentCropDetails.CheckToolAvailable(currentItemDetails.itemId))
                        {
                            if (currentTile.growthDays >= currentCropDetails.TotalGrowDays)
                            {
                                SetCursorValid();
                            }
                            else
                            {
                                SetCursorInValid();
                            }
                        }
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case E_ItemType.ReapableScenery:    //可收割风景物品
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
    }
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid= false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItemDetails);
        }
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //当物品为背包内物品时才会调用该事件，从而改变鼠标光标 
        //SlotUI->OnPointClicked可修改条件
        if (!isSelected)
        {
            currentItemDetails = null;
            currentSprite = normal; 
            cursorEnable = false;
        }
        else
        {
            currentItemDetails = itemDetails;
            //当前鼠标图片 随 所点击的物品种类 发生改变
            //WORKFLOW:添加所有类型对应的图片
            currentSprite = itemDetails.itemType switch
            {
                E_ItemType.ReapTool => tool,
                E_ItemType.CollectTool => tool,
                E_ItemType.WaterTool => tool,
                E_ItemType.HoeTool => tool,
                E_ItemType.BreakTool => tool,
                E_ItemType.ChopTool => tool,
                E_ItemType.Furniture => tool,
                E_ItemType.Seed=>seed,
                E_ItemType.Commodity => commodity,
                _=>normal,
            };
            cursorEnable = true;
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    private void OnAfterSceneLoadEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
        //cursorEnable = true;
        SetCursorImage(normal); //防止在跳转场景后物品取消选中而鼠标图片未恢复为默认
    }

}
