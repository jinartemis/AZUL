using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using static UnityEngine.GameObject;
using Reo;
using ReoGames;

public partial class GameManager : MonoBehaviour
{

    [SerializeField, Header("ステージ情報 実行時取得")]
    private StageData stageData;

    private int sheetNum = 0;

    [SerializeField, Header("タイル移動スピード")]
    private float moveSpeed = .5f;
    [SerializeField, Header("タイル移動タイムラグ")]
    private float moveDelay = .2f;

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

    [SerializeField, Header("スコアテキスト")]
    private Text scoreText;

    private GameObject resultPanel;
    private GameObject clearPanel;
    private GameObject failedPanel;

    private Button nextStageButton;
    private Button retryButton;
    private GameObject loadingPanel;
    private Text failedTalkLabel;

    public struct ClearUI
    {
        public Image[] stars;
        public Text judgeLabel;  //very nice
        public Image[] lane0Gem;
        public Image[] lane1Gem;
        public Image[] lane2Gem;
        public Image[] lane3Gem;
        public Text scoreLabel;
        public Text scoreValueLabel;
        public Text highScoreLabel;
        public Text highScoreValueLabel;
        public Text newRecordLabel;
    }
    private ClearUI clearUI;

    private string[] failedTalk_JP = new string[] {
    "次に出てくるタイルを見てとるべきタイルを考えよう！" ,
    "Nextをタップすると全タイル一覧が表示されるよ！活用しよう！" ,
    "出現回数が少ないタイルは優先してはめよう！" ,
    "おしい！もう一回やったらいけるかも？"};



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
        Init();

