using M_DB;
using UnityEngine;

[CreateAssetMenu]
public class DalmoreSetting : ScriptableObject
{

    // 환경설정 파일 경로
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

    // 전역 변수 설정
    [Header("성분표")]
    [SerializeField] private RecipeData makingLog;
    public static RecipeData MakingLog => Setting.makingLog;
    [Header("주문목록")]
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
    [Header("메뉴판")]
    [SerializeField] private RecipeDB menu;
    public static RecipeDB DalmoreMenu => Setting.menu;
    [Header("아이템 DB")]
    [SerializeField] private ItemDB itemDB;
    public static ItemDB DB_Item => Setting.itemDB;
    [Header("레시피 DB")]
    [SerializeField] private RecipeDB recipeDB;
    public static RecipeDB DB_Recipe => Setting.recipeDB;
    [Header("크래프팅 DB")]
    [SerializeField] private CraftingDB craftingDB;
    public static CraftingDB DB_Crafting => Setting.craftingDB;
    [Header("플레이어 데이터")]
    [SerializeField] private PlayerData playerData;
    public static PlayerData PlayerData => Setting.playerData;
}
