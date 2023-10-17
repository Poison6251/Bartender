using Item;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/CountItemData")]
public class CountableItemData : ItemData
{
    [SerializeField] protected uint _count =1;
}