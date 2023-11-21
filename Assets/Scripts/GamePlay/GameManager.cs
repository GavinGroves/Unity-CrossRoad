using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<int> scoreList;
    private int tempScore;
    private string dataPath; //路径
    public bool isGlobal; //切换全球与本地排行榜

    private void Awake()
    {
        dataPath = Application.persistentDataPath + "/leaderboard.json";
        // scoreList = GetScoreListData();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        EventHandler.GameOverEvent += OnGameOverEvent;
        EventHandler.GetPointEvent += OnGetPointEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameOverEvent -= OnGameOverEvent;
        EventHandler.GetPointEvent -= OnGetPointEvent;
    }

    private void Start()
    {
        scoreList = GetScoreListData();
    }

    private void OnGetPointEvent(int point)
    {
        tempScore = point;
    }

    private void OnGameOverEvent()
    {
        if (!isGlobal)
        {
            //在list里添加新的分数，排序
            if (!scoreList.Contains(tempScore))
            {
                scoreList.Add(tempScore);
            }

            //从小到大排序
            scoreList.Sort();
            //然后反过来(最下面的在上也就是变成从大到小
            scoreList.Reverse();

            File.WriteAllText(dataPath, JsonConvert.SerializeObject(scoreList));
        }
        else
        {
            //发送分数Playfab
            PlayfabManager.instance.SendLeaderBoard(tempScore);
        }
    }

    /// <summary>
    /// 读取保存数据的记录
    /// </summary>
    /// <returns></returns>
    public List<int> GetScoreListData()
    {
        if (!isGlobal)
        {
            if (File.Exists(dataPath))
            {
                string jsonData = File.ReadAllText(dataPath);
                return JsonConvert.DeserializeObject<List<int>>(jsonData);
            }

            return new List<int>();
        }
        else
        {
            return PlayfabManager.instance.scoreList;
        }
    }
}