        LoadUI();
    }

    private void Start()
    {
        //ステージデータ取得
        stageData = masterDataManager.GetStageData(gameData.nowStageNumber);

        //タイルシート作成
        MakeTileSheet();

        //プールタイル生成
        StartCoroutine(MakePoolTiles(0));

        //エフェクト生成
        MakeEffects();

        //広告表示
        AdManager.instance.RequestBanner(true);

        //BGM
        SoundManager.instance.PlayBGM(SoundData.BGM.Game);
    }

    void Init()
    {
        gameData.nowWaveNumber = 0;
        gameData.nowScore = 0;
        Debug.Log("ステージ番号" + gameData.nowStageNumber);

        state = State.Idle;

        laneCompleted = new bool[4];
    }

    private void LoadUI()
    {
        //オブジェクト取得
        resultPanel = Find(HierarchyPath_Game.UICanvas._resultPanel).gameObject;
        clearPanel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel).gameObject;
        failedPanel = Find(HierarchyPath_Game.UICanvas._resultPanel_FailedPanel).gameObject;
        nextStageButton = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_NextStageButton).AddComponent<Button>();
        retryButton = Find(HierarchyPath_Game.UICanvas._resultPanel_FailedPanel_RetryButton).AddComponent<Button>();
        loadingPanel = Find(HierarchyPath_Game.UICanvas._LoadingPanel).gameObject;
        failedTalkLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_FailedPanel_FailedHukidashi_FailedTalkLabel).GetComponent<Text>();

        clearUI.stars = new Image[3];
        clearUI.stars[0] = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_JudgeFrame_starImage0).GetComponent<Image>();
        clearUI.stars[1] = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_JudgeFrame_starImage1).GetComponent<Image>();
        clearUI.stars[2] = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_JudgeFrame_starImage2).GetComponent<Image>();
        clearUI.judgeLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_JudgeFrame_JudgeLabel_JudgeText).GetComponent<Text>();
        Transform lane0 = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_ResultImage_lane0).transform;
        Transform lane1 = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_ResultImage_lane1).transform;
        Transform lane2 = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_ResultImage_lane2).transform;
        Transform lane3 = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_ResultImage_lane3).transform;
        int gemCount = lane0.childCount;
        clearUI.lane0Gem = new Image[gemCount];
        clearUI.lane1Gem = new Image[gemCount];
        clearUI.lane2Gem = new Image[gemCount];
        clearUI.lane3Gem = new Image[gemCount];
        for (int g = 0; g < lane0.childCount; g++)
        {
            clearUI.lane0Gem[g] = lane0.GetChild(g).GetComponent<Image>();
            clearUI.lane1Gem[g] = lane1.GetChild(g).GetComponent<Image>();
            clearUI.lane2Gem[g] = lane2.GetChild(g).GetComponent<Image>();
            clearUI.lane3Gem[g] = lane3.GetChild(g).GetComponent<Image>();
        }
        clearUI.scoreLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_ScoreLabel).GetComponent<Text>();
        clearUI.scoreValueLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_ScoreValueLabel).GetComponent<Text>();
        clearUI.highScoreLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_HighScoreLabel).GetComponent<Text>();
        clearUI.highScoreValueLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_HighScoreValueLabel).GetComponent<Text>();
        clearUI.newRecordLabel = Find(HierarchyPath_Game.UICanvas._resultPanel_ClearPanel_NewRecord).GetComponent<Text>();

        //ボタン設定
        nextStageButton.onClick.RemoveAllListeners();
        nextStageButton.onClick.AddListener(() => { GoNextStage(); });
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(() => { RetryStage(); });

        //パネル非表示
        resultPanel.SetActive(false);
        clearPanel.SetActive(false);
        failedPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    //タイルシート作成
    public void MakeTileSheet()
    {
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
                        tiles0[tileNum].GetComponent<Image>().sprite = masterDataManager.GetTileImages()[(int)tileInfo[laneNum, tileNum].type];
                        break;

                    case 1:
                        tileInfo[laneNum, tileNum].obj = tiles1[tileNum].gameObject;
                        tiles1[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles1[tileNum].GetComponent<Image>().sprite = masterDataManager.GetTileImages()[(int)tileInfo[laneNum, tileNum].type];
                        break;

                    case 2:
                        tileInfo[laneNum, tileNum].obj = tiles2[tileNum].gameObject;
                        tiles2[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles2[tileNum].GetComponent<Image>().sprite = masterDataManager.GetTileImages()[(int)tileInfo[laneNum, tileNum].type];
                        break;

                    case 3:
                        tileInfo[laneNum, tileNum].obj = tiles3[tileNum].gameObject;
                        tiles3[tileNum].onClick.AddListener(() => { TileButton(laneNum, tileNum); });
                        tiles3[tileNum].GetComponent<Image>().sprite = masterDataManager.GetTileImages()[(int)tileInfo[laneNum, tileNum].type];
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
                    //Debug.Log("ddd"+ tileInfo[laneNum, i].type);
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

                Sequence seq = DOTween.Sequence()
                    .AppendInterval(posNum * moveDelay)
                    .Append(rect.DOMove(tileInfo[movePos[posNum][0], movePos[posNum][1]].obj.transform.position, moveSpeed))
                    .Join(rect.DORotateQuaternion(Quaternion.identity, moveSpeed).OnComplete(() =>
                    {
                        //タイル移動完了
                    }));
                seq.Play();

                pt.obj.GetComponent<FloatingTile>().DisableFloating();

                //埋まった
                tileInfo[movePos[posNum][0], movePos[posNum][1]].filled = true;

                SoundManager.instance.PlaySE(SoundData.SE.Select);

                posNum++;
            }
        }

        if(posNum == 0)
        {
            SoundManager.instance.PlaySE(SoundData.SE.Error);
            Debug.LogError("移動できるものがない");
            return;
        }

        StartCoroutine(ClearCheck(laneNum));
    }

    IEnumerator ClearCheck(int laneNum)
    {
        state = State.Checking;

        yield return new WaitForSeconds(1.0f);

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
            StartCoroutine(StageClear());
            yield break;
        }

        //次のウェーブがあるかどうか
        if(stageData.GetStageData().pool.Length-1 <= gameData.nowWaveNumber)
        {
            //ゲームオーバー
            Debug.LogError("ゲームオーバー！");
            state = State.Result;
            SoundManager.instance.PlayBGM(SoundData.BGM.Failed);
            ShowResultPanel(clear:false);
        }
        else
        {
            //次のプールタイルを生成する
            gameData.nowWaveNumber++;
            yield return new WaitForSeconds(.5f);
            StartCoroutine(MakePoolTiles(gameData.nowWaveNumber));
        }
    }

    [SerializeField, Header("クリアパネルを表示するまでの時間")]
    private float delayForShowClear = 1.5f;
    private IEnumerator StageClear()
    {
        yield return new WaitForSeconds(delayForShowClear);

        state = State.Result;
        SoundManager.instance.PlayBGM(SoundData.BGM.Clear);
        ShowResultPanel(clear: true);
        //最新ステージ番号更新
        var allStageCount = masterDataManager.GetAllStageCount();
        var nowStage = gameData.nowStageNumber;
        if (nowStage + 1 >= allStageCount)
        {
            //ここで全ステージクリア
            Debug.Log("全ステージクリア");
            //NextStageボタンを無効にする
            nextStageButton.enabled = false;
            nextStageButton.image.color = Color.gray;
        }
        else
        {
            int nextStage = nowStage + 1;
            PlayerPrefs.SetInt(Define.NEW_STAGE_KEY, nextStage);
        }
    }

    //リザルト表示
    private void ShowResultPanel(bool clear)
    {
        //表示設定
        if (clear == true)
        {
            //クリア
            int nowScore = gameData.nowScore;
            int nowStageNumber = gameData.nowStageNumber;
            int hightScore = PlayerPrefs.GetInt(string.Format(Define.HIGH_SCORE_FORMAT_KEY, nowStageNumber));
            int star3Score = stageData.GetStageData().star3_score;
            int star2Score = stageData.GetStageData().star2_score;
            int star1Score = stageData.GetStageData().star1_score;
            int starCount = 0;
            //スコア表記/////////////////////////////////////////////////////////////////////////////////////////////////////////////
            starCount = ScoreChecker.GetStarScore(nowScore, star3Score, star2Score, star1Score);

            for (int i = 0; i < 3; i++)
            {
                //★表示
                clearUI.stars[i].color = (i < starCount) ? Color.white : Color.gray;
            }

            //スコア
            clearUI.newRecordLabel.gameObject.SetActive(nowScore > hightScore);
            if (nowScore > hightScore)
            {
                //ハイスコア更新
                hightScore = nowScore;
                PlayerPrefs.SetInt(string.Format(Define.HIGH_SCORE_FORMAT_KEY, nowStageNumber), hightScore);
            }
            clearUI.scoreValueLabel.text = nowScore.ToString("D8");
            clearUI.highScoreValueLabel.text = hightScore.ToString("D8");
            //完成図表記////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //stageData.sheet[0].lane[0].
            for (int i = 0; i < stageData.GetStageData().sheet[sheetNum].lane.Length; i++)
            {
                for (int t = 0; t < stageData.GetStageData().sheet[sheetNum].lane[i].tile.Length; t++)
                {
                    tileInfo[i, t] = stageData.GetStageData().sheet[sheetNum].lane[i].tile[t];
                    switch (i)
                    {
                        case 0: { clearUI.lane0Gem[t].sprite = masterDataManager.GetTileImages()[(int)tileInfo[i, t].type]; }break;
                        case 1: { clearUI.lane1Gem[t].sprite = masterDataManager.GetTileImages()[(int)tileInfo[i, t].type]; } break;
                        case 2: { clearUI.lane2Gem[t].sprite = masterDataManager.GetTileImages()[(int)tileInfo[i, t].type]; } break;
                        case 3: { clearUI.lane3Gem[t].sprite = masterDataManager.GetTileImages()[(int)tileInfo[i, t].type]; } break;
                    }
                }
            }
        }
        else
        {
            //失敗
            failedTalkLabel.text = failedTalk_JP[Random.Range(0, failedTalk_JP.Length)];
        }

        resultPanel.SetActive(true);
        clearPanel.SetActive(clear);
        failedPanel.SetActive(!clear);
    }

    private void GoNextStage()
    {
        var allStageCount = masterDataManager.GetAllStageCount();
        int nowStage = gameData.nowStageNumber;
        int nextStage = nowStage + 1;
        gameData.nowStageNumber = nextStage;
        //シーン読み直し
        StartCoroutine(ReloadScene());
    }

    private void RetryStage()
    {
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        loadingPanel.SetActive(true);
        SoundManager.instance.PlaySE(SoundData.SE.Select);

        yield return new WaitForSeconds(1.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    void CheckPoint(int laneNum)
    {
        Debug.Log("CheckPoint" + laneNum);
        //点数計算
        //まずそのレーンを完成させた時にもらえるポイントをゲット
        //そのあと完成させたレーンの右にあるレーンについて、もし完成しているものがあれば、そのレーンのポイントもボーナスとしてゲット。

        //ポイントイメージの色を1に
        lanePointImage[laneNum].color = Color.white;

        int effectCount = 0;
        for (int k = laneNum; k < 4; k++)
        {
            //エフェクト
            bool completed = true;
            for (int i = 0; i < 4; i++)
            {
                if (tileInfo[k, i].filled == false)
                {
                    completed = false;
                }
            }
            if (completed == true)
            {
                for (int t = 0; t < 4; t++)
                {
                    //エフェクト生成
                    //var ef = Instantiate(fitEffect, tileInfo[k, t].obj.transform);
                }
                StartCoroutine(PlayCompleteEffect(k, k-laneNum, effectCount));
                effectCount++;
                gameData.nowScore += stageData.GetStageData().sheet[0].lane[k].point;
            }
        }

        SoundManager.instance.PlaySE(SoundData.SE.Point);

        scoreText.text = "score:" + gameData.nowScore;
        
    }


    /// <summary>
    ///　プールタイル作成
    /// </summary>
    //[SerializeField, Header("タイル生成時間")]
    private float makeSpan = .1f;
    public IEnumerator MakePoolTiles(int waveNum)
    {
        //ネクストタイルを取得　ない場合は非表示にする
        int lastPoolNum = stageData.GetStageData().pool.Length;
        if (waveNum == lastPoolNum - 1)
        {
            //非表示
            for (int i = 0; i < stageData.GetStageData().pool[waveNum].tile.Length; i++)
            {
                nextTileImages[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < stageData.GetStageData().pool[waveNum + 1].tile.Length; i++)
            {
                Define.TileType type = stageData.GetStageData().pool[waveNum + 1].tile[i].type;
                nextTileImages[i].sprite = masterDataManager.GetTileImages()[(int)type];
            }
        }

        //タイル生成
        int tileCount = stageData.GetStageData().pool[waveNum].tile.Length;
        poolTilesInfo = new Define.Tile[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            poolTileImages[i] = Instantiate(poolTilePrefab, poolTileParent.transform).GetComponent<Image>();
            poolTileImages[i].transform.position = poolTileImagePos[i].transform.position;

            poolTilesInfo[i] = stageData.GetStageData().pool[waveNum].tile[i];
            poolTilesInfo[i].obj = poolTileImages[i].gameObject;
            poolTileImages[i].sprite = masterDataManager.GetTileImages()[(int)poolTilesInfo[i].type];

            yield return new WaitForSeconds(makeSpan);
        }
        state = State.Idle;
    }
}

