using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ItemDataComponent))]
public class ItemDataWindow : MonoBehaviour
{
    private GameObject UIWindow;                // ������ ����â
    private static GameObject focusObject;      // ����â�� ����� ���
    [SerializeField]
    public ItemDataComponent data;             // ������
    private IItemDataReader reader;             // ������
    private Text ui;                            // �ؽ�Ʈ UI

    // ������ �׽�Ʈ�� ��� ���

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
        // ��Ŀ�� �� �����۸� ���
        if (focusObject != gameObject)
        {
            View(false);
            return;
        }

        // ������ �о����
        ui.text = "";
        if(reader != null)
        foreach (var t in reader.Read(data.GetItemData))
            ui.text += t.ToString();
    }

    // â ���
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
