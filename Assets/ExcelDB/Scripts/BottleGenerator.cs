using Item;
using System.IO;
using UnityEditor;
using UnityEngine;
using static DalmoreSetting;
public class BottleGenerator : PrefabGenerator<GlassItemData>
{
#if UNITY_EDITOR
    protected override bool Generate()
    {
        if (!base.Generate()) return false;
        foreach (var data in m_DataList)
        {
            var obj = PrefabUtility.InstantiatePrefab(m_origin) as GameObject;
            obj.name = data.ID.ToString();
            if (DB_Item.TryGetItemData(data.ID, out ItemData item))
            {
                var glass = item as GlassItemData;
                obj.transform.GetChild(0).GetComponent<MeshFilter>().mesh = glass.Mesh;
                obj.transform.GetChild(2).GetComponent<MeshFilter>().mesh = glass.LiquidMesh;
                obj.GetComponent<ItemDataComponent>().ItemDataOrigin = glass;
            }
            PrefabUtility.SaveAsPrefabAsset(obj, Path.Combine(m_PrefabFolderPath, data.ID.ToString() + ".prefab"));
            GameObject.DestroyImmediate(obj);
        }
        return true;
    }
#endif
}
