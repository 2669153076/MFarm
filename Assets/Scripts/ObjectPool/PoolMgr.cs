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

    private Queue<GameObject> soundQueue = new Queue<GameObject>();

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        EventHandler.InitSoundEffectEvent += OnInitSoundEffectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffectEvent -= OnInitSoundEffectEvent;
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


    //private void InitSoundEffect(SoundDetails soundDetails)
    //{
    //    ObjectPool<GameObject> pool = poolEffectList[4];
    //    var obj = pool.Get();

    //    obj.GetComponent<Sound>().SetSound(soundDetails);
    //    StartCoroutine(DisableSound(pool, obj, soundDetails));
    //}

    //private IEnumerator DisableSound(ObjectPool<GameObject> pool, GameObject obj, SoundDetails soundDetails)
    //{
    //    yield return new WaitForSeconds(soundDetails.soundClip.length);
    //    pool.Release(obj);
    //}
    /// <summary>
    /// 创建音效
    /// </summary>
    private void CreateSoundPool()
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);
        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }
    /// <summary>
    /// 从对象池中获取音效物体
    /// </summary>
    /// <returns></returns>
    private GameObject GetSoundPool()
    {
        if (soundQueue.Count < 2)
            CreateSoundPool();
        return soundQueue.Dequeue();
    }
    /// <summary>
    /// 初始化音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void InitSoundEffect(SoundDetails soundDetails)
    {
        var obj = GetSoundPool();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }
    private IEnumerator DisableSound(GameObject obj,float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
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
    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(2.5f);
        pool.Release(obj);
    }

    private void OnInitSoundEffectEvent(SoundDetails details)
    {
        InitSoundEffect(details);
    }
}
