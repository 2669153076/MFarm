using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDetailsList_SO", menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDetails> soundDetailsList;

    public SoundDetails GetSoundDetails(E_SoundName name)
    {
        return soundDetailsList.Find(i => i.soundName == name);
    }
}
