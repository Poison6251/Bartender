using M_DB;
using UnityEngine;

[CreateAssetMenu(menuName = "DB/RecipeDB")]
public class RecipeDB : DB<RecipeData>
{
    public override void AddItem(RecipeData item) { }

    public override void RemoveItem(RecipeData item) { }
}
