using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [SerializeField, Header("ステージ情報 実行時取得")]
    private StageData stageData;

    [SerializeField, Header("プレイ中のゲームデータ　実行時取得")]
    private GameData gameData;

    [SerializeField, Header("ステージ　マスターデータ")]
    private MasterDataManager masterDataManager;

    [SerializeField, Header("プールタイルの親オブジェクト")]
    private GameObject poolTileParent;

    [SerializeField, Header("プールタイルプレファブ")]
    private GameObject poolTilePrefab;

    [SerializeField, Header("プールのタイルのポジション　オブジェクト")]
    private GameObject[] poolTileImagePos;

    [SerializeField, Header("プールタイル情報")]
    private Define.Tile[] poolTilesInfo;

    [SerializeField, Header("プールのタイル画像")]
    private Image[] poolTileImages;

    [SerializeField, Header("ネクストタイル")]
    private Image[] nextTileImages;

    [SerializeField, Header("タイルレーンシート")]
    private GameObject[] lanes;

    [SerializeField, Header("レーンポイントイメージ画像")]
    private Image[] lanePointImage;

    [SerializeField, Header("レーンポイントテキスト")]
    private Text[] lanePoint;

    [SerializeField, Header("レーンが完成しているかどうか")]
    private bool[] laneCompleted;

    [SerializeField, Header("タイルがハマった時に表示するエフェクトプレファブ")]
    private GameObject fitEffect;

    [SerializeField, Header("レーン0タイルボタン")]
    private Button[] tiles0;

    [SerializeField, Header("レーン1タイルボタン")]
    private Button[] tiles1;

    [SerializeField, Header("レーン2タイルボタン")]
    private Button[] tiles2;

    [SerializeField, Header("レーン3タイルボタン")]
    private Button[] tiles3;

    //各レーンのタイル情報
    [SerializeField, Header("レーンタイル情報")]
    private Define.Tile[,] tileInfo;

    [SerializeField, Header("失敗フラグ")]
    bool miss;

    [SerializeField]
    private SoundManager soundManager;

    [SerializeField, Header("スコアテキスト")]
    private Text scoreText;

    [SerializeField, Header("リザルトパネル")]
    private GameObject resultPanel;

    [SerializeField, Header("クリアパネル")]
    private GameObject clearPanel;

    [SerializeField, Header("FailedPanel")]
    private GameObject failedPanel;

    public enum State
    {
        Idle,
        Moving,
        Checking,
        MakeNext,
        Result,
    }

    [SerializeField]
    private State state;


    private void Awake()
    {
        //広告表示
        AdManager.instance.RequestBanner(true);

        Init();

        //ステージデータ取得
        stageData = masterDataManager.GetStageData(gameData.nowStageNumber);

        //タイルシート作成
        MakeTileSheet();

        //プールタイル生成
        MakePoolTiles(0);
    }

    void Init()
    {
        gameData.nowWaveNumber = 0;
        gameData.nowScore = 0;
        Debug.Log("ステージ番号" + gameData.nowStageNumber);

        state = State.Idle;

        laneCompleted = new bool[4];
    }


    //タイルシート作成
    public void MakeTileSheet()
    {
        int sheetNum = 0;

        tileInfo = new Define.Tile[4, 4];


        for (int i = 0; i < stageData.GetStageData().sheet[sheetNum].lane.Length; i++)
        {
            lanePoint[i].text = stageData.GetStageData().sheet[sheetNum].lane[i].point + "pt";

            for(int t = 0; t < stageData.GetStageData().sheet[sheetNum].lane[i].tile.Length; t++)
            {
                tileInfo[i, t] = stageData.GetStageData().sheet[sheetNum].lane[i].tile[t];   

                int laneNum = i;
                int tileNum = t;
                switch (laneNum)
                {
                    case 0:
                        tileInfo[laneNum, tileNum].obj = tiles0[tileNum].gameObject;
                        tiles0[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles0[tileNum].GetComponent<Image>().sprite = masterDataManager.tileImage[(int)tileInfo[laneNum, tileNum].type];
                        break;

                    case 1:
                        tileInfo[laneNum, tileNum].obj = tiles1[tileNum].gameObject;
                        tiles1[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles1[tileNum].GetComponent<Image>().sprite = masterDataManager.tileImage[(int)tileInfo[laneNum, tileNum].type];
                        break;

                    case 2:
                        tileInfo[laneNum, tileNum].obj = tiles2[tileNum].gameObject;
                        tiles2[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles2[tileNum].GetComponent<Image>().sprite = masterDataManager.tileImage[(int)tileInfo[laneNum, tileNum].type];
                        break;

                    case 3:
                        tileInfo[laneNum, tileNum].obj = tiles3[tileNum].gameObject;
                        tiles3[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles3[tileNum].GetComponent<Image>().sprite = masterDataManager.tileImage[(int)tileInfo[laneNum, tileNum].type];
                        break;
                }
            }
            
        }
    }

    //シートにある各タイルボタン　
    void TileButton(int laneNum, int tileNum)
    {
        if(state != State.Idle)
        {
            return;
        }

        Debug.Log(string.Format("TileButton lane:{0} tile:{1}", laneNum, tileNum));
        //押されたタイルバックボタンのタイルタイプと一致するタイルタイプをプールで検索する
        Define.TileType type = tileInfo[laneNum, tileNum].type;

        //タイルの移動先を決定し、Filledフラグを立てる
        List<int[]> movePos = new List<int[]>();
        for(int i = 0; i < 4; i++)
        {
            if(tileInfo[laneNum, i].type == type)
            {
                //埋まっていなければ移動先に追加
                if(tileInfo[laneNum, i].filled == false)
                {
                    movePos.Add(new int[] { laneNum, i });
                    Debug.Log("ddd"+ tileInfo[laneNum, i].type);
                }
            }
        }


        int posNum = 0;
        foreach (var pt in poolTilesInfo)
        {
            if(pt.type == type)
            {
                if (posNum >= movePos.Count)
                {
                    //これ以上は入らない
                    Debug.LogError("ミス処理！ タイルが移動して破れて欲しい？");
                    miss = true;
                    break;
                    //posNum = movePos.Count - 1;
                }

                RectTransform rect = pt.obj.GetComponent<RectTransform>();;

                state = State.Moving;

                rect.DOMove(tileInfo[movePos[posNum][0], movePos[posNum][1]].obj.transform.position, 1.0f);
                rect.DORotateQuaternion(Quaternion.identity, 1.0f).OnComplete(() => {
                    //エフェクト表示？
                });
                pt.obj.GetComponent<FloatingTile>().DisableFloating();

                //埋まった
                tileInfo[movePos[posNum][0], movePos[posNum][1]].filled = true;

                soundManager.PlaySE(SoundData.SE.Select);

                posNum++;
            }
        }

        if(posNum == 0)
        {
            Debug.LogError("移動できるものがない");
            return;
        }

        StartCoroutine(ClearCheck(laneNum));
    }

    IEnumerator ClearCheck(int laneNum)
    {
        state = State.Checking;

        yield return new WaitForSeconds(2.0f);

        if(miss == true)
        {
            Debug.Log("ゲームオーバー");
        }



            for(int tileNum = 0; tileNum < 4; tileNum++)
            {
                //埋まったボタンを光らせて、このあと押せなくする
                if(tileInfo[laneNum, tileNum].filled == true)
                {
                    switch (laneNum)
                    {
                        case 0:
                            tiles0[tileNum].GetComponent<Image>().color = Color.white;
                            tiles0[tileNum].interactable = false;
                            break;

                        case 1:
                            tiles1[tileNum].GetComponent<Image>().color = Color.white;
                            tiles1[tileNum].interactable = false;
                            break;

                        case 2:
                            tiles2[tileNum].GetComponent<Image>().color = Color.white;
                            tiles2[tileNum].interactable = false;
                            break;

                        case 3:
                            tiles3[tileNum].GetComponent<Image>().color = Color.white;
                            tiles3[tileNum].interactable = false;
                            break;
                    }
                }
            }

        for(int pn = 0; pn < 5; pn++)
        {
            Destroy(poolTileImages[pn].gameObject);
        }

        //まずそのレーンが完成したかどうかのチェック 完成していたら点数計算
        bool completed = true;
        for(int i = 0; i < 4; i++)
        {
            if(tileInfo[laneNum, i].filled == false)
            {
                completed = false;
            }
        }

        if(completed == true)
        {
            for(int t = 0; t < 4; t++)
            {
                //エフェクト生成
                var ef = Instantiate(fitEffect, tileInfo[laneNum, t].obj.transform);
            }

            laneCompleted[laneNum] = true;
            //点数計算
            CheckPoint(laneNum);
        }


        //クリアチェック
        bool cleared = true;

        for(int l = 0; l < 4; l++)
        {
            if(laneCompleted[l] == false)
            {
                cleared = false;
            }   
        }

        if(cleared == true)
        {
            state = State.Result;
            soundManager.PlayBGM(SoundData.BGM.Clear);
            resultPanel.SetActive(true);
            clearPanel.SetActive(true);
            
            yield break;
        }

        //次のウェーブがあるかどうか
        if(stageData.GetStageData().pool.Length-1 <= gameData.nowWaveNumber)
        {
            //ゲームオーバー
            Debug.LogError("ゲームオーバー！");
            state = State.Result;
            soundManager.PlayBGM(SoundData.BGM.Failed);
            resultPanel.SetActive(true);
            failedPanel.SetActive(true);
        }
        else
        {
            //次のプールタイルを生成する
            gameData.nowWaveNumber++;
            MakePoolTiles(gameData.nowWaveNumber);
        }
    }


    void CheckPoint(int laneNum)
    {
        Debug.Log("CheckPoint"+laneNum);
        //点数計算
        //まずそのレーンを完成させた時にもらえるポイントをゲット
        //そのあと完成させたレーンの右にあるレーンについて、もし完成しているものがあれば、そのレーンのポイントもボーナスとしてゲット。

        //ポイントイメージの色を1に
        lanePointImage[laneNum].color = Color.white;

        //エフェクト


        soundManager.PlaySE(SoundData.SE.Point);
        gameData.nowScore += stageData.GetStageData().sheet[0].lane[laneNum].point;
        scoreText.text = "score:" + gameData.nowScore;
        
    }


    /// <summary>
    ///　プールタイル作成
    /// </summary>
    public void MakePoolTiles(int waveNum)
    {
        state = State.Idle;

        int tileCount = stageData.GetStageData().pool[waveNum].tile.Length;
        poolTilesInfo = new Define.Tile[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            poolTileImages[i] = Instantiate(poolTilePrefab, poolTileParent.transform).GetComponent<Image>();
            poolTileImages[i].transform.position = poolTileImagePos[i].transform.position;

            poolTilesInfo[i] = stageData.GetStageData().pool[waveNum].tile[i];
            poolTilesInfo[i].obj = poolTileImages[i].gameObject;
            poolTileImages[i].sprite = masterDataManager.tileImage[(int)poolTilesInfo[i].type];
        }

        //ネクストタイルを取得　ない場合は非表示にする
        int lastPoolNum = stageData.GetStageData().pool.Length;
        if(waveNum == lastPoolNum - 1)
        {
            //非表示
            for (int i = 0; i < stageData.GetStageData().pool[waveNum].tile.Length; i++)
            {
                nextTileImages[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < stageData.GetStageData().pool[waveNum+1].tile.Length; i++)
            {
                Define.TileType type = stageData.GetStageData().pool[waveNum+1].tile[i].type;
                nextTileImages[i].sprite = masterDataManager.tileImage[(int)type];
            }
        }
    }




}

