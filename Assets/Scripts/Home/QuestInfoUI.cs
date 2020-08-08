using Reo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GameObject;

namespace ReoGames
{
    public class QuestInfoUI : MonoBehaviour
    {
        //クエスト名
        private Text questLabel;
        //評価★
        private Image[] stars = new Image[3];
        //依頼人画像
        private Image mobImage;
        //セリフテキスト
        private Text serihuLabel;
        //ジェム画像
        public struct Lane
        {
            public Image[] gems;
        }
        private Lane[] lanes;
        //画面下Waveジェムリスト
        public struct GemListPanel
        {
            public Text waveLabel;
            public Image[] gems;
        }
        private GemListPanel[] gemListPanels;

        //パネル非表示ボタン
        private Button closeQuestInfoPanelButton;
        //ゲーム開始ボタン
        private Button startButton;

        [Header("シート番号")]
        private int sheetNumber = 0;

        public void Initialize()
        {
            LoadUI();
            this.gameObject.SetActive(false);
        }

        private void LoadUI()
        {
            questLabel = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_QuestLabel).GetComponent<Text>();
            stars[0] = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_Stars_star0).GetComponent<Image>();
            stars[1] = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_Stars_star1).GetComponent<Image>();
            stars[2] = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_Stars_star2).GetComponent<Image>();
            mobImage = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_MobCharacter).GetComponent<Image>();
            serihuLabel = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_SerihuFrame_SerihuLabel).GetComponent<Text>();
            var correctPanel = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_CorrectPanel).transform;
            lanes = new Lane[correctPanel.childCount];
            for(int i = 0; i < lanes.Length; i++)
            {
                lanes[i].gems = new Image[correctPanel.GetChild(i).childCount];
                for(int g = 0; g < lanes[i].gems.Length; g++)
                {
                    lanes[i].gems[g] = correctPanel.GetChild(i).GetChild(g).GetComponent<Image>();
                }
            }
            var gemListContent = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_GemListScrollView_Viewport_GemListScrollViewContent).transform;
            gemListPanels = new GemListPanel[gemListContent.childCount];
            for(int h = 0; h < gemListPanels.Length; h++)
            {
                gemListPanels[h].waveLabel = gemListContent.GetChild(h).Find("waveLabel").GetComponent<Text>();
                gemListPanels[h].gems = new Image[gemListContent.GetChild(h).Find("list").childCount];
                for(int j = 0; j < gemListPanels[h].gems.Length; j++)
                {
                    gemListPanels[h].gems[j] = gemListContent.GetChild(h).Find("list").GetChild(j).GetComponent<Image>();
                }
            }
            closeQuestInfoPanelButton = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_HideInfoPanelButton).AddComponent<Button>();
            startButton = Find(HierarchyPath_Home.UICanvas._QuestPanel_QuestInfoPanel_StartGameButton).AddComponent<Button>();

            //Button Settings
            closeQuestInfoPanelButton.onClick.RemoveAllListeners();
            closeQuestInfoPanelButton.onClick.AddListener(()=> { QuestManager.instance.ShowInfoPanel(false); });
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() => { QuestManager.instance.BeginButton(); });
        }

        public void ShowQuestInfo(bool show, StageData data = null, int stageNumber = 0)
        {
            QuestManager.instance.ShowQuestListPanelButton(!show);

            if (data != null)
            {
                //ステージ情報更新
                var sData = data.GetStageData();
                questLabel.text = $"Stage{stageNumber}";// sData.stageName;
                int highScore = PlayerPrefs.GetInt(string.Format(Define.HIGH_SCORE_FORMAT_KEY, stageNumber), 0);
                int star3Score = sData.star3_score;
                int star2Score = sData.star2_score;
                int star1Score = sData.star1_score;
                int starCount = ScoreChecker.GetStarScore(highScore, star3Score, star2Score, star1Score);
                for(int i = 0; i < 3; i++)
                {
                    stars[i].color = (i < starCount) ? Color.white : Color.gray;
                }
                //完成図
                for (int l = 0; l < sData.sheet[0].lane.Length; l++)
                {
                    var tile = sData.sheet[0].lane[l].tile;
                    for (int t = 0; t < tile.Length; t++)
                    {
                        int gemType = (int)sData.sheet[sheetNumber].lane[l].tile[t].type;
                        var gemImage = MasterDataManager.instance.GetTileImages()[gemType];
                        lanes[l].gems[t].sprite = gemImage;
                    }
                }

                //プールリスト
                for (int p = 0; p < sData.pool.Length; p++)
                {
                    gemListPanels[p].waveLabel.text = $"Wave{p}";
                    for(int g = 0; g < sData.pool[p].tile.Length; g++)
                    {
                        int gemType2 = (int)sData.pool[p].tile[g].type;
                        gemListPanels[p].gems[g].sprite = MasterDataManager.instance.GetTileImages()[gemType2];
                    }
                }
            }
            this.gameObject.SetActive(show);
        }
    }
}

