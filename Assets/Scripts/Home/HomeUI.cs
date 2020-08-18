using Reo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GameObject;
using DG.Tweening;

namespace ReoGames
{
    public class HomeUI : MonoBehaviour
    {
        public static HomeUI instance;

        private GameObject questPanel;
        private GameObject shopPanel;
        private GameObject settingPanel;
        private Button puzzleButton;
        private Button shopButton;
        private Button settingsButton;

        private QuestManager questManager;

        public enum Panel
        {
            Puzzle,
            Shop,
            Settings,
        }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            LoadUI();
        }

        private void LoadUI()
        {
            //オブジェクト検索，コンポーネント追加
            questManager = Find(HierarchyPath_Home.QuestManager.Root).GetComponent<QuestManager>();
            questManager.LoadUI();
            questPanel = Find(HierarchyPath_Home.UICanvas._QuestPanel).gameObject;
            questPanel.SetActive(false);
            shopPanel = Find(HierarchyPath_Home.UICanvas._ShopPanel).gameObject;
            shopPanel.SetActive(false);
            settingPanel = Find(HierarchyPath_Home.UICanvas._SettingPanel).gameObject;
            settingPanel.SetActive(false);
            puzzleButton = Find(HierarchyPath_Home.UICanvas._PuzzleButton).AddComponent<Button>();
            shopButton = Find(HierarchyPath_Home.UICanvas._ShopButton).AddComponent<Button>();
            settingsButton = Find(HierarchyPath_Home.UICanvas._SettingButton).AddComponent<Button>();

            //ボタン設定
            puzzleButton.onClick.RemoveAllListeners();
            puzzleButton.onClick.AddListener(()=> { ShowPanel(true, Panel.Puzzle); });
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(() => { ShowPanel(true, Panel.Shop); });
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => { ShowPanel(true, Panel.Settings); });
        }


        //各種パネル表示
        public void ShowPanel(bool show, Panel panel)
        {
            if (show == true)
            {
                SoundManager.instance.PlaySE(SoundData.SE.Select);
            }
            switch (panel)
            {
                case Panel.Puzzle:
                    {
                        questManager.ShowQuestPanel(show);
                        //ホーム画面ボタン3種消す
                        ShowHomeUIButtons(!show);
                    }
                    break;

                case Panel.Shop:
                    {
                        shopPanel.SetActive(show);
                    }
                    break;

                case Panel.Settings:
                    {
                        settingPanel.SetActive(show);
                    }
                    break;
            }
        }
       

        public void ShowHomeUIButtons(bool show)
        {
            //後でUIアニメーションつける？
            if (show == true)
            {
                puzzleButton.gameObject.SetActive(true);
                shopButton.gameObject.SetActive(true);
                settingsButton.gameObject.SetActive(true);
            }
            else
            {
                puzzleButton.gameObject.SetActive(false);
                shopButton.gameObject.SetActive(false);
                settingsButton.gameObject.SetActive(false);
            }
        }

        public void JumpToWeb()
        {
#if UNITY_ANDROID
            string url = "";
            if( Application.systemLanguage = SystemLanguage.Japanese)
            {
                url = "https://play.google.com/store/apps/developer?id=Reo+Games&hl=jp_JP";
            }
            else
            {
			    url = "https://play.google.com/store/apps/developer?id=Reo+Games&hl=en_US";
            }
			Application.OpenURL(url);
#elif UNITY_IPHONE
            string url = "https://itunes.apple.com/jp/developer/reo-komura/id1353284945";
            Application.OpenURL(url);
#else
#endif
        }

        [SerializeField]
        private GameObject creditPanel = default;
        public void ShowCreditPanel(bool show)
        {
            //SE
            if (show == true) { SoundManager.instance.PlaySE(SoundData.SE.Select); }
            else { SoundManager.instance.PlaySE(SoundData.SE.Cansel); }
            creditPanel.SetActive(show);
        }

        public void HideSettingPanel()
        {
            SoundManager.instance.PlaySE(SoundData.SE.Cansel);
            settingPanel.SetActive(false);
        }
    }
}

