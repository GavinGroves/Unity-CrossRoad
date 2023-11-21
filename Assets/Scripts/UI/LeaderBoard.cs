using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public List<ScoreRecord> scoreRecords;
    private List<int> scoreList;
    private List<string> nameList;
    private Button backBtn;
    private Button resetBtn;

    private void OnEnable()
    {
        scoreList = GameManager.instance.GetScoreListData();
        backBtn = transform.Find("Board/BackButton").GetComponent<Button>();
        resetBtn = transform.Find("Board/ResetButton").GetComponent<Button>();
        backBtn.onClick.AddListener(BackToMenu);
        resetBtn.onClick.AddListener(ResetGame);
        PlayfabManager.instance.GetLeaderBoardData();
        SetLeaderBoardData();
    }

    public void SetLeaderBoardData()
    {
        nameList = PlayfabManager.instance.nameList;
        for (int i = 0; i < scoreRecords.Count; i++)
        {
            if (i < scoreList.Count)
            {
                scoreRecords[i].SetScoreText(scoreList[i]);
                //设置名字
                scoreRecords[i].SetName(nameList[i]);
                scoreRecords[i].gameObject.SetActive(true);
            }
            else
            {
                scoreRecords[i].gameObject.SetActive(false);
            }
        }
    }

    private void ResetGame()
    {
        AdsManager.instance.ShowRewardAds();
        // gameObject.SetActive(false);
        // SendMessageUpwards("RestartGame");
    }

    private void BackToMenu()
    {
        gameObject.SetActive(false);
    }
}