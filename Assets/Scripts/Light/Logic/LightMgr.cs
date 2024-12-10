using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Light{
    public class LightMgr : MonoBehaviour
    {
        private LightController[] sceneLights;
        private E_LightShift currentLightShift;
        private E_Season currentSeason;
        private float timeDifference;

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
        }
        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        }


        private void OnAfterSceneLoadEvent()
        {
            sceneLights = FindObjectsOfType<LightController>();

            foreach (var light in sceneLights)
            {
                light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
            }
        }
        private void OnLightShiftChangeEvent(E_Season season, E_LightShift lightShift, float timeDifference)
        {
            currentSeason = season;
            this.timeDifference = timeDifference;
            if (currentLightShift != lightShift)
            {
                currentLightShift = lightShift;
                foreach (var light in sceneLights)
                {
                    light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
                }
            }
        }
    }
}