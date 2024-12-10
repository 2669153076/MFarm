using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPatternList_SO", menuName = "Light/LightPatternList")]
public class LightPatternList_SO : ScriptableObject
{
    public List<LightDetails> lightDetailsList;

    public LightDetails GetLightDetails(E_Season season,E_LightShift lightShift)
    {
        return lightDetailsList.Find(i => i.season == season && i.lightShift == lightShift);
    }
}
