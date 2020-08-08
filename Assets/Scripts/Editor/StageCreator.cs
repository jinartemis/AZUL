using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;

namespace Reo
{
    public class StageCreator
    {
        static readonly string CSV_PATH = "CSV/AZUL_StageParams";
        static readonly string STAGE_DATA_FILE_FORMAT_PATH = "Assets/Resources/Datas/StageDatas/stage{0}.asset";
        static readonly string STAGE_DATA_FILE_FORMAT_RESOURCES_PATH = "Datas/StageDatas/stage{0}";
        static readonly string DEFINE_PATH = "Assets/Scripts/Defines/StageDataDefine.cs";

        static int sheetNumber = 0;
        static int laneCount = 4;
        static int poolCount = 10;
        static int sheetTileCount = 4;
        static int poolTileCount = 5;

        [MenuItem("Tools/ステージデータ作成")]
        static void Create()
        {
            if(EditorCommon.CanCreate() == false)
            {
                return;
            }

            CreateStageDatas();

            EditorUtility.DisplayDialog("ステージデータ作成", "作成完了", "OK");
        }

        [MenuItem("Tools/ステージデータDefine作成")]
        static void CreateDefine()
        {
            if (EditorCommon.CanCreate() == false)
            {
                return;
            }

            CreateStageDataDefine();

            EditorUtility.DisplayDialog("ステージデータDefine作成", "作成完了", "OK");
        }

        private static void CreateStageDataDefine() 
        {
            TextAsset csv = Resources.Load<TextAsset>(CSV_PATH);
            if (csv == null)
            {
                Debug.LogError("CSVファイルが見つかりませんでした");
                return;
            }

            StringReader reader = new StringReader(csv.text);
            List<List<string>> strList = new List<List<string>>();
            int lineCounter = 0;
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                lineCounter++;
                if (lineCounter == 1)
                {
                    //項目名Define作成
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("public static partial class Define");
                    builder.AppendLine("{");
                    builder.AppendLine("\tpublic enum StageDataElement");
                    builder.AppendLine("\t{");
                    var elements = line.Split(',').Select(str => EditorCommon.GetStrWithoutInvalidChars(str, new List<string> { " ", "_" })).ToList();
                    foreach (var e in elements)
                    {
                        if (e == "") { continue; }; //何もないセルはスキップ
                        builder.AppendFormat("\t\t{0},", e).AppendLine();
                    }
                    builder.AppendLine("\t}");
                    builder.AppendLine("}");

                    //保存
                    //Directoryチェック
                    var defineDirectoryName = Path.GetDirectoryName(DEFINE_PATH);
                    if (Directory.Exists(defineDirectoryName) == false)
                    {
                        Directory.CreateDirectory(defineDirectoryName);
                    }
                    //CSファイルチェック
                    if (File.Exists(DEFINE_PATH) == false)
                    {
                        Debug.Log("新規作成" + DEFINE_PATH);
                    }
                    File.WriteAllText(DEFINE_PATH, builder.ToString(), Encoding.UTF8);
                    AssetDatabase.ImportAsset(DEFINE_PATH);
                    break;
                }
            }
        }

