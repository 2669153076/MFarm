using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画调用事件
/// </summary>
public class AnimationEvent : MonoBehaviour
{
    public void FootstepSound()
    {
        EventHandler.CallPlaySoundEvent(E_SoundName.FootStepHard);
    }
}
