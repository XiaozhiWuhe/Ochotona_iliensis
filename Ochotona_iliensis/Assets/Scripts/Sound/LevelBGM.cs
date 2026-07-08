using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBGM : MonoBehaviour
{
    public AudioClip bgmClip; // 拖入对应关卡的 BGM

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmClip);
        }
    }
}