        private static void CreateStageDatas()
        {
            TextAsset csv = Resources.Load<TextAsset>(CSV_PATH);
            if(csv == null)
            {
                Debug.LogError("CSVファイルが見つかりませんでした");
                return;
            }

            StringReader reader = new StringReader(csv.text);
            List<List<string>> strList = new List<List<string>>();
            List<string> defineElementList = new List<string>();
            int lineCounter = 0;
            while(reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                lineCounter++;
                if (lineCounter == 1)
                {
                    //項目名Define作成
                    defineElementList = line.Split(',').Select(str => EditorCommon.GetStrWithoutInvalidChars(str, new List<string> { " " })).ToList(); //英語の場合スペースが入るので削除させない
                }
                else
                {
                    var items = line.Split(',').Select(str => EditorCommon.GetStrWithoutInvalidChars(str, new List<string>{" "})).ToList(); //英語の場合スペースが入るので削除させない
                    strList.Add(items);
                }
            }

            foreach (var list in strList)
            {
                string stageNumber = (int.Parse(list.First())).ToString("D3");
                //データが存在していないときは新規作成して保存する
                var DATA_PATH = string.Format(STAGE_DATA_FILE_FORMAT_PATH, stageNumber);
                var dataDirectoryName = Path.GetDirectoryName(DATA_PATH);
                if (Directory.Exists(dataDirectoryName) == false)
                {
                    Directory.CreateDirectory(dataDirectoryName);
                }

                //ScriptableObject作成
                StageData data = default;
                if (File.Exists(DATA_PATH) == false)
                {
                    Debug.Log("新規作成");
                    data = ScriptableObject.CreateInstance("StageData") as StageData;
                    AssetDatabase.CreateAsset(data, DATA_PATH);
                }
                else
                {
                    data = Resources.Load(string.Format(STAGE_DATA_FILE_FORMAT_RESOURCES_PATH, stageNumber)) as StageData;
                }

                //ステージデータ読み込み
                var _stageData = data.GetStageData();
                _stageData.stageName = list.First();
                _stageData.sheet = new Define.Sheet[sheetNumber+1];
                _stageData.sheet[sheetNumber].lane = new Define.Lane[laneCount];
                for(int i = 0; i < laneCount; i++) { _stageData.sheet[sheetNumber].lane[i].tile = new Define.Tile[sheetTileCount];  };
                //プール数取得
                var poolList = list.Where((item, index) => index > (int)Define.StageDataElement.Pool0_0).Where(element => element != "");
                poolCount = (poolList.Count()) / poolTileCount;
                Debug.Log("PoolCount" + poolCount);
                _stageData.pool = new Define.Pool[poolCount];
                for (int h = 0; h < poolCount; h++) { _stageData.pool[h].tile = new Define.Tile[poolTileCount]; };
                for (int i = 0; i < list.Count; i++)
                {
                    switch (i)
                    {
                        //BGM
                        //case (int)(Define.StageDataElement.BGM): { _stageData.bgm =  }break;
                        //モブテキスト
                        case (int)(Define.StageDataElement.MobJP): { data.mobText_jp = list[i]; } break;
                        case (int)(Define.StageDataElement.MobEN): { data.mobText_en = list[i]; } break;
                        //★3
                        case (int)(Define.StageDataElement.Star3): { _stageData.star3_score = int.Parse(list[i]); } break;
                        //★2
                        case (int)(Define.StageDataElement.Star2): { _stageData.star2_score = int.Parse(list[i]); } break;
                        //★1
                        case (int)(Define.StageDataElement.Star1): { _stageData.star1_score = int.Parse(list[i]); } break;
                        //ボーナス　レーン0
                        case (int)(Define.StageDataElement.Bonus0): { _stageData.sheet[sheetNumber].lane[0].point = int.Parse(list[i]); } break;
                        //ボーナス　レーン1
                        case (int)(Define.StageDataElement.Bonus1): { _stageData.sheet[sheetNumber].lane[1].point = int.Parse(list[i]); } break;
                        //ボーナス　レーン2
                        case (int)(Define.StageDataElement.Bonus2): { _stageData.sheet[sheetNumber].lane[2].point = int.Parse(list[i]); } break;
                        //ボーナス　レーン3
                        case (int)(Define.StageDataElement.Bonus3): { _stageData.sheet[sheetNumber].lane[3].point = int.Parse(list[i]); } break;
                    }
                    if ((int)(Define.StageDataElement.Lane0_0) <= i && i <= (int)(Define.StageDataElement.Lane3_3))
                    {
                        var laneNumber = (i - (int)Define.StageDataElement.Lane0_0) / 4;    //(i < 13) ? 0 : (i < 17) ? 1 : (i < 21) ? 2 : 3;
                        var sheetTileNumber = (i - (int)Define.StageDataElement.Lane0_0) % 4;
                        _stageData.sheet[sheetNumber].lane[laneNumber].tile[sheetTileNumber].type = (Define.TileType)(int.Parse(list[i]));
                    }
                    else if((int)(Define.StageDataElement.Pool0_0) <= i )
                    {
                        if (list[i] == "") continue;
                        var poolNumber = (i - (int)(Define.StageDataElement.Pool0_0)-1) / 6;
                        var poolTileNumber = (i - (int)(Define.StageDataElement.Pool0_0)-1) % 6;
                        _stageData.pool[poolNumber].tile[poolTileNumber].type = (Define.TileType)(int.Parse(list[i]));
                    }
                    data.SetStageData(_stageData);
                }


                AssetDatabase.ImportAsset(DATA_PATH);
                EditorUtility.SetDirty(data);
            }
        }
    }
}

