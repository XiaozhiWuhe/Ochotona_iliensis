using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    void Awake()
    {
        // 单例模式，全局只一个，切换场景不销毁
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 两个音频源：一个播音乐（循环），一个播音效
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.6f; 

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 1.0f;
    }

    // 播放背景音乐
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // 播放音效
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}