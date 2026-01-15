using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using GoogleSheet;
using Common.Interfaces;

[DefaultExecutionOrder(-500)]
public class DatabaseClassManager : SingletonBehaviour<DatabaseClassManager>
{
    public override bool IsDontDestroyOnLoad => true;
    private DataSerialize dataSerialize;
    private readonly Dictionary<Type, Dictionary<int, IBaseData>> databaseMap = new();
    string jsonFolderPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/CardDataBase_JSON");
    string jsonStringPath = Path.Combine(Application.dataPath, "UGS.Generated/Resources/hhmd_StringData.json");
    string csvOutputPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles/StringData/StringData.csv");
    
    
    public void Initialize()
    {
        dataSerialize = new DataSerialize();

#if UNITY_EDITOR
        GenerateAll();
        JsonToCsvConverter.ConvertJsonToCsv(jsonStringPath, csvOutputPath);
        JsonSplitter.SplitAndSaveSheets();
#endif
        LoadAllDataBase(jsonFolderPath);
    }

    public Dictionary<int, IBaseData> Get<T>() where T : IBaseData
    {
        object bucket = null;
        
        if (databaseMap.TryGetValue(typeof(T), out var list))
            bucket = list;
        else
        {
            foreach (var kv in databaseMap)
            {
                if (kv.Value == null) continue;
                if (typeof(T).IsAssignableFrom(kv.Key))
                {
                    bucket = kv.Value;
                    break;
                }
            }
        }

        if (bucket == null)
        {
            Debug.LogError($"등록되지 않은 리스트 요청: {typeof(T).Name}");
            return new Dictionary<int, IBaseData>();
        }
        
        if (bucket is IDictionary<int, IBaseData> dict)
        {
            return dict
                .Where(kvp => kvp.Value is T)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        Debug.LogError($"지원하지 않는 버킷 타입: {bucket.GetType().FullName}");
        return new Dictionary<int, IBaseData>();
    }

    #region MyGenrate
#if UNITY_EDITOR
    [MenuItem("Tools/Generate All BaseClass SOs")]
    private void GenerateAll()
    {
        string[] files = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "AssetBundles/CardDataBase_JSON"));
        foreach (string filePath in files)
        {
            if (Path.GetExtension(filePath) != ".meta")
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string className = fileName + "Class";
                if (DataTypeRegistry.TryGetType(fileName, out Type type))
                {
                    ClassFileGenerator.GenerateAndSaveSOClass(type, className);
                }
            }
        }
        
        Debug.Log("모든 BaseClass SO 생성이 완료되었습니다!");
    }
    #endif
    #endregion

    private void LoadAllDataBase(string jsonFolderPath)
    {
        var interfaceType = typeof(IBaseData);
        
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t)
            && t.IsClass && !t.IsAbstract)
            .ToArray();
        
        var jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json");
        

        Dictionary<Type, string> typeJsonMap = new Dictionary<Type, string>();

        foreach (var type in types)
        {
            string typeName = type.Name;
            
            foreach (var filePath in jsonFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
                
                if (typeName.IndexOf(fileNameNoExt, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (!typeJsonMap.ContainsKey(type))
                    {
                        typeJsonMap.Add(type, filePath);
                    }
                }
            }
        }
        
        foreach (var kv in typeJsonMap)
        {
            var enumerable = dataSerialize.ClassDataParsing(kv.Key, kv.Value);
            var list = enumerable.Cast<IBaseData>().ToList();
            Dictionary<int, IBaseData> dictionary = new Dictionary<int, IBaseData>();
            foreach (var item in list)
            {
                dictionary.Add(item.Index, item);
            }
            
            databaseMap.Add(kv.Key, dictionary);
            
        }
    }
}