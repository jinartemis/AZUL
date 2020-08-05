using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField, Header("ゲームデータ")]
    private GameData gameData;

    public GameData GetGameData()
    {
        return gameData;
    }
}
