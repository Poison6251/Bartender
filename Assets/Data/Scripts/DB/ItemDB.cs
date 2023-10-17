using Item;
using UnityEngine;

namespace M_DB
{
    [CreateAssetMenu(menuName = "DB/ItemDB")]
    public class ItemDB : DB<ItemData>
    {
        public override void AddItem(ItemData item) { }

        public override void RemoveItem(ItemData item) { }
    }
    
}