using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage; //白天、夜晚、凌晨、傍晚
    public RectTransform clockParent;   //时辰父物体
    public Image seasonImage;   //季节
    public Text dateText;   //日期
    public Text timeText;   //时间

    public Sprite[] seasonSprites;  //各季节精灵图片
    private List<GameObject> clockBlocks = new List<GameObject>(); //时辰图片列表

    private void Awake()
    {
        for (int i = 0;i<clockParent.childCount;i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }
    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, E_Season season)
    {
        dateText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season-1];
        SwitchHourImage(hour);
        RotateDayNightImage(hour);
    }

    /// <summary>
    /// 切换时辰图片(添加或删除）
    /// </summary>
    /// <param name="hour">当前几点</param>
    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        if(index == 0 )
        {
            foreach (var item in clockBlocks)
            {
                item.SetActive(false);
            }
        }
        else
        {
            for (int i = 0;i<clockBlocks.Count;i++)
            {
                if (i < index+1)
                {
                    clockBlocks[i].SetActive(true);
                }
                else
                {
                    clockBlocks[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 旋转日夜图片
    /// </summary>
    /// <param name="hour"></param>
    private void RotateDayNightImage(int hour)
    {
        var target = new Vector3(0, 0, hour * 15-45);
        dayNightImage.DORotate(target,1f,RotateMode.Fast);
    }
}
