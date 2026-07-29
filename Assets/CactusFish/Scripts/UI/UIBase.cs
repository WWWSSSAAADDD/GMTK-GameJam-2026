using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    /// <summary>UI名称（自动取GameObject名）</summary>
    public string UIName { get; private set; }

    /// <summary>当前层级</summary>
    public UILayer Layer { get; private set; }

    /// <summary>是否已打开</summary>
    public bool IsOpen { get; private set; }

    /// <summary>初始化（由UIManager调用）</summary>
    public void Init(string uiName, UILayer layer)
    {
        UIName = uiName;
        Layer = layer;
        OnInit();
    }

    /// <summary>打开UI时调用</summary>
    public void Open()
    {
        IsOpen = true;
        gameObject.SetActive(true);
        OnOpen();
        EventManager.Publish(new OpenUI() { UIName = UIName });
    }

    /// <summary>关闭UI时调用</summary>
    public void Close()
    {
        IsOpen = false;
        OnClose();
        gameObject.SetActive(false);
    }

    // ========== 子类可重写的生命周期 ==========

    /// <summary>初始化时调用（只调一次）</summary>
    protected virtual void OnInit() { }

    /// <summary>每次打开时调用</summary>
    protected virtual void OnOpen() { }

    /// <summary>每次关闭时调用</summary>
    protected virtual void OnClose() { }
}
