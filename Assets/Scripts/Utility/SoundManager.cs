using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    private AudioSource bgmAudio;

    [SerializeField]
    private AudioSource seAudio;

    [SerializeField, Header("スクリプタブルオブジェクト")]
    private SoundData clip;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public void PlaySE(SoundData.SE s)
    {
        seAudio.PlayOneShot(clip.se[(int)s]);
    }



    public void PlayBGM(SoundData.BGM b)
    {
        bgmAudio.clip = clip.bgm[(int)b];
        bgmAudio.Play();
    }
}
