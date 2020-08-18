using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ReoGames
{
    public class HomeCharacter : MonoBehaviour
    {
        [System.Serializable]
        private struct Data
        {
            [Header("キャラクター画像")]
            public Sprite charaSprite;

            [Header("日本語")]
            public string serihu_Jp;

            [Header("English")]
            public string serihu_En;
        }

        [SerializeField, Header("データ")]
        private Data[] data;

        [SerializeField, Header("キャラ")]
        private Image charaImage;

        [SerializeField, Header("吹き出し")]
        private GameObject hukidashi;

        [SerializeField, Header("セリフラベル")]
        private Text serihuLabel;

        private Vector3 hukidashiBasePos = default;

        private void Start()
        {
            hukidashiBasePos = hukidashi.transform.position;
            CharaAction();
            
        }

        public void TouchCharacter()
        {
            SoundManager.instance.PlaySE(SoundData.SE.Touch);
            CharaAction();
        }

        private void CharaAction()
        {
            //ランダムにセリフを表示、
            //表情変更
            int rnd = Random.Range(0, data.Length);
            charaImage.sprite = data[rnd].charaSprite;

            hukidashi.transform.DOJump(endValue: hukidashiBasePos, jumpPower: .1f, numJumps: 1, duration: .5f);
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Japanese:
                    {
                        serihuLabel.text = data[rnd].serihu_Jp;
                    }
                    break;

                default:
                    {
                        serihuLabel.text = data[rnd].serihu_En;
                    }
                    break;
            }
        }
    }
}

