using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private Button playBtn;
    public GameObject nameInputPanel;
    public Button confirmButton;
    public InputField inputField;
    void Awake()
    {
        playBtn = GetComponent<Button>();
        playBtn.onClick.AddListener(OnGamePlay);
        confirmButton.onClick.AddListener(ConfirmName);
    }

    /// <summary>
    /// 加载游戏场景
    /// </summary>
    private void OnGamePlay()
    {
        // SceneManager.LoadScene("Scenes/GamePlay");
        if (PlayfabManager.instance.playerName == String.Empty)
        {
            nameInputPanel.SetActive(true);
        }
        else
        {
            nameInputPanel.SetActive(false);
            TransitionManager.instance.Transition("Scenes/GamePlay");
        }
    }

    private void ConfirmName()
    {
        PlayfabManager.instance.SubmitName(inputField.text);
        nameInputPanel.SetActive(false);
    }
}
