using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DebugMenu 
{
    [MenuItem("Tools/ResetUserData")]
    public static void ResetUserData()
    {
        PlayerPrefs.DeleteAll();
        UnityEditor.EditorUtility.DisplayDialog("ユーザーデータリセット", "ユーザーデータ削除完了", "OK");
    }
}
