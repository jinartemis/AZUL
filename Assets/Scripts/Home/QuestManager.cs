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

    [SerializeField]
    GameDataManager gameDataManager;

    [SerializeField]
    private SoundManager soundManager;

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
            //ステージ番号を最新のステージのものに設定する
            var allStageDatas = Resources.LoadAll<StageData>(STAGE_DATA_FOLDER_PATH);
            gameDataManager.GetGameData().nowStageNumber = allStageDatas.Length-1;
            BeginButton();
        });
        //questPanel.SetActive(false);
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

            qb.GetComponent<Button>().onClick.AddListener(()=> {
                soundManager.PlaySE(SoundData.SE.Point);
                gameDataManager.GetGameData().nowStageNumber = stageNum;

                //詳細パネル表示　
                ShowInfoPanel(true, allStageDatas[stageNum]);
                

            });
        }
    }

    public void ShowInfoPanel(bool show, StageData data = null)
    {
        //クエストパネル表示
        questInfoUI.ShowQuestInfo(show, data);
        //questInfoPanel.SetActive(show);
    }

    public void BeginButton()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(BeginGame());

    }

    IEnumerator BeginGame()
    {
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("Game");
    }

}
