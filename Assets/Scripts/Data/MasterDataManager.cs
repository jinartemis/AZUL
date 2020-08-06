using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


//マスターデータ管理用スクリプト
public class MasterDataManager : MonoBehaviour
{
    public static MasterDataManager instance;

    const string STAGE_DATA_FOLDER_PATH = "Datas/StageDatas";
    [SerializeField]
    private AllStageData allStageData;

    [System.Serializable]
    public struct TileSkin
    {
        public Sprite[] tileImages;
    }
    [SerializeField, Header("タイル画像 Define.TileTypeに対応")]
    private List<TileSkin> tileImageList;

    [SerializeField, Header("選択スキン番号")]
    private int skinNumber = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        var allData = Resources.LoadAll<StageData>(STAGE_DATA_FOLDER_PATH);
        allStageData.SetStageDatas(allData);
        LoadTileSkin();
    }

    private void LoadTileSkin()
    {
        Debug.LogError("後で動的にスキンをロードできるように修正する");
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

    public Sprite[] GetTileImages()
    {
        return tileImageList.ElementAt(skinNumber).tileImages;
    }
}
