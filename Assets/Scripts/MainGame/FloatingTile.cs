using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//フワフワ浮かせる
public class FloatingTile : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;

    Sequence seq;

    private void Awake()
    {
        rect = this.gameObject.GetComponent<RectTransform>();

        float rnd = Random.Range(.1f, .4f);
        float rotateRnd = Random.Range(5f, 10f);

        seq = DOTween.Sequence();
        seq.Append(rect.DOMove(new Vector3(0, rnd, 0) , rotateRnd)).SetRelative()
                        .Join(rect.DOLocalRotate(new Vector3(0, 0, 180f), rotateRnd)
                        .SetEase(Ease.Linear))
                        .Append(rect.DOMove(new Vector3(0, -rnd, 0), rotateRnd)).SetRelative()
                        .Join(rect.DOLocalRotate(new Vector3(0, 0, 180), rotateRnd)
                        .SetEase(Ease.Linear))
                        .SetLoops(-1);

    }

    public void DisableFloating()
    {
        seq.Kill();
    }


}
