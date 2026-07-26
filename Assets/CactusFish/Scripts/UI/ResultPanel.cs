using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;




public class ResultPanel : UIBase
{
    [Header("文本")]
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public GameObject scoreGroup;   // 分数区域（暂停时隐藏）

    [Header("按钮")]
    public Button settingsButton;   // 设置
    public Button restartButton;    // 重玩
    public Button homeButton;       // 返回主页
    public Button quitButton;       // 退出页面


    protected override void OnOpen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    protected override void OnClose()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Show(GameState state, int score = 0)
    {
        switch (state)
        {
            case GameState.Paused:
                titleText.text = "Pause";
                if (scoreGroup != null) scoreGroup.SetActive(false);
                break;

            case GameState.GameOver:
                titleText.text = "Game Over";
                if (scoreGroup != null) scoreGroup.SetActive(true);
                if (scoreText != null) scoreText.text = score.ToString();
                break;

            case GameState.Victory:
                titleText.text = "Level Cleared";
                if (scoreGroup != null) scoreGroup.SetActive(true);
                if (scoreText != null) scoreText.text = score.ToString();
                break;
        }
    }

    // ========== 按钮点击事件（Inspector里绑定） ==========

    /// <summary>设置按钮</summary>
    public void OnSettingsClicked()
    {
        GameManager.Instance.UI.Open("Setting");
    }

    /// <summary>重玩按钮</summary>
    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.UI.ClearAll();
        GameManager.Instance.Scene.ReloadScene();
    }

    /// <summary>返回主页按钮</summary>
    public void OnHomeClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.UI.ClearAll();
        GameManager.Instance.Scene.LoadAsync("Home");
    }

    /// <summary>退出页面按钮</summary>
    public void OnQuitClicked()
    {
        if (GameManager.Instance.State == GameState.Paused)
            GameManager.Instance.ChangeState(GameState.Playing);
        GameManager.Instance.UI.Close(UIName);
    }
}


