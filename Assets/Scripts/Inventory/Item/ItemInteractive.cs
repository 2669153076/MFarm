using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 互动效果
/// 1.碰到草时晃动草
/// </summary>
public class ItemInteractive : MonoBehaviour
{
    private bool isAnimating;   //是否正在播放动画
    private WaitForSeconds pause = new WaitForSeconds(0.04f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if(collision.transform.position.x<transform.position.x)
            {
                StartCoroutine(RotateRight());
            }
            else
            {
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(E_SoundName.Rustle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if (collision.transform.position.x > transform.position.x)
            {
                StartCoroutine(RotateRight());
            }
            else
            {
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(E_SoundName.Rustle);
        }
    }

    /// <summary>
    /// 向左摇晃
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotateLeft()
    {
        isAnimating = true;

        for (int i = 0;i<4;i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;
        }
        transform.GetChild(0).Rotate(0, 0, 2);
        yield return pause;

        isAnimating = false;
    }
    /// <summary>
    /// 向右摇晃
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotateRight()
    {
        isAnimating = true;

        for (int i = 0;i<4;i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;
        }
        transform.GetChild(0).Rotate(0, 0, -2);
        yield return pause;

        isAnimating = false;
    }
}
