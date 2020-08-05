using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using System.IO;

public static class UnitEnumCreator
{
    //CSVファイルの読み込みは省略
    //ユニット名を読み取ったリストがこれとする
    static List<string> itemList = new List<string>(){ "Unit0", "Unit1", "Unit2", };

    //Defineを作成する場所
    static string DEFINE_FILE_PATH = "Assets/Scripts/Defines/UnitDefine.cs";
 
 [MenuItem("Tools/Enum作成")]
    public static void Create()
    {
        if (CanCreate() == true)
        {
            //Define作成
            CreateDefine();
        }
        EditorUtility.DisplayDialog(DEFINE_FILE_PATH, "作成完了", "OK");
    }

    //[MenuItem("Tools/Enum作成", true)]
    public static bool CanCreate()
    {
        //シーン実行中とかコンパイル中とかには作成させないようにする
        return !EditorApplication.isPlaying
        && !Application.isPlaying
        && !EditorApplication.isCompiling;
    }

    //Defineを作ってそこにUnitのEnumを書く
    public static void CreateDefine()
    {
        //これに何を書きたいか設定していく．
        var enumBuilder = new StringBuilder();
        enumBuilder.AppendLine("public static class UnitDefine");
        enumBuilder.AppendLine("{");
        enumBuilder.AppendLine("\tpublic enum Unit");
        enumBuilder.AppendLine("\t{");
        foreach (var item in itemList)
        {
            enumBuilder.Append("\t\t").AppendFormat("{0},", item)
            .AppendLine();
        }
        enumBuilder.AppendLine("\t}");
        enumBuilder.AppendLine("}");

        var directoryName = Path.GetDirectoryName(DEFINE_FILE_PATH);
        if (Directory.Exists(directoryName) == false)
        {
            Directory.CreateDirectory(directoryName);
        }
        File.WriteAllText(DEFINE_FILE_PATH, enumBuilder.ToString(),
             Encoding.UTF8);
        AssetDatabase.ImportAsset(DEFINE_FILE_PATH);
    }
}