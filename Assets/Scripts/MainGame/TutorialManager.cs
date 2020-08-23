using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField, Header("チュートリアルパネル")]
    private GameObject tutorialPanel = default;

    [SerializeField, Header("チュートリアル画像パネル")]
    private List<GameObject> tutorialImagePanel = default;

    int nowImageNumber = 0;

    [SerializeField, Header("ネクストボタンラベル")]
    private Text nextButtonLabel = default;

    public void NextButton()
    {
        if(nowImageNumber < tutorialImagePanel.Count-1)
        {
            nowImageNumber++;
            if(nowImageNumber < tutorialImagePanel.Count - 1) { nextButtonLabel.text = (Application.systemLanguage == SystemLanguage.Japanese) ? "次へ" :"Next"; }
            else { nextButtonLabel.text = (Application.systemLanguage == SystemLanguage.Japanese) ? "閉じる" : "Close"; }
            ChangeImagePanel();
        }
        else
        {
            GameManager.instance.ShowTutorialPanel(false);
            SoundManager.instance.PlaySE(SoundData.SE.Cansel);
        }
    }

    public  void BackButton()
    {
        if(nowImageNumber-1 >=  0)
        {
            nowImageNumber--;
            ChangeImagePanel();
        }
        else
        {
            SoundManager.instance.PlaySE(SoundData.SE.Error);
        }
    }

    private void ChangeImagePanel()
    {
        SoundManager.instance.PlaySE(SoundData.SE.Select);
        for (int i = 0; i < tutorialImagePanel.Count; i++)
        {
            tutorialImagePanel[i].SetActive(i == nowImageNumber);
        }
    }
}
