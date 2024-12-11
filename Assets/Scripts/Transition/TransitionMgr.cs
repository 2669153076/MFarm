using MFarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Transition{
    /// <summary>
    /// 场景跳转管理
    /// </summary>
    public class TransitionMgr : Singleton<TransitionMgr>, ISaveable
    {
        [SceneName]
        public string startSceneName = string.Empty;
        [SerializeField]
        private CanvasGroup fadeCanvasGroup;    //场景加载面板
        private bool isFade;

        public string GUID => GetComponent<DataGUID>().guid;

        protected override void Awake()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        }
        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;

        }
        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;
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
            yield return Fade(1);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());  //异步卸载场景

            yield return LoadSceneSetActive(sceneName); //携程异步加载场景并激活
            EventHandler.CallAfterSceneLoadEvent();
            EventHandler.CallMoveToPositionEvent(targetPosition);   //移动角色坐标
            yield return Fade(0);
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
        /// <summary>
        /// 加载面板缓入缓出
        /// </summary>
        /// <param name="targetAlpha"></param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;
            fadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha)/Settings.fadeDuration;
            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha,targetAlpha,speed*Time.deltaTime);
                yield return null;
            }

            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }

        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1f);

            if(SceneManager.GetActiveScene().name != "PersistentScene")
            {
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            //yield return SceneManager.LoadSceneAsync(sceneName);
            EventHandler.CallAfterSceneLoadEvent();
            yield return Fade(0f);
        }

        private IEnumerator UnloadScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0f);
        }


        private void OnTransitionEvent(string sceneName, Vector3 pos)
        {
            if (!isFade)
            {
                StartCoroutine(Transition(sceneName, pos));
            }
        }
        private void OnStartNewGameEvent(int obj)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }



        public GameSaveData GenerateSaveData()
        {
            GameSaveData data = new GameSaveData();
            data.dataSceneName = SceneManager.GetActiveScene().name;

            return data;
        }

        public void RestoreData(GameSaveData data)
        {
            StartCoroutine(LoadSaveDataScene(data.dataSceneName));
        }
    }
}