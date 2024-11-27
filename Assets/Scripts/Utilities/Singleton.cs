using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this as T;
    }

    protected virtual void OnDestory()
    {
        if (instance == this)
            instance = null;
    }
}
