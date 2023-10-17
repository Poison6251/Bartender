using Item;
using UnityEngine;
using static Item.ItemData;

public class ItemDataInteractable : MonoBehaviour, IItemDataInteractable
{
    [SerializeField]
    protected RecipeData itemData;
    public RecipeData GetInteractionData()
    {
        return itemData;
    }
}
