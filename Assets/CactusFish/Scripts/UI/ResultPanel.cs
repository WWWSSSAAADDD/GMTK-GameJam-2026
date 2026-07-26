using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPanel : UIBase
{
    [Header("文本")]
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public GameObject scoreGroup;

    [Header("按钮")]
    public Button settingsButton;
    public Button restartButton;
    public Button homeButton;
    public Button quitButton;
    public TMP_Text homeButtonText;     // 按钮上的文字，Victory时自动切"下一关"

    [Header("下一关")]
    public string nextLevelName = "Level2";

    private GameState _currentState;

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
        _currentState = state;

        switch (state)
        {
            case GameState.Paused:
                titleText.text = "Pause";
                if (scoreGroup != null) scoreGroup.SetActive(false);
                if (homeButtonText != null) homeButtonText.text = "返回主页";
                break;

            case GameState.GameOver:
                titleText.text = "Game Over";
                if (scoreGroup != null) scoreGroup.SetActive(true);
                if (scoreText != null) scoreText.text = score.ToString();
                if (homeButtonText != null) homeButtonText.text = "返回主页";
                break;

            case GameState.Victory:
                titleText.text = "Level Cleared";
                if (scoreGroup != null) scoreGroup.SetActive(true);
                if (scoreText != null) scoreText.text = score.ToString();
                if (homeButtonText != null) homeButtonText.text = "下一关";
                break;
        }
    }

    // ========== 按钮点击事件 ==========

    public void OnSettingsClicked()
    {
        GameManager.Instance.UI.Open("Setting");
        // 确保设置页在最上层
        var settings = GameManager.Instance.UI.Get<SettingsPanel>("Setting");
        if (settings != null)
            settings.transform.SetAsLastSibling();
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.UI.ClearAll();
        GameManager.Instance.Scene.ReloadScene();
    }

    public void OnHomeClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.UI.ClearAll();

        if (_currentState == GameState.Victory)
            GameManager.Instance.Scene.ReloadScene();  // TODO: 第二关做好后改成 LoadAsync(nextLevelName)
        else
            GameManager.Instance.Scene.LoadAsync("Home");
    }

    public void OnQuitClicked()
    {
        if (GameManager.Instance.State == GameState.Paused)
            GameManager.Instance.ChangeState(GameState.Playing);
        GameManager.Instance.UI.Close(UIName);
    }
}
