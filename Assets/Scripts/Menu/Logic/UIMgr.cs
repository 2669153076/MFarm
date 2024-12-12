using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : Singleton<UIMgr>
{
    private GameObject menuCanvas;

    public GameObject menuPrefab;

    public Button settingBtn;
    public GameObject pausePanel;
    public Slider musicSlider;
    public Slider soundSlider;


    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }
    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, menuCanvas.transform);
        settingBtn.onClick.AddListener(TogglePausePanel);
        musicSlider.onValueChanged.AddListener(AudioMgr.Instance.SetMasterVolume);
        soundSlider.onValueChanged.AddListener(AudioMgr.Instance.SetEffectVolume);
    }


    private void OnAfterSceneLoadEvent()
    {
        if (menuCanvas.transform.childCount > 0)
        {
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
        }
    }

    private void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;

        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            System.GC.Collect();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        EventHandler.CallEndGameEvent();
        yield return new WaitForSeconds(1f);
        Instantiate(menuPrefab, menuCanvas.transform);
    }
}
