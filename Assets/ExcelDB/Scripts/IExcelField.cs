using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ExcelDB
{
#if UNITY_EDITOR
    public interface IExcelField
    {
        public object Read(string data);
        public string FieldName { get; set; }
    }
    public abstract class ExcelField : IExcelField
    {
        public static IExcelField GetField(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            Type fieldType = field?.FieldType;

            // 타입 반환 (필드 종류에 따라 추가해 줘야 함
            if (fieldType == typeof(string)) return new stringField();
            else if (fieldType == typeof(System.Int32)) return new intField();
            else if (fieldType == typeof(System.UInt32)) return new uintField();
            else if (fieldType == typeof(System.Single)) return new floatField();
            else if (fieldType == typeof(Texture2D)) return new Texture2DField();
            else if (fieldType == typeof(Color)) return new ColorField();
            else if (fieldType == typeof(GameObject)) return new GameObjectField();
            else if (fieldType == typeof(Mesh)) return new MeshField();
            else if (fieldType == typeof(Material)) return new MaterialField();
            else return null;
            //
        }
        public abstract object Read(string data);
        public string FieldName { get; set; }
    }

    #region example
    public class uintField : ExcelField
    {
        public override object Read(string data)
        {
            try
            {
                if (data == null || data == "") return (uint)0;
                return uint.Parse(data);
            }
            catch
            {
                return (uint)0;
            }
        }
    }
    public class stringField : ExcelField
    {
        public override object Read(string data)
        {
            return data;
        }
    }
    public class intField : ExcelField
    {
        public override object Read(string data)
        {
            try
            {
                if (data == null || data == "") return 0;
                return int.Parse(data);
            }
            catch
            {
                return 0;
            }
        }
    }
    public class floatField : ExcelField
    {
        public override object Read(string data)
        {
            try
            {
                if (data == null || data == "") return 0f;
                return float.Parse(data);
            }
            catch
            {
                return 0f;
            }

        }

    }
    public class Texture2DField : ExcelField
    {
        public override object Read(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return AssetDatabase.LoadAssetAtPath(data,typeof( Texture2D));
        }
    }
    public class ColorField : ExcelField
    {
        public override object Read(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length !=6) return Color.magenta;
            byte r = byte.Parse(data.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(data.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(data.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r/255f, g/255f, b/255f);
        }
    }
    public class GameObjectField : ExcelField
    {
        public override object Read(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return AssetDatabase.LoadAssetAtPath<GameObject>(data);
        }
    }
    public class MeshField : ExcelField
    {
        public override object Read(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            Mesh mesh = null;
            if (Path.GetExtension(data) == ".mesh")
                mesh = AssetDatabase.LoadAssetAtPath<Mesh>(data);
            return mesh;
        }
    }
    public class MaterialField : ExcelField
    {
        public override object Read(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return AssetDatabase.LoadAssetAtPath<Material>(data);
        }
    }
    #endregion
#endif
}