using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用设置属性
/// </summary>
public class Settings
{
    public const float itemFadeDuration = 0.35f;   //缓入缓出时间
    public const float targetAlpha = 0.45f;    //目标Alpha值

    //时间相关
    public const float secondThreshold = 0.012f;   //数值越小时间流速越快
    public const int secondHold = 59;       //时间进制0-59
    public const int minuteHold = 59;
    public const int hourHold = 23; //0-23
    public const int dayHold = 30;  //1-30
    public const int monthHold = 12;    //1-12
    public const int seasonHold = 4;    //1-4

    //场景切换相关
    public const float fadeDuration = 1.5f;

    public const int reapAmount = 2;  //一次性销毁多少个可收割物品
}
