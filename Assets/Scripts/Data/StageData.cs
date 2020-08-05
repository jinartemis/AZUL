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
        [SerializeField, Header("BGM")]
        public AudioClip bgm;

        [SerializeField, Header("ステージで使えるシート ３種類用意する？")]
        public Define.Sheet[] sheet;

        [SerializeField, Header("プール情報10ウェーブ")]
        public Define.Pool[] pool;
    }

    [SerializeField]
    private Stage stage;


    public Stage GetStageData()
    {
        return stage;
    }
}
