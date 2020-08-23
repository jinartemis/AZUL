using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private GameObject LockPanel;

    public void HomeButton()
    {
        SoundManager.instance.PlaySE(SoundData.SE.Select);

        LockPanel.SetActive(true);
        StartCoroutine(GoHome());
    }

    IEnumerator GoHome()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Home");
    }

    public void JumpToWeb()
    {
#if UNITY_ANDROID
            string url = "";
            if( Application.systemLanguage == SystemLanguage.Japanese)
            {
                url = "https://play.google.com/store/apps/developer?id=Reo+Games&hl=jp_JP";
            }
            else
            {
			    url = "https://play.google.com/store/apps/developer?id=Reo+Games&hl=en_US";
            }
			Application.OpenURL(url);
#elif UNITY_IPHONE
        string url = "https://itunes.apple.com/jp/developer/reo-komura/id1353284945";
        Application.OpenURL(url);
#else
#endif
    }
}
