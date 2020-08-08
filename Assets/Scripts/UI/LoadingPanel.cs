using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField, Header("GemIcon")]
    private UnityEngine.UI.Image gemIcon = default;

    [SerializeField, Header("GemImageList")]
    private List<Sprite> gemList = default;

    private void OnEnable()
    {
        gemIcon.sprite = gemList[Random.Range(0, gemList.Count)];
    }
}
