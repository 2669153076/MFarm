using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Light{
    public class LightMgr : MonoBehaviour
    {
        private LightController[] sceneLights;
        private E_LightShift currentLightShift;
        private E_Season currentSeason;
        private float timeDifference = Settings.lightChangeDuration;

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }


        private void OnAfterSceneLoadEvent()
        {
            sceneLights = FindObjectsOfType<LightController>();
            if (sceneLights != null)
            {
                foreach (var light in sceneLights)
                {
                    light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
                }
            }
        }
        private void OnLightShiftChangeEvent(E_Season season, E_LightShift lightShift, float timeDifference)
        {
            currentSeason = season;
            this.timeDifference = timeDifference;
            if (currentLightShift != lightShift&&sceneLights!=null)
            {
                currentLightShift = lightShift;
                foreach (var light in sceneLights)
                {
                    light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
                }
            }
        }


        private void OnStartNewGameEvent(int obj)
        {
            currentLightShift = E_LightShift.Morning;
        }
    }
}