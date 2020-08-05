using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private GameObject LockPanel;

    [SerializeField]
    private SoundManager soundManager;

    public void HomeButton()
    {
        soundManager.PlaySE(SoundData.SE.Select);

        LockPanel.SetActive(true);
        StartCoroutine(GoHome());
    }

    IEnumerator GoHome()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("Home");
    }

}
