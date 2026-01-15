using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JsonSplitter
{
    #if UNITY_EDITOR
    [MenuItem("Tools/UGS/분리된 JSON 시트 저장")]
    public static void SplitAndSaveSheets()
    {
        string inputPath = "Assets/UGS.Generated/Resources/hhmd_DataBase.json";
        string outputDir = Path.Combine(Application.streamingAssetsPath, "AssetBundles/CardDataBase_JSON");

        if (!File.Exists(inputPath))
        {
            Debug.LogError("원본 JSON 파일을 찾을 수 없습니다: " + inputPath);
            return;
        }

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
            Debug.Log($"새 폴더 생성됨: {outputDir}");
        }

        string jsonText = File.ReadAllText(inputPath);
        JObject root = JObject.Parse(jsonText);

        if (root["jsonObject"] is not JObject sheets)
        {
            Debug.LogError("'jsonObject' 섹션이 잘못되었습니다.");
            return;
        }

        foreach (var sheetPair in sheets)
        {
            string sheetName = sheetPair.Key;
            JObject sheetObject = (JObject)sheetPair.Value;

            var columnNames = new List<string>();
            var columns = new List<JArray>();
            foreach (var col in sheetObject)
            {
                columnNames.Add(col.Key.Split(':')[0].Trim());
                columns.Add(JArray.Parse(col.Value.ToString()));
            }

            int rowCount = columns[0].Count;
            JArray rowArray = new JArray();

            for (int i = 0; i < rowCount; i++)
            {
                JObject rowObj = new JObject();
                for (int j = 0; j < columnNames.Count; j++)
                {
                    string fieldName = columnNames[j];
                    string cellValue = columns[j][i].ToString().Trim();

                    if (fieldName.Equals("cardType", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (System.Enum.TryParse(cellValue, out eCardType enumValue))
                            rowObj[fieldName] = ((int)enumValue).ToString();
                        else
                            rowObj[fieldName] = "-1";
                    }
                    else if (fieldName.Equals("cardRank", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (System.Enum.TryParse(cellValue, out eCardRank enumValue))
                            rowObj[fieldName] = ((int)enumValue).ToString();
                        else
                            rowObj[fieldName] = "-1";
                    }
                    else if (fieldName.Equals("conditionType", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (System.Enum.TryParse(cellValue, out eConditionType enumValue))
                            rowObj[fieldName] = ((int)enumValue).ToString();
                        else
                            rowObj[fieldName] = "-1";
                    }
                    else if (fieldName.Equals("targetType", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (System.Enum.TryParse(cellValue, out eTargetType enumValue))
                            rowObj[fieldName] = ((int)enumValue).ToString();
                        else
                            rowObj[fieldName] = "-1";
                    }
                    else if (fieldName.Equals("sex", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (System.Enum.TryParse(cellValue, out eSex enumValue))
                            rowObj[fieldName] = ((int)enumValue).ToString();
                        else
                            rowObj[fieldName] = "-1";
                    }
                    else if (fieldName.Equals("panicResponse", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (System.Enum.TryParse(cellValue, out ePanic enumValue))
                            rowObj[fieldName] = ((int)enumValue).ToString();
                        else
                            rowObj[fieldName] = "-1";
                    }
                    else
                    {
                        rowObj[fieldName] = cellValue;
                    }
                }
                rowArray.Add(rowObj);
            }

            string outputPath = Path.Combine(outputDir, sheetName + ".json");
            string outputJson = JsonConvert.SerializeObject(rowArray, Formatting.Indented);

            File.WriteAllText(outputPath, outputJson);
        }

        AssetDatabase.Refresh();
    }
    #endif
}