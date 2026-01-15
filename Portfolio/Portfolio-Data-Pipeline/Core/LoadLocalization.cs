using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadLocalization
{
    public static LoadLocalization Instance { get; private set; }
    private string LocalizationFileName = Path.Combine(Application.dataPath, "Localization/AllStringData");
    private string CSVFileName = Path.Combine(Application.streamingAssetsPath, "AssetBundles/StringData/StringData.csv");
    
    private Dictionary<int, string> localizationDictionary = new Dictionary<int, string>();
    

    private LoadLocalization()
    {
        
    }

    public static LoadLocalization CreateInstance()
    {
        if (Instance == null)
        {
            Instance = new LoadLocalization();
            Instance.Initialize();
        }
        return Instance;
    }

    private LoadLocalization Initialize()
    {
        return this;
    }
    
    public string FindStringByIndexAsync(string index)
    {
        LocalizationSettings.InitializationOperation.WaitForCompletion();
        
        var handle = LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync("AllStringData", index);

        var result = handle.WaitForCompletion();
        return result ?? index.ToString();
    }

    #if UNITY_EDITOR
    public static void UpdateStringTable(string assetPath, string key, string value)
    {
        var stringTable = AssetDatabase.LoadAssetAtPath<StringTable>(assetPath);
        if (stringTable == null)
        {
            Debug.LogError($"StringTable을 찾을 수 없습니다: {assetPath}");
            return;
        }
        
        var entry = stringTable.GetEntry(key);
        if(entry == null)
        {
            stringTable.AddEntry(key, value);
        }
        else
        {
            entry.Value = value;
        }
        
        EditorUtility.SetDirty(stringTable);
        AssetDatabase.SaveAssets();
        Debug.Log($"[{key}] -> \"{value}\"적용됨.");
    }
    #endif
}
