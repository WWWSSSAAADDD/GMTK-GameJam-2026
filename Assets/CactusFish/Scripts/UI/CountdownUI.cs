using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CountdownUI : MonoBehaviour
{
    public TMP_Text countdownText;

    private float _timeLeft;
    private bool _isRunning;

    void Update()
    {
        if (!_isRunning) return;

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            _isRunning = false;
            countdownText.text = "0";
            EventManager.Publish(new TimeUpEvent());
        }
        else
        {
            countdownText.text = Mathf.CeilToInt(_timeLeft).ToString();
        }
    }

    /// <summary>开始倒计时</summary>
    public void StartCountdown(float seconds)
    {
        _timeLeft = seconds;
        _isRunning = true;
        gameObject.SetActive(true);
    }

    /// <summary>停止倒计时（不触发事件）</summary>
    public void StopCountdown()
    {
        _isRunning = false;
    }

    /// <summary>隐藏倒计时UI</summary>
    public void Hide()
    {
        _isRunning = false;
        gameObject.SetActive(false);
    }

    /// <summary>获取剩余时间</summary>
    public float GetTimeLeft()
    {
        return _timeLeft;
    }
}
