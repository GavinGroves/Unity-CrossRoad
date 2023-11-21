using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text score;
    private GameObject gameOverPanel;
    private GameObject leaderboardPanel;
    private Button restartBtn;
    private Button backBtn;
    private Button leaderBtn;

    private void OnEnable()
    {
        Time.timeScale = 1;
        EventHandler.GetPointEvent += OnGetPointEvent;
        EventHandler.GameOverEvent += GameOverEvent;
    }
    
    private void OnDisable()
    {
        EventHandler.GetPointEvent -= OnGetPointEvent;
        EventHandler.GameOverEvent -= GameOverEvent;
    }
    private void Start()
    {
        score = transform.Find("Score").GetComponent<Text>();
        score.text = "00";
        gameOverPanel = transform.Find("GameOverPanel").gameObject;
        leaderboardPanel = transform.Find("LeaderBoard").gameObject;
        
        restartBtn = transform.Find("GameOverPanel/Restart").GetComponent<Button>();
        restartBtn.onClick.AddListener(RestartGame);
        backBtn = transform.Find("GameOverPanel/Back").GetComponent<Button>();
        backBtn.onClick.AddListener(BackToMenu);
        leaderBtn = transform.Find("GameOverPanel/Leaderboar").GetComponent<Button>();
        leaderBtn.onClick.AddListener(OpenLeaderBoard);
    }
    
    private void GameOverEvent()
    {
        gameOverPanel.SetActive(true);
        if (gameOverPanel.activeInHierarchy)
        {
            //暂停游戏
            Time.timeScale = 0;
        }
    }

    private void OnGetPointEvent(int point)
    {
        score.text = point.ToString();
    }

    #region 按钮添加方法
    private void RestartGame()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AdsManager.instance.ShowInterAds();
        gameOverPanel.SetActive(false);
        // TransitionManager.instance.Transition("GamePlay");
    }

    private void BackToMenu()
    {
        gameOverPanel.SetActive(false);
        TransitionManager.instance.Transition("Title");
    }

    public void OpenLeaderBoard()
    {
        leaderboardPanel.SetActive(true);
    }
    #endregion

}
