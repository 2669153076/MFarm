using MFarm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if(Application.isPlaying)
        {
            if(dialoguePiece.hasToPause)
            {
                //暂停Timeline
                TimelineMgr.Instance.PauseTimeline(director);
            }
            else
            {

                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(Application.isPlaying)
        {
            TimelineMgr.Instance.IsDone = dialoguePiece.isDone;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(null);
    }

    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallShowDialogueEvent(null);
        EventHandler.CallUpdateGameStateEvent(E_GameState.Playing);
    }

    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(E_GameState.Pause);
    }
}
