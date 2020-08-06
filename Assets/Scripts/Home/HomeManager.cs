using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
        //広告表示
        AdManager.instance.RequestBanner(true);
#endif

        //BGM
        SoundManager.instance.PlayBGM(SoundData.BGM.Home);
     }
}
