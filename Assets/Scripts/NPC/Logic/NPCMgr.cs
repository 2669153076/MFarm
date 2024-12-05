using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMgr : Singleton<NPCMgr>
{
    public SceneRouteDataList_SO sceneRouteData;
    public List<NPCPosition> npcPositionList;

    private Dictionary<string ,SceneRoute> sceneRouteDic = new Dictionary<string ,SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDic();
    }

    /// <summary>
    /// 初始化路径字典
    /// </summary>
    private void InitSceneRouteDic()
    {
        if(sceneRouteData.sceneRouteList.Count > 0)
        {
            foreach (var route in sceneRouteData.sceneRouteList)
            {
                var key = route.fromSceneName+route.toSceneName;

                if(sceneRouteDic.ContainsKey(key))
                {
                    continue;
                }
                else
                {
                    sceneRouteDic.Add(key, route);
                }
            }
        }
    }

    /// <summary>
    /// 获取场景路径
    /// </summary>
    /// <param name="fromsceneName">起点</param>
    /// <param name="tosceneName">终点</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromsceneName,string tosceneName)
    {
        return sceneRouteDic[fromsceneName + tosceneName];
    }
}
