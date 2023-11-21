using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;
    public List<int> scoreList;
    public List<string> nameList;
    public string playerName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
        Login();
    }

    #region 登陆信息

    private void Login()
    {
        //选择登录方式
        // var request = new LoginWithCustomIDRequest
        //     {   //设备登录指定ID；获得当前设备ID
        //         CustomId = SystemInfo.deviceUniqueIdentifier,
        //         //使用当前ID创建账号
        //         CreateAccount = true 
        //     };
        var req = new LoginWithCustomIDRequest();
        req.CustomId = SystemInfo.deviceUniqueIdentifier;
        req.CreateAccount = true;
        //请求当前数据
        req.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
        {
            GetPlayerProfile = true
        };
            
        //调用API登录
        PlayFabClientAPI.LoginWithCustomID(req, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult obj)
    {
        Debug.Log("Successful login/create account!");
        if (obj.InfoResultPayload.PlayerProfile != null)
        {
            playerName = obj.InfoResultPayload.PlayerProfile.DisplayName;
        }
    }

    #endregion

    private void OnLoginFailure(PlayFabError obj)
    {
        Debug.Log("Error!");
        //生成错误报告
        Debug.Log(obj.GenerateErrorReport());
    }

    #region 排行榜请求

    /// <summary>
    /// 发送排行榜信息(发布数据请求
    /// </summary>
    /// <param name="score">分数</param>
    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest();
        request.Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate()
            {
                StatisticName = "HighScores",
                Value = score
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnLoginFailure);
    }

    private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult obj)
    {
        Debug.Log("LeaderBoard Updated!");
        //上传成功后获得排行榜信息
        GetLeaderBoardData();
    }

    public void GetLeaderBoardData()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScores",
            StartPosition = 0,
            MaxResultsCount = 7
        };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderBoard, OnLoginFailure);
    }

    private void OnGetLeaderBoard(GetLeaderboardResult obj)
    {
        scoreList = new List<int>();
        nameList = new List<string>();
        foreach (var item in obj.Leaderboard)
        {
            Debug.Log(item.Position + "   " + item.DisplayName + "   " + item.StatValue);
            scoreList.Add(item.StatValue);
            nameList.Add(item.DisplayName);
        }
    }

    #endregion

    /// <summary>
    /// 更新服务器名字
    /// </summary>
    /// <param name="name">名字</param>
    public void SubmitName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,OnDisplayNameUpdate,OnLoginFailure);
    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult obj)
    {
        playerName = obj.DisplayName;
        Debug.Log("UpdateUserTitleDisplayName");
    }
}