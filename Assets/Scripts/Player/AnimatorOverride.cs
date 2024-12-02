using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画控制器
/// </summary>
public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;   
    public SpriteRenderer holdItem; //被举起的物品图片

    [Header("各部分动画列表")]
    public List<AnimatorType> animatorTypeList;

    private Dictionary<string,Animator> animatorDic = new Dictionary<string,Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        holdItem.enabled = false;

        foreach (Animator animator in animators)
        {
            animatorDic.Add(animator.name, animator);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //WORKFLOW:不同的工具返回不同的动画
        E_PartType currentType = itemDetails.itemType switch
        {
            E_ItemType.Seed => E_PartType.Carry,
            E_ItemType.Commodity => E_PartType.Carry,
            E_ItemType.HoeTool => E_PartType.Hoe,
            _ => E_PartType.None,
        };
        if (!isSelected)
        {
            currentType = E_PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if(currentType == E_PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
        }

        SwitchAnimator(currentType);
    }

    /// <summary>
    /// 切换动画
    /// </summary>
    /// <param name="e_PartType"></param>
    private void SwitchAnimator(E_PartType e_PartType)
    {
        foreach (var item in animatorTypeList)
        {
            if(item.partType == e_PartType)
            {
                animatorDic[item.partName.ToString()].runtimeAnimatorController = item.animatorOverrideController;
            }
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;   
        SwitchAnimator(E_PartType.None);
    }

}
