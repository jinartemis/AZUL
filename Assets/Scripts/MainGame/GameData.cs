using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//プレイ中のゲームデータ
[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [SerializeField, Header("現在プレイ中のステージ番号")]
    public int nowStageNumber;

    [SerializeField, Header("現在プレイ中のWave番号")]
    public int nowWaveNumber;

    [SerializeField, Header("現在スコア")]
    public int nowScore;
}
