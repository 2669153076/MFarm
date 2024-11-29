using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 限制边界
/// </summary>
public class SwitchBounds : MonoBehaviour
{
    private void Start()
    {
        //SwitchConfinerShape();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchConfinerShape;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchConfinerShape;

    }

    /// <summary>
    /// 限制相机边界
    /// </summary>
    public void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;

        //Call this if the bounding shape's points change at runtime
        confiner.InvalidatePathCache();
    }
}
