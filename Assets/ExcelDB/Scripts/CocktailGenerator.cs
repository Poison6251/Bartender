using Item;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnitySimpleLiquid;
using static DalmoreSetting;

public class CocktailGenerator : PrefabGenerator<GlassItemData>
{
    #if UNITY_EDITOR   
    public void UpdatePFPath()
    {
        var path = GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text;
        m_PrefabFolderPath = path;
        GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "";
        GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.white;
        if (string.IsNullOrEmpty(path))
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "PrefabFolderPath�� �Է��� �ּ���.";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("PrefabFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
            return;
        }
        if (!Directory.Exists(path))
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "PrefabFolderPath�� ������ �����ϴ�.";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("PrefabFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
            return;
        }
    }
    public void UpdateDFPath()
    {
        var path = GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text;
        m_DataFolderPath = path;
        GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "";
        GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.white;
        if (string.IsNullOrEmpty(path))
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "DataFolderPath�� �Է��� �ּ���.";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("DataFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
            return;
        }
        if (!Directory.Exists(path))
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "DataFolderPath�� ������ �����ϴ�.";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("DataFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
            return;
        }
    }
    public void StartGenerate()
    {
        GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "";
        GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.white;
        // ��ǲ �ʵ� �Է� ���� ����
        if (string.IsNullOrEmpty(m_DataFolderPath))
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "DataFolderPath�� �Է��� �ּ���.";
            GameObject.Find("DataFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
        }
        if (string.IsNullOrEmpty(m_PrefabFolderPath))
        {
            if (GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text != "")
                GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "\n";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "PrefabFolderPath�� �Է��� �ּ���.";
            GameObject.Find("PrefabFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
        }
        if (string.IsNullOrEmpty(m_PrefabFolderPath) || string.IsNullOrEmpty(m_DataFolderPath)) return;

        // ��ǲ �ʵ� �߸��� �� �Է� ����
        if (!Directory.Exists(m_DataFolderPath))
        {
            if (GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text != "")
                GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "\n";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "DataFolderPath�� ������ �����ϴ�.";
            GameObject.Find("DataFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
        }
        if (!Directory.Exists(m_PrefabFolderPath))
        {
            if (GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text != "")
                GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "\n";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "PrefabFolderPath�� ������ �����ϴ�.";
            GameObject.Find("PrefabFolder Path").GetComponent<Animator>()?.Play("Anim_inputFieldError");
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
        }
        if (!(Directory.Exists(m_DataFolderPath) && Directory.Exists(m_PrefabFolderPath))) return;
        if(m_origin==null)
        {
            if (GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text != "")
                GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "\n";
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text += "������ �������� �־� �ּ���.";
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
            return;
        }
        if (Generate()) 
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "�Ϸ�.";
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Clear");
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.white;
        }
        else
        {
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().text = "����.";
            GameObject.Find("���׷���Ʈ Anim").GetComponent<Animator>()?.Play("Anim_Error");
            GameObject.Find("���׷���Ʈ �ؽ�Ʈ").GetComponent<Text>().color = Color.red;
        }
    }
    [ContextMenu("d")]
    protected override bool Generate()
    {
        if (!base.Generate()) return false;
        foreach(GlassItemData data in m_DataList)
        {
            // ������ ��������
            var obj = PrefabUtility.InstantiatePrefab(m_origin) as GameObject;
            if(obj == null) continue;

            // ���� �̸�
            obj.name = data.ID.ToString();

            // ������ �Է�
            var liquidContainer = obj.GetComponent<LiquidContainer>();
            liquidContainer.LiquidColor = data.LiquidColor;

            var glass = data;
            var splitCtr = obj.transform.GetComponent<SplitController>();
            var radius_height = CalculateMaxRadius(glass.Mesh);
            // �޽�
            obj.transform.GetChild(0).GetComponent<MeshFilter>().mesh = glass.Mesh;
            obj.transform.GetChild(2).GetComponent<MeshFilter>().mesh = glass.LiquidMesh;
            // ��ü
            liquidContainer.FillAmountPercent = data.Capacity/ glass.Capacity;
            liquidContainer.Volume = glass.Capacity;
            // �����
            splitCtr.BottleneckRadius = radius_height.x;
            splitCtr.height = radius_height.y*0.01f;
            // ������ ������
            obj.GetComponent<ItemDataComponent>().ItemDataOrigin = glass;
            obj.GetComponent<MeshCollider>().sharedMesh = data.Mesh;
            if(transform.childCount == 4)
            {
                obj.transform.GetChild(3).GetComponent<MeshRenderer>().material = data.LabelMaterial;
                obj.transform.GetChild(3).GetComponent<MeshFilter>().mesh = data.LabelMesh;
            }

            // ����
            PrefabUtility.SaveAsPrefabAsset(obj, Path.Combine(m_PrefabFolderPath, data.ID.ToString() + ".prefab"));

            // ����
            GameObject.DestroyImmediate(obj);
        }
        return true;

    }

    private Vector2 CalculateMaxRadius(Mesh mesh)
    {
        if (mesh == null) return Vector2.zero;
        Vector3[] vertices = mesh.vertices;
        var list = vertices.Select(x => x.y).Distinct().ToList();
        if (list == null || list.Count == 0) return Vector2.zero;
        list.Sort((a, b) => b.CompareTo(a));
        var radius = FindFarthestDistance(vertices.Where(x => x.y == list.First()).ToList());
        return new Vector2(radius, list.First());


        float FindFarthestDistance(List < Vector3 > points)
        {
            float farthestDistance = points
                .SelectMany((p1, index1) => points.Skip(index1 + 1).Select(p2 => Vector3.Distance(p1, p2)))
                .Max();

            return farthestDistance/2f;
        }
    }
#endif
}
