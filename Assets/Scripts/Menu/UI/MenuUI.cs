using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 菜单界面
/// </summary>
public class MenuUI : MonoBehaviour
{
    public GameObject[] panels;

    /// <summary>
    /// 切换面板
    /// </summary>
    /// <param name="index"></param>
    public void SwitchPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if(i == index)
            {
                panels[i].transform.SetAsLastSibling();
            }
        }
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
