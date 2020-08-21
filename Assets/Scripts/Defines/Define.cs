using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Define
{
    //タイルの種類
    [System.Serializable]
    public enum TileType
    {
        A,
        B,
        C,
        D,
        E,
        F,
    }

    //タイル情報
    [System.Serializable]
    public struct Tile
    {
        [SerializeField, Header("タイルタイプ")]
        public TileType type;

        [SerializeField, Header("埋まっているかどうか（シートタイルのみ使用）")]
        public bool filled;

        [SerializeField, Header("オブジェクト")]
        public GameObject obj;

    }

    //レーンデータ  各レーンに４つのタイルが配置される
    [System.Serializable]
    public struct Lane
    {
        [SerializeField, Header("タイル情報　４つ")]
        public Tile[] tile;

        [SerializeField, Header("完成した時もらえるポイント")]
        public int point;
    }


    //シートデータ
    [System.Serializable]
    public struct Sheet
    {
        [SerializeField, Header("レーン情報　４つ")]
        public Lane[] lane;
    }


    //プールデータ
    [System.Serializable]
    public struct Pool
    {
        [SerializeField, Header("１プールに含まれる５つのタイル")]
        public Tile[] tile;
    }



    //プレイヤーデータ
    //最新ステージ（クエスト）番号
    public const string NEW_STAGE_KEY = "NEW_STAGE_KEY";
    //ハイスコア
    public const string HIGH_SCORE_FORMAT_KEY = "HIGH_SCORE_STAGE{0}";

    //スコア表記フォーマット
    public static readonly string scoreFormat = "D5";
}
