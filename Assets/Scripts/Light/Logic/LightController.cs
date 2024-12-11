using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MFarm.Light{
    public class LightController : MonoBehaviour
    {
        public LightPatternList_SO lightPatternData;
        private Light2D currentLight;
        private LightDetails currentLightDetails;

        private void Awake()
        {
            currentLight = GetComponent<Light2D>();
        }

        /// <summary>
        /// 切换灯光
        /// </summary>
        /// <param name="season"></param>
        /// <param name="lightShift"></param>
        /// <param name="timeDifference"></param>
        public void ChangeLightShift(E_Season season,E_LightShift lightShift,float timeDifference)
        {
            currentLightDetails = lightPatternData.GetLightDetails(season, lightShift);
            if (timeDifference < Settings.lightChangeDuration)
            {
                var colorOffset = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration * timeDifference;
                currentLight.color += colorOffset;
                DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);
                DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightIntensity, Settings.lightChangeDuration - timeDifference);
            }
            if (timeDifference >= Settings.lightChangeDuration)
            {
                currentLight.color = currentLightDetails.lightColor;
                currentLight.intensity = currentLightDetails.lightIntensity;
            }
        }
    }
}