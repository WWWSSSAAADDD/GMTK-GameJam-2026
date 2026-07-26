using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 预制体字典
    private readonly Dictionary<string, GameObject> _prefabs = new();

    // 实例字典：名字 → 实例
    private readonly Dictionary<string, GameObject> _instances = new();

    // 当前打开的UI栈（用于返回功能）
    private readonly Stack<string> _uiStack = new();

    // 各层级的根节点
    private readonly Dictionary<UILayer, Transform> _layerRoots = new();

    // 每个UI的层级配置
    private readonly Dictionary<string, UILayer> _uiLayers = new();

    void Awake()
    {
        // 创建Canvas
        var canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            var go = new GameObject("UIRoot");
            go.transform.SetParent(transform);
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<UnityEngine.UI.CanvasScaler>();
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // 为每个层级创建根节点
        foreach (UILayer layer in System.Enum.GetValues(typeof(UILayer)))
        {
            var layerGo = new GameObject($"Layer_{layer}");
            layerGo.transform.SetParent(canvas.transform, false);
            _layerRoots[layer] = layerGo.transform;
        }
    }

    /// <summary>注册UI预制体</summary>
    public void Register(string uiName, GameObject prefab, UILayer layer = UILayer.Normal)
    {
        _prefabs[uiName] = prefab;
        _uiLayers[uiName] = layer;
    }

    /// <summary>打开UI（入栈）</summary>
    public void Open(string uiName)
    {
        if (!_prefabs.ContainsKey(uiName))
        {
            Debug.LogWarning($"[UIManager] {uiName} 未注册");
            return;
        }

        // 获取或创建实例
        if (!_instances.TryGetValue(uiName, out var instance))
        {
            var layer = _uiLayers.GetValueOrDefault(uiName, UILayer.Normal);
            var parent = _layerRoots[layer];
            instance = Instantiate(_prefabs[uiName], parent);
            _instances[uiName] = instance;

            // 如果挂了UIBase，调用初始化
            var uiBaseInit = instance.GetComponent<UIBase>();
            if (uiBaseInit != null)
                uiBaseInit.Init(uiName, layer);
        }

        // 已打开则不重复
        var uiBase = instance.GetComponent<UIBase>();
        if (uiBase != null)
        {
            if (uiBase.IsOpen) return;
            uiBase.Open();
        }
        else
        {
            if (instance.activeSelf) return;
            instance.SetActive(true);
            EventManager.Publish(new OpenUI { UIName = uiName });
        }

        _uiStack.Push(uiName);
    }

    /// <summary>关闭当前UI（出栈，显示上一个）</summary>
    public void Close()
    {
        if (_uiStack.Count == 0) return;

        var uiName = _uiStack.Pop();
        CloseUIInternal(uiName);
    }

    /// <summary>关闭指定UI</summary>
    public void Close(string uiName)
    {
        if (!_instances.ContainsKey(uiName)) return;
        RemoveFromStack(uiName);
        CloseUIInternal(uiName);
    }

    // 内部关闭UI的统一逻辑
    private void CloseUIInternal(string uiName)
    {
        if (!_instances.TryGetValue(uiName, out var instance)) return;

        var uiBase = instance.GetComponent<UIBase>();
        if (uiBase != null)
        {
            if (!uiBase.IsOpen) return;
            uiBase.Close();
        }
        else
        {
            if (!instance.activeSelf) return;
            instance.SetActive(false);
            EventManager.Publish(new CloseUI { UIName = uiName });
        }
    }

    /// <summary>切换显隐</summary>
    public void Toggle(string uiName)
    {
        if (_instances.TryGetValue(uiName, out var instance))
        {
            var uiBase = instance.GetComponent<UIBase>();
            bool isOpen = uiBase != null ? uiBase.IsOpen : instance.activeSelf;
            if (isOpen)
                Close(uiName);
            else
                Open(uiName);
        }
        else
        {
            Open(uiName);
        }
    }

    /// <summary>获取UI实例</summary>
    public T Get<T>(string uiName) where T : Component
    {
        if (_instances.TryGetValue(uiName, out var instance))
            return instance.GetComponent<T>();
        return null;
    }

    /// <summary>关闭并销毁所有UI</summary>
    public void ClearAll()
    {
        foreach (var kv in _instances)
        {
            if (kv.Value != null) Destroy(kv.Value);
        }
        _instances.Clear();
        _uiStack.Clear();
    }

    // 从栈中移除指定UI（保持顺序）
    private void RemoveFromStack(string uiName)
    {
        var temp = new Stack<string>();
        while (_uiStack.Count > 0)
        {
            var name = _uiStack.Pop();
            if (name != uiName) temp.Push(name);
        }
        while (temp.Count > 0)
            _uiStack.Push(temp.Pop());
    }
}
