using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ReoGames;
using static UnityEngine.GameObject;
using Reo;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField, Header("クエストボタンプレファブ")]
    private GameObject questButtonPrefab;

    [SerializeField, Header("クエストボタン親")]
    private GameObject questButtonParent;

    private List<Button> questButtonList = new List<Button>();

    [SerializeField]
    GameDataManager gameDataManager;

    [SerializeField, Header("LoadingPanel")]
    private GameObject loadingPanel;

    const string STAGE_DATA_FOLDER_PATH = "Datas/StageDatas";

    //クエスト選択パネル
    private GameObject questPanel;
    //クエスト詳細パネルクラス
    private QuestInfoUI questInfoUI;

    private Button hideQuestPanelButton;
    private Button nextQuestPanelButton;

    public HomeUI homeUI;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadUI()
    {
        questInfoUI = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel).GetComponent<QuestInfoUI>();
        questInfoUI.Initialize();
        questPanel = Find(HierarchyPath_Home.UICanvas._QuestPanel).gameObject;
        MakeQuestButtons();
        homeUI = Find(HierarchyPath_Home.UICanvas.Root).GetComponent<HomeUI>();
        hideQuestPanelButton = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestListBack_HideQuestPanelButton).AddComponent<Button>();
        hideQuestPanelButton.onClick.RemoveAllListeners();
        hideQuestPanelButton.onClick.AddListener(() => { homeUI.ShowPanel(false, HomeUI.Panel.Puzzle); });
        nextQuestPanelButton = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestListBack_NextQuestPanelButton).AddComponent<Button>();
        nextQuestPanelButton.onClick.RemoveAllListeners();
        nextQuestPanelButton.onClick.AddListener(() => {
            //ステージ番号をプレイ可能な最新ステージのものに設定する
            gameDataManager.GetGameData().nowStageNumber = PlayerPrefs.GetInt(Define.NEW_STAGE_KEY, 0);
            BeginButton();
        });
        //questPanel.SetActive(false);
    }

    public void ShowQuestPanel(bool show)
    {
        //表示されるときにステージボタンのOnOff設定をおこなう
       StageButtonEnable();
        questPanel.SetActive(show);
    }

    private void StageButtonEnable()
    {
        int newStage = PlayerPrefs.GetInt(Define.NEW_STAGE_KEY, 0);
        //進んでいるステージまでしかボタンを有効にしない．
        for(int q = 0; q < questButtonList.Count; q++)
        {
            questButtonList[q].image.color = (q <= newStage) ? Color.white : Color.gray;
            questButtonList[q].enabled = (q <= newStage);
        }
    }

    //クエストボタン生成
    void MakeQuestButtons()
    {

        //全ステージデータ
        var allStageDatas = Resources.LoadAll<StageData>(STAGE_DATA_FOLDER_PATH);

        for(int i = 0; i < allStageDatas.Length; i++)
        {
            int stageNum = i;
            var qb = Instantiate(questButtonPrefab, questButtonParent.transform);

            qb.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", stageNum);

            Button qButton = qb.GetComponent<Button>();
            qButton.onClick.AddListener(()=> {
                SoundManager.instance.PlaySE(SoundData.SE.Select);
                gameDataManager.GetGameData().nowStageNumber = stageNum;

                //詳細パネル表示　
                ShowInfoPanel(true,allStageDatas[stageNum], stageNum);
            });
            questButtonList.Add(qButton); ;
        }
    }

    public void ShowInfoPanel(bool show, StageData data = null, int stageNumber = 0)
    {
        //クエストパネル表示
        questInfoUI.ShowQuestInfo(show, data, stageNumber);
        //questInfoPanel.SetActive(show);
    }

    public void BeginButton()
    {
        loadingPanel.SetActive(true);
        SoundManager.instance.PlaySE(SoundData.SE.Select);
        StartCoroutine(BeginGame());

    }

    IEnumerator BeginGame()
    {
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("Game");
    }


    public void ShowQuestListPanelButton(bool show)
    {
        nextQuestPanelButton.gameObject.SetActive(show);
        hideQuestPanelButton.gameObject.SetActive(show);
    }
}
