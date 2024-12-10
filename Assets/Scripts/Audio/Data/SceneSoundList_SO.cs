using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneSoundList_SO", menuName = "Sound/SceneSoundList")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundItemList;

    public SceneSoundItem GetSceneSoundItem(string sceneName)
    {
        return sceneSoundItemList.Find(i => i.sceneName == sceneName);
    }
}
