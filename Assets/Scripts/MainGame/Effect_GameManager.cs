using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.GameObject;
using DG.Tweening;
using UnityEngine.UI;

public partial class GameManager
{
    private List<Text> chainLabelList = new List<Text>();
    private List<List<GameObject>> completeEffect = new List<List<GameObject>>();
    private static readonly string CHAIN_FORMAT_TEXT = "{0}Chain !!";

    private static readonly float effectShowingDelayRate = .1f;
    private static readonly float chainEffectMoveSpan = .5f;
    private static readonly float chainEffectShowingSpan = .5f;
    private static readonly float chainEffectExpandRate = 1.5f;
    private Vector3 chainLabelMoveDistance = new Vector3(0, .5f, 0);

    private void MakeEffects()
    {
        LoadChainLabels();
        MakeCompleteLineEffect();
    }

    private void LoadChainLabels()
    {
        chainLabelList.Clear();
        Transform chainTextParent = Find(Reo.HierarchyPath_Game.GameCanvas._ChainTextParent).transform;
        foreach (Transform item in chainTextParent.transform)
        {
            chainLabelList.Add(item.GetComponent<Text>());
            item.gameObject.SetActive(false);
        }
    }

    //ラインが完成した時のエフェクトをキャッシュ
    private void MakeCompleteLineEffect()
    {
        for (int x = 0; x < 4; x++)
        {
            var list = new List<GameObject>();
            for (int y = 0; y < 4; y++)
            {
                var ef = Instantiate(fitEffect, tileInfo[x, y].obj.transform);
                ef.SetActive(false);
                list.Add(ef);
            }
            completeEffect.Add(list);
        }
    }

    IEnumerator PlayCompleteEffect(int line, float delay, int effectCount)
    {
        Debug.Log("Playcompleteeffect " + line);
        delay *= effectShowingDelayRate;
        yield return new WaitForSeconds(delay);

        //エフェクト表示
        var efList = completeEffect.Where((item, index) => index == line).FirstOrDefault().ToList();
        foreach (var ef in efList)
        {
            ef.SetActive(false);
            ef.SetActive(true);
        }
        //チェインエフェクト
        StartCoroutine(PlayChainEffect(efList.FirstOrDefault().transform.position, effectCount));

        yield return new WaitForSeconds(2.0f);
        //エフェクトを再度非表示に
        foreach (var ef in efList)
        {
            ef.SetActive(false);
        }
    }

    IEnumerator PlayChainEffect(Vector3 pos, int line)
    {
        //０はチェインテキスト表示しない
        if(line == 0)
        {
            yield break;
        }
        var ef = chainLabelList.Where(item => item.gameObject.activeSelf == false).FirstOrDefault();
        ef.text = string.Format(CHAIN_FORMAT_TEXT, line+1);
        ef.transform.position = pos;
        ef.gameObject.SetActive(true);
        var endPos = pos + chainLabelMoveDistance;
        Sequence seq = DOTween.Sequence()
            .Append(ef.transform.DOScale(Vector3.one * chainEffectExpandRate, chainEffectMoveSpan))
            .Join(ef.transform.DOMove(endPos, chainEffectMoveSpan))
            .AppendInterval(chainEffectShowingSpan)
            .OnComplete(()=> {
                ef.gameObject.SetActive(false);
                ef.transform.localScale = Vector3.one;
            });
        seq.Play();
        yield break;
    }
}
