using M_DB;
using UnityEngine;

namespace Item
{
    public enum ItemGroup { None, Bottle, Glass, Cocktail, Garnish, Recipie, Tool }
    public abstract class ItemData : ScriptableObject, IQueryableToDB
    {
        [SerializeField] protected uint _id; public uint ID => _id;
        [TextArea, SerializeField] protected string _name; public string Name => _name;
        [TextArea, SerializeField] protected string _discription; public string Discription
                                                                         => _discription;
        [SerializeField] protected uint _price; public uint Price => _price;
        [SerializeField] protected GameObject _prefab; public GameObject Prefab => _prefab;
        [SerializeField] protected Mesh _mesh; public Mesh Mesh => _mesh;
        [SerializeField] protected Material _material; public Material Material => _material;
        [SerializeField] protected Texture2D _previewTexture; public Texture PreviewTexture => _previewTexture;
        public static uint GetGID(uint id)
        {
            return id / 10000;
        }
        public static ItemGroup GetGroup(uint id)
        {
            var gid = GetGID(id);

            if (gid == 120 || gid == 121) return ItemGroup.Bottle;
            if (gid == 122) return ItemGroup.Garnish;
            if (gid == 130) return ItemGroup.Glass;
            if (gid == 131) return ItemGroup.Tool;
            if (gid == 220) return ItemGroup.Cocktail;
            if (gid == 320) return ItemGroup.Recipie;

            return ItemGroup.None;
        }

        public abstract class ModifierItemData : ItemData
        {
            public ItemModifier Modifier;
        }

        public interface IItemDataInteractable
        {
            public RecipeData GetInteractionData();
        }
    }
}
