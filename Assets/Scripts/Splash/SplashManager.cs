using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashManager : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void Start()
    {
        StartCoroutine(GoHomeScene());
    }

    //ステージデータを取得する
    private void LoadStageData()
    {
      //後でできればURLからステージデータ取得できるようにしてみたい．
    }

    IEnumerator GoHomeScene()
    {
        yield return null;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}
