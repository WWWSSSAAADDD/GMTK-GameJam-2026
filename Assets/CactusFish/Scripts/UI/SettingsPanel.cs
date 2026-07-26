using UnityEngine;
using UnityEngine.UI;


public class SettingsPanel : UIBase
{
    [Header("音量滑条")]
    public Slider bgmSlider;
    public Slider sfxSlider;


    private void OnEnable()
    {
        OnOpen();
    }

    //private void Update()
    //{
    //    GameManager.Instance.Audio.bgmVolume = bgmSlider.value;
    //    GameManager.Instance.Audio.sfxVolume = sfxSlider.value;
    //}
    protected override void OnOpen()
    {
        // 打开时同步当前音量到滑条
        var audio = GameManager.Instance.Audio;
        bgmSlider.value = audio.bgmVolume;
        sfxSlider.value = audio.sfxVolume;
    }

    /// <summary>BGM音量改变（Slider绑定）</summary>
    public void OnBGMVolumeChanged(float value)
    {
        GameManager.Instance.Audio.SetBGMVolume(value);
    }

    /// <summary>SFX音量改变（Slider绑定）</summary>
    public void OnSFXVolumeChanged(float value)
    {
        GameManager.Instance.Audio.SetSFXVolume(bgmSlider.value);
    }

    /// <summary>返回按钮</summary>
    public void OnBackClicked()
    {
        GameManager.Instance.UI.Close(UIName);
    }
}

