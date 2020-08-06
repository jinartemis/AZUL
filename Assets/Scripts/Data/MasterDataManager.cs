using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//マスターデータ管理用スクリプト
public class MasterDataManager : MonoBehaviour
{
    const string STAGE_DATA_FOLDER_PATH = "Datas/StageDatas";
    [SerializeField]
    private AllStageData allStageData;


    [SerializeField, Header("タイル画像 Define.TileTypeに対応")]
    public Sprite[] tileImage;

    private void Awake()
    {
        var allData = Resources.LoadAll<StageData>(STAGE_DATA_FOLDER_PATH);
        allStageData.SetStageDatas(allData);
    }

    public int GetAllStageCount()
    {
        return allStageData.GetAllStageDataCount();
    }

    public StageData GetStageData(int num)
    {
        Debug.Log("ステージデータ取得" + num);
        
        return allStageData.GetStageData(num);
    }

}
