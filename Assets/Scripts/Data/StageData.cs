using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StageData : ScriptableObject
{
    //ステージデータ
    [System.Serializable]
    public struct Stage
    {
        [SerializeField, Header("ステージ名")]
        public string stageName;

        [SerializeField, Header("BGM")]
        public AudioClip bgm;

        [SerializeField, Header("★3")]
        public int star3_score;
        [SerializeField, Header("★2")]
        public int star2_score;
        [SerializeField, Header("★1")]
        public int star1_score;

        [SerializeField, Header("ステージで使えるシート ３種類用意する？")]
        public Define.Sheet[] sheet;

        [SerializeField, Header("プール情報10ウェーブ")]
        public Define.Pool[] pool;
    }

    [SerializeField]
    private Stage stage;

    [Header("モブテキストJP")]
    public string mobText_jp;

    [Header("モブテキストEN")]
    public string mobText_en;

    public Stage GetStageData()
    {
        return stage;
    }

    public void SetStageData(Stage data)
    {
        stage = data;
    }
}
