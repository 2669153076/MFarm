using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Transition{
    public class TransitionMgr : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
        }

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
            
        }
        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneName">需要加载的目标场景名</param>
        /// <param name="targetPosition">角色在另一个场景中的位置</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallMoveToPositionEvent(targetPosition);   //移动角色坐标

            EventHandler.CallAfterSceneLoadEvent();
        }

        /// <summary>
        /// 携程加载场景叠加并激活
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);
        }



        private void OnTransitionEvent(string sceneName, Vector3 pos)
        {
            StartCoroutine(Transition(sceneName, pos));
        }
    }
}