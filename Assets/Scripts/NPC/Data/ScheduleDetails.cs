using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色行动轨迹
/// </summary>
[System.Serializable]
public class ScheduleDetails : IComparable<ScheduleDetails>
{
    public int priority;    //优先级 越小越优先执行
    public int day, hour, minute;
    public E_Season season;

    public string targetScene; //目标场景
    public Vector2Int targetGridPosition;   //目标格子坐标

    public AnimationClip clipAtStop;    //停下来后播放什么动画
    public bool interactable;   //是否可以与玩家互动

    public int Time => (hour * 100) + minute;
    public ScheduleDetails(int priority, int day, int hour, int minute, E_Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable)
    {
        this.priority = priority;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.clipAtStop = clipAtStop;
        this.interactable = interactable;
    }
    /// <summary>
    /// 比较执行优先级<br/>
    /// 升序排列
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// -1  放左边<br/>
    /// 0   不变<br/>
    /// 1   放右边
    /// </returns>
    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time)
        {
            {
                return -1;
            }
        }
        return 0;
    }
}
