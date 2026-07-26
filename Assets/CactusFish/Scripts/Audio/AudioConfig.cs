using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Data/Audiofig")]
public class AudioConfig : ScriptableObject
{
    [Header("背景音乐")]
    public List<AudioEnTry> bgms = new();
    [Header("音效")]
    public List<AudioEnTry> sfx = new();

    private Dictionary<string, AudioClip> _bgmDic;
    private Dictionary<string, AudioClip> _sfxDic;

    public void Init()
    {
        _bgmDic = new Dictionary<string, AudioClip>();
        foreach (var item in bgms)
        {
            if (item.clip != null)
            {
                _bgmDic[item.name] = item.clip;
            }
        }
        foreach (var item in sfx)
        {
            if (item.clip != null)
            {
                _sfxDic[item.name] = item.clip;
            }
        }
    }

    public AudioClip GetBGM(string name)
    {
        EnsureInit();
        _bgmDic.TryGetValue(name, out var clip);
        return clip;
    }
    public AudioClip GetSFX(string name)
    {
        EnsureInit();
        _sfxDic.TryGetValue(name, out var clip);
        return clip;
    }
    private void EnsureInit()
    {
        if (_bgmDic == null) Init();
    }
}

public struct AudioEnTry
{
    public string name;
    public AudioClip clip;
}