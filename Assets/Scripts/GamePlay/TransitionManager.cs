using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;
    private CanvasGroup _canvasGroup;
    [SerializeField] private float scaler;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        StartCoroutine(Fade(0));
    }

    public void Transition(string sceneName)
    {
        Time.timeScale = 1;
        StartCoroutine(TransitionToScene(sceneName));
    }

    /// <summary>
    /// 执行变黑→加载场景→透明
    /// </summary>
    /// <param name="sceneName">场景名字</param>
    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return Fade(1);
        //在后台异步加载场景
        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return Fade(0);
    }

    /// <summary>
    /// 渐变
    /// </summary>
    /// <param name="amount">1是黑，0是透明</param>
    private IEnumerator Fade(int amount)
    {
        _canvasGroup.blocksRaycasts = true;
        while (_canvasGroup.alpha != amount)
        {
            switch (amount)
            {
                case 1:
                    _canvasGroup.alpha += Time.deltaTime * scaler;
                    break;
                case 0:
                    _canvasGroup.alpha -= Time.deltaTime * scaler;
                    break;
            }

            yield return null;
        }

        _canvasGroup.blocksRaycasts = false;
    }
}