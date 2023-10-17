using Item;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PrefabGenerator<T> : MonoBehaviour where T : ScriptableObject
{
#if UNITY_EDITOR   
    [SerializeField] protected GameObject m_origin;
    public string m_PrefabFolderPath;
    public string m_DataFolderPath;
    protected List<T> m_DataList = new List<T>();
    protected virtual bool Generate()
    {
        if (m_origin == null) return false;
        if (IsNullOrNotFound(m_PrefabFolderPath) || IsNullOrNotFound(m_DataFolderPath)) return false;

        var assetList = Directory.GetFiles(m_DataFolderPath).Where(x=>Path.GetExtension(x)==".asset").Select(x => Path.GetFileName(x));
        m_DataList.Clear();

        foreach(var asset in assetList)
        {
            m_DataList.Add(AssetDatabase.LoadAssetAtPath<T>(Path.Combine(m_DataFolderPath, asset)));
        }
        if (m_DataList.Count == 0) return false;
        return true;
    }

    bool IsNullOrNotFound(string path) => string.IsNullOrEmpty(path) && !Directory.Exists(path);
#endif
}
