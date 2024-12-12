using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineMgr : Singleton<TimelineMgr>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isPause;   //对话是否暂停

    private bool isDone;    //是否完成对话
    public bool IsDone { set { isDone = value; } }

    private bool isFirst;

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void Update()
    {
        if(isPause&&Input.GetKeyDown(KeyCode.Space))
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    public void PauseTimeline(PlayableDirector  playableDirector)
    {
        currentDirector = playableDirector;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }

    private void OnAfterSceneLoadEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if(currentDirector != null&& isFirst)
        {
            currentDirector.Play();
            isFirst = false;
        }
    }

    private void OnStartNewGameEvent(int obj)
    {
        isFirst = true;
    }

}
