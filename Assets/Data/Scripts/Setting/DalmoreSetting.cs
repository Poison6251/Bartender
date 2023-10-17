using M_DB;
using UnityEngine;

[CreateAssetMenu]
public class DalmoreSetting : ScriptableObject
{

    // ȯ�漳�� ���� ���
    private static DalmoreSetting setting;
    private static DalmoreSetting Setting
    {
        get
        {
            if (setting == null)
            {
                setting = Resources.Load("ProjectSetting") as DalmoreSetting;
            }
            return setting;
        }
    }

    // ���� ���� ����
    [Header("����ǥ")]
    [SerializeField] private RecipeData makingLog;
    public static RecipeData MakingLog => Setting.makingLog;
    [Header("�ֹ����")]
    [SerializeField] private OrderList orderWindow;
    public static OrderList OrderWindowList 
    { 
        get 
        { 
            if(Setting.orderWindow == null)
            {
                Setting.orderWindow = new OrderList();
            }
            return Setting.orderWindow;
        } 
    }
    [Header("�޴���")]
    [SerializeField] private RecipeDB menu;
    public static RecipeDB DalmoreMenu => Setting.menu;
    [Header("������ DB")]
    [SerializeField] private ItemDB itemDB;
    public static ItemDB DB_Item => Setting.itemDB;
    [Header("������ DB")]
    [SerializeField] private RecipeDB recipeDB;
    public static RecipeDB DB_Recipe => Setting.recipeDB;
    [Header("ũ������ DB")]
    [SerializeField] private CraftingDB craftingDB;
    public static CraftingDB DB_Crafting => Setting.craftingDB;
    [Header("�÷��̾� ������")]
    [SerializeField] private PlayerData playerData;
    public static PlayerData PlayerData => Setting.playerData;
}
