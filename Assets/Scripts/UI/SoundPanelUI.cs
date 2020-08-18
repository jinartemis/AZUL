using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanelUI : MonoBehaviour
{
    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private Slider seSlider;

    public void ChangeBGMValue()
    {
        SoundManager.instance.SetVolume(SoundManager.Kind.BGM, bgmSlider.value);
    }

    public void ChangeSEValue()
    {
        SoundManager.instance.SetVolume(SoundManager.Kind.SE, seSlider.value);
    }
}
