using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State { get; private set; }
    public UIManager UI { get; private set; }
    public AudioManager Audio { get; private set; }
    public SceneLoader Scene { get; private set; }

    public EventSystem eventSystem { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            UI = GetOrCreate<UIManager>();
            Audio = GetOrCreate<AudioManager>();
            Scene = GetOrCreate<SceneLoader>();
            eventSystem = GetOrCreate<EventSystem>();
            Audio.PlayBGM("HJM");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(Instance);
        }

    }

    //获取或添加组件
    public T GetOrCreate<T>() where T : Component
    {
        var comp = GetComponentInChildren<T>();
        if (comp == null)
        {
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(transform);
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;
        var oldState = State;
        State = newState;

        switch (State)
        {
            case GameState.Paused:
                Time.timeScale = 0;
                EventManager.Publish(new GamePause());
                break;
            case GameState.Playing:
                Time.timeScale = 1;
                if (oldState == GameState.Paused)
                {
                    EventManager.Publish(new GameResume());
                }
                else
                {
                    EventManager.Publish(new GameStart());
                }
                break;
            case GameState.Victory:
                Time.timeScale = 0f;
                EventManager.Publish(new VictoryEvent());
                break;
            case GameState.GameOver:
                EventManager.Publish(new GameOver());
                break;
        }
        Debug.Log($"[GameManager] 状态切换: {oldState} → {newState}");
    }
    void OnApplicationQuit()
    {
        ChangeState(GameState.GameOver);
        Instance = null;
    }
}
