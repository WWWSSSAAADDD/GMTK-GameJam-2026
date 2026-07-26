using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool _isLoading = false;


    public void LoadAsync(string sceneName, System.Action onComplete = null, bool autoSwitch = true)
    {
        if (_isLoading)
        {
            Debug.LogWarning("[SceneLoader] 正在加载中，请勿重复调用");
            return;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete, autoSwitch));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, System.Action onComplete, bool autoSwitch)
    {
        _isLoading = true;

        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = autoSwitch;

        // 广播进度（Unity的进度最大到0.9，切换后才是1.0）
        while (operation.progress < 0.9f)
        {
            EventManager.Publish(new SceneLoadProgressEvent
            {
                Progress = operation.progress / 0.9f
            });
            yield return null;
        }

        EventManager.Publish(new SceneLoadProgressEvent { Progress = 1f });

        if (!autoSwitch)
        {
            yield return new WaitForSecondsRealtime(0.1f);  // 用Realtime版本，不受timeScale影响
            operation.allowSceneActivation = true;
        }

        yield return operation;

        EventManager.Publish(new SceneLoadCompleteEvent { SceneName = sceneName });

        _isLoading = false;
        onComplete?.Invoke();
    }

    /// <summary>获取当前场景名</summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void ReloadScene(System.Action onComplete = null)
    {
        string current = GetCurrentSceneName();
        LoadAsync(current, onComplete);
    }
}
