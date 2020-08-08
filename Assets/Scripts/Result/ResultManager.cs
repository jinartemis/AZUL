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

}
