using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReoGames
{
    public class LanguageChanger : MonoBehaviour
    {
        private Text label;

        [SerializeField, Header("日本語")]
        private string jp;

        [SerializeField, Header("English")]
        private string en;

        private void Awake()
        {
            label = this.GetComponent<Text>();
            var language = Application.systemLanguage;
            switch (language)
            {
                case SystemLanguage.Japanese:
                    {
                        label.text = jp;
                    }
                    break;

                default:
                    {
                        label.text = en;
                    }
                    break;
            }
        }
    }
}

