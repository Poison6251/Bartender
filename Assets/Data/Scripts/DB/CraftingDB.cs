using Item;
using M_DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "DB/CraftingDB")]
public class CraftingDB : ScriptableObject
{
    [SerializeField] private List<CraftingRecipe> recipes;
    public List<CraftingRecipe> GetList => recipes.ToList();
}

[Serializable]
public struct CraftingRecipe : IQueryableToDB
{
    public uint A;
    public uint B;
    public uint ID => A;
    public uint Result;
}
