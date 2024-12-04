using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// 对象池管理器
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
    }

    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            var parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
                );

            poolEffectList.Add(newPool);
        }
    }


    private void OnParticleEffectEvent(E_ParticaleEffectType effectType, Vector3 pos)
    {
        //WORKFLOW:根据特效补全
        ObjectPool<GameObject> objPool = effectType switch
        {
            E_ParticaleEffectType.LeavesFalling01 => poolEffectList[0],
            E_ParticaleEffectType.LeavesFalling02 => poolEffectList[1],
            E_ParticaleEffectType.Rock=> poolEffectList[2],
            E_ParticaleEffectType.ReapableScenery => poolEffectList[3],
            _ => null
        };

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool,GameObject obj)
    {
        yield return new WaitForSeconds(2.5f);
        pool.Release(obj);
    }

}
