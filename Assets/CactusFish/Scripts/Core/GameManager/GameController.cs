using UnityEngine;


public class GameController : MonoBehaviour
{
    [Header("UI预制体")]
    public GameObject resultPanelPrefab;
    public GameObject settingsPanelPrefab;

    [Header("场景引用")]
    public CountdownUI countdownUI;
    public StartTrigger startTrigger;
    public GoalTrigger goalTrigger;

    [Header("倒计时秒数")]
    public float countdownSeconds = 60f;

    void Start()
    {
        // 1. 注册UI（用框架现有的 Register）
        var ui = GameManager.Instance.UI;
        ui.Register("Result", resultPanelPrefab, UILayer.Popup);
        ui.Register("Settings", settingsPanelPrefab, UILayer.Popup);

        // 2. 监听业务层自定义事件
        EventManager.Subscribe<TimeUpEvent>(OnTimeUp);
        EventManager.Subscribe<VictoryEvent>(OnVictory);


        // 3. 绑定触发器
        if (startTrigger != null)
            startTrigger.OnLeaveStart = OnLeaveStart;
        if (goalTrigger != null)
            goalTrigger.OnReachGoal = OnReachGoal;

        // 4. 进入游戏状态（倒计时不在这启动，等玩家离开出生点才启动）
        countdownUI.StartCountdown(countdownSeconds);
    }

    void Update()
    {
        // ESC键：暂停/继续
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.State == GameState.Playing)
            {
                GameManager.Instance.ChangeState(GameState.Paused);
                ShowResult(GameState.Paused);
            }
            else if (GameManager.Instance.State == GameState.Paused)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
                GameManager.Instance.UI.Close("Result");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed, current state: " + GameManager.Instance.State);
        }
    }

    // ========== 事件回调 ==========

    /// <summary>倒计时结束 → 游戏结束</summary>
    void OnTimeUp(TimeUpEvent evt)
    {
        countdownUI.Hide();
        GameManager.Instance.ChangeState(GameState.GameOver);
        ShowResult(GameState.GameOver, GetScore());

    }

    /// <summary>通关事件回调（框架内置 VictoryEvent）</summary>
    void OnVictory(VictoryEvent evt)
    {
        countdownUI.StopCountdown();
        ShowResult(GameState.Victory, GetScore());
    }

    /// <summary>玩家到达终点</summary>
    void OnReachGoal()
    {
        // 切到 Victory 状态，框架会自动发 VictoryEvent
        GameManager.Instance.ChangeState(GameState.Victory);
    }

    /// <summary>玩家离开出生点 → 开始倒计时</summary>
    void OnLeaveStart()
    {
        countdownUI.StartCountdown(countdownSeconds);
    }

    // ========== 内部方法 ==========

    /// <summary>打开结果面板并设置状态</summary>
    void ShowResult(GameState state, int score = 0)
    {
        GameManager.Instance.UI.Open("Result");
        var panel = GameManager.Instance.UI.Get<ResultPanel>("Result");
        if (panel != null)
        {
            panel.Show(state, score);
        }
    }

    /// <summary>获取分数（替换成你自己的分数系统）</summary>
    int GetScore()
    {
        return 0;
    }

    void OnDestroy()
    {
        EventManager.Unscribe<TimeUpEvent>(OnTimeUp);
        EventManager.Unscribe<VictoryEvent>(OnVictory);
    }
}

/// <summary>
/// 终点触发器：玩家碰到后通知 GameController
/// 挂到终点的物体上，勾选IsTrigger
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    /// <summary>玩家到达终点时的回调（由GameController绑定）</summary>
    public System.Action OnReachGoal;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnReachGoal?.Invoke();
        }
    }
}

/// <summary>
/// 出生点触发器：玩家离开后通知 GameController 开始倒计时
/// 挂到出生点的物体上，勾选IsTrigger
/// </summary>
public class StartTrigger : MonoBehaviour
{
    /// <summary>玩家离开出生点时的回调（由GameController绑定）</summary>
    public System.Action OnLeaveStart;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLeaveStart?.Invoke();
        }
    }
}
