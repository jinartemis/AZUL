using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShineButton : MonoBehaviour
{
    private ShinyEffectForUGUI shine;
    [SerializeField, Header("開始時間ディレイ")]
    private float delayTime = 0;
    private void Awake()
    {
        shine = this.GetComponent<ShinyEffectForUGUI>();
    }

    private void OnEnable()
    {
        if(shine == null)
        {
            shine = this.GetComponent<ShinyEffectForUGUI>();
        }
        StartCoroutine(FirstPlay());
    }

    IEnumerator FirstPlay()
    {
        yield return new WaitForSeconds(delayTime);
        shine.Play();
        delta = -delayTime;
    }

    float delta = 0;
    [SerializeField, Header("周期")]
    private float span = 1.0f;

    private void Update()
    {
        delta += Time.deltaTime;
        if(delta > span)
        {
            delta = 0;
            shine.Play();
        }
    }
}
