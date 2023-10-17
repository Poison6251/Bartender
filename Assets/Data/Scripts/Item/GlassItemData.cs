using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item 
{
    [CreateAssetMenu(menuName ="ItemData/GlassItemData")]
    public class GlassItemData : ItemData
    {
        [SerializeField] protected uint _glass; public uint Glass => _glass;
        [SerializeField] protected Color _liquidColor; public Color LiquidColor => _liquidColor;
        [SerializeField] protected GameObject _breakingPrefab; public GameObject BreakingPrefab => _breakingPrefab;
        [SerializeField] protected Mesh _labelMesh; public Mesh LabelMesh => _labelMesh;
        [SerializeField] protected Material _labelMaterial; public Material LabelMaterial => _labelMaterial;
        [SerializeField] protected Mesh _liquidMesh; public Mesh LiquidMesh => _liquidMesh;
        [SerializeField] protected float _ml; public float Capacity =>_ml;
    }
}


