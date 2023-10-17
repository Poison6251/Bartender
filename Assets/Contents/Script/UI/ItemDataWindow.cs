using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ItemDataComponent))]
public class ItemDataWindow : MonoBehaviour
{
    private GameObject UIWindow;                // 아이템 정보창
    private static GameObject focusObject;      // 정보창에 출력할 대상
    [SerializeField]
    public ItemDataComponent data;             // 데이터
    private IItemDataReader reader;             // 리더기
    private Text ui;                            // 텍스트 UI

    // 에디터 테스트용 출력 기능

    private void OnMouseEnter()
    {
        View(true);
    }
    private void OnMouseExit()
    {
        View(false);
    }
    private void OnDestroy()
    {
        View(false);
    }



    void Awake()
    {
        if(data==null)data = GetComponent<ItemDataComponent>();
        reader = data.GetReader();
    }
    void Update()
    {
        // 포커싱 된 아이템만 출력
        if (focusObject != gameObject)
        {
            View(false);
            return;
        }

        // 데이터 읽어오기
        ui.text = "";
        if(reader != null)
        foreach (var t in reader.Read(data.GetItemData))
            ui.text += t.ToString();
    }

    // 창 출력
    public void View(bool isView)
    {
        if(isView)
        {
            UIWindow = Instantiate(Resources.Load<GameObject>("ItemDataWindow"));

            ui = UIWindow.GetComponentInChildren<Text>();
            focusObject = gameObject;
            UIWindow.SetActive(true);
            UIWindow.transform.position = transform.position;
            enabled = true;

           
        }
        else
        {
            Destroy(UIWindow);

            enabled = false;
        }
    }
}
