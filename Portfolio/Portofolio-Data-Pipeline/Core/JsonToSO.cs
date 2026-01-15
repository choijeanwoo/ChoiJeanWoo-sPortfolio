using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Common.Interfaces;
using System.Text;


public class JsonToSO
{
    public interface ILoadFromJson
    {
        void LoadFromJson(string jsonPath);
    }

    public interface IIndexedData<T> where T : IBaseData
    {
        T FindByIndex(int index);
    }
}

public static class DataClassGenerator
{
    #if UNITY_EDITOR
    public static string GenerateClassFromType(Type sourceType, string className)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using Common.Interfaces;");
        sb.AppendLine();
        sb.AppendLine("[Serializable]");
        sb.AppendLine($"public partial class {className} : IBaseData");
        sb.AppendLine("{");

        var fields = sourceType
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => (f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                        && !f.IsStatic
                        && f.GetCustomAttribute<NonSerializedAttribute>() == null
                        && !f.Name.Contains("k__BackingField"))
            .ToArray();

        foreach (var field in fields)
        {
            string fieldName = field.Name;
            Type fieldType = field.FieldType;
            string fieldTypeName = GetCleanTypeName(fieldType);

            if (fieldName.ToLower() == "index")
            {
                sb.AppendLine($"    [SerializeField] private int index;");
                sb.AppendLine($"    public int Index");
                sb.AppendLine("    {");
                sb.AppendLine("        get => index;");
                sb.AppendLine("        set => index = value;");
                sb.AppendLine("    }");
                sb.AppendLine();
                continue;
            }

            sb.AppendLine($"    public {fieldTypeName} {fieldName};");
            if (string.Equals(fieldName, "spritePath", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine($"    [NonSerialized] public Sprite sprite;");
            }
            sb.AppendLine();
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetCleanTypeName(Type type)
    {
        if (type == typeof(int) || type == typeof(System.Int32)) return "int";
        if (type == typeof(long) || type == typeof(System.Int64)) return "long";
        if (type == typeof(string) || type == typeof(System.String)) return "string";
        if (type == typeof(float) || type == typeof(System.Single)) return "float";
        if (type == typeof(double) || type == typeof(System.Double)) return "double";
        if (type == typeof(bool) || type == typeof(System.Boolean)) return "bool";

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type innerType = type.GetGenericArguments()[0];
            return GetCleanTypeName(innerType) + "[]";
        }

        return type.Name;
    }
    #endif
}