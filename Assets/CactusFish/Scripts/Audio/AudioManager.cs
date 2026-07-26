using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _bgmSource;
    private List<AudioSource> _sfxPool;

    [Header("配置表（可选，不填也能用）")]
    public AudioConfig config;

    [Header("音量设置")]
    [Range(0, 1)] public float bgmVolume = 0.5f;
    [Range(0, 1)] public float sfxVolume = 1f;

    [Header("音效播放器数量")]
    public int sfxSourceCount = 5;

    // 当前BGM淡入淡出协程
    private Coroutine _bgmFadeCoroutine;

    void Awake()
    {
        // BGM播放器
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.volume = 0f;

        // SFX播放器池
        _sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxSourceCount; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.loop = false;
            src.volume = sfxVolume;
            _sfxPool.Add(src);
        }

        // 自动加载配置表：如果Inspector没拖，从Resources自动加载
        if (config == null)
            config = Resources.Load<AudioConfig>("AudioConfig");
    }

    /// <summary>播放BGM（用配置表的名字）</summary>
    public void PlayBGM(string bgmName, float fadeDuration = 0.5f)
    {
        if (config == null)
        {
            Debug.LogWarning("[AudioManager] 没有配置AudioConfig");
            return;
        }
        var clip = config.GetBGM(bgmName);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] 找不到BGM: {bgmName}");
            return;
        }
        PlayBGM(clip, fadeDuration);
    }

    /// <summary>播放BGM（带淡入）</summary>
    public void PlayBGM(AudioClip clip, float fadeDuration = 0.5f)
    {
        if (clip == null) return;

        if (_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        _bgmFadeCoroutine = StartCoroutine(BGMFadeIn(clip, fadeDuration));
    }

    /// <summary>停止BGM（带淡出）</summary>
    public void StopBGM(float fadeDuration = 0.5f)
    {
        if (_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        _bgmFadeCoroutine = StartCoroutine(BGMFadeOut(fadeDuration));
    }

    /// <summary>播放音效（用配置表的名字）</summary>
    public void PlaySFX(string sfxName, float volumeScale = 1f)
    {
        if (config == null)
        {
            Debug.LogWarning("[AudioManager] 没有配置AudioConfig");
            return;
        }
        var clip = config.GetSFX(sfxName);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] 找不到SFX: {sfxName}");
            return;
        }
        PlaySFX(clip, volumeScale);
    }

    /// <summary>播放音效</summary>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        foreach (var src in _sfxPool)
        {
            if (!src.isPlaying)
            {
                src.volume = sfxVolume * volumeScale;
                src.PlayOneShot(clip);
                return;
            }
        }

        _sfxPool[0].volume = sfxVolume * volumeScale;
        _sfxPool[0].PlayOneShot(clip);
    }

    /// <summary>设置BGM音量</summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (_bgmFadeCoroutine == null)
            _bgmSource.volume = bgmVolume;
    }

    /// <summary>设置SFX音量</summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    // BGM淡入协程
    private IEnumerator BGMFadeIn(AudioClip clip, float duration)
    {
        if (duration <= 0f)
        {
            _bgmSource.clip = clip;
            _bgmSource.volume = bgmVolume;
            _bgmSource.Play();
            yield break;
        }

        if (_bgmSource.clip != clip)
        {
            if (_bgmSource.isPlaying)
                yield return BGMFadeTo(0f, duration * 0.5f);

            _bgmSource.clip = clip;
            _bgmSource.Play();
        }

        yield return BGMFadeTo(bgmVolume, duration);
        _bgmFadeCoroutine = null;
    }

    // BGM淡出协程
    private IEnumerator BGMFadeOut(float duration)
    {
        if (duration <= 0f)
        {
            _bgmSource.Stop();
            _bgmSource.volume = 0f;
            yield break;
        }

        yield return BGMFadeTo(0f, duration);
        _bgmSource.Stop();
        _bgmFadeCoroutine = null;
    }

    // 音量渐变到目标值
    private IEnumerator BGMFadeTo(float targetVolume, float duration)
    {
        float startVolume = _bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;  // 用unscaledDeltaTime，暂停时也能淡入淡出
            _bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        _bgmSource.volume = targetVolume;
    }
}
