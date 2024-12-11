using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioMgr : Singleton<AudioMgr>
{
    [Header("数据")]
    public SoundDetailsList_SO soundDetailsData;    //音效
    public SceneSoundList_SO sceneSoundData;    //BGM
    [Header("组件")]
    public AudioSource ambientSource;
    public AudioSource musicSource;
    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot ambientSnapshot;
    public AudioMixerSnapshot muteSnapshot;
    [Header("AudioMixer")]
    public AudioMixer audioMixer;


    private Coroutine soundRoutine;
    private float musicTransitionSecond = 8f;

    public float MusicStartSecond => Random.Range(5f, 15f);

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }


    /// <summary>
    /// 播放背景音
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails,float transitionTime)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        musicSource.clip = soundDetails.soundClip;
        if (musicSource.isActiveAndEnabled)
        {
            musicSource.Play();
        }

        normalSnapshot.TransitionTo(musicTransitionSecond);
    }
    /// <summary>
    /// 播放环境音
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails,float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
        {
            ambientSource.Play();
        }

        ambientSnapshot.TransitionTo(transitionTime);
    }

    private IEnumerator PlaySoundRoutine(SoundDetails musicDetails,SoundDetails ambientDetails)
    {
        if (musicDetails!=null&&ambientDetails!=null)
        {
            PlayAmbientClip(ambientDetails, 1f);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(musicDetails,musicTransitionSecond);
        }
    }
    private float ConvertSoundVolume(float amount)
    {
        return amount * 100 - 80;
    }

    public void SetMasterVolume(float value)
    {
        //audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
        audioMixer.SetFloat("MusicVolume", (value * 100 - 80));
        audioMixer.SetFloat("AmbientVolume", (value * 100 - 80));
    }
    public void SetEffectVolume(float value)
    {
        audioMixer.SetFloat("EffectVolume", (value * 100 - 80));
    }


    private void OnAfterSceneLoadEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        var currentSceneSoundItem = sceneSoundData.GetSceneSoundItem(currentScene);
        if (currentSceneSoundItem == null)
        {
            return;
        }
        SoundDetails ambientDetails = soundDetailsData.GetSoundDetails(currentSceneSoundItem.ambient);
        SoundDetails musicDetails = soundDetailsData.GetSoundDetails(currentSceneSoundItem.music);

        PlayAmbientClip(ambientDetails, musicTransitionSecond);
        PlayMusicClip(musicDetails, musicTransitionSecond);

        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        soundRoutine = StartCoroutine(PlaySoundRoutine(musicDetails, ambientDetails));
    }

    private void OnPlaySoundEvent(E_SoundName name)
    {
        var soundDetails = soundDetailsData.GetSoundDetails(name);
        if (soundDetails != null)
        {
            EventHandler.CallInitSoundEffectEvent(soundDetails);
        }

    }

    private void OnEndGameEvent()
    {
        if (soundRoutine!=null)
        {
            StopCoroutine(soundRoutine);
        }
        muteSnapshot.TransitionTo(1f);
    }


}
