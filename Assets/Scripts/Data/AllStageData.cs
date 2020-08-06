using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AllStageData : ScriptableObject
{
    [SerializeField, Header("ステージデータ")]
    private StageData[] stageData;

    public int GetAllStageDataCount()
    {
        return stageData.Length;
    }

    public void SetStageDatas(StageData[] allData)
    {
        stageData = new StageData[allData.Length];
        stageData = allData;
    }

    public StageData GetStageData(int num)
    {
        Debug.Log(num);
        return stageData[num];
    }
}
