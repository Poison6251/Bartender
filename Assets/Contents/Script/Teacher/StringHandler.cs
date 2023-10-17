using System.Collections.Generic;
using TMPro;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class StringHandler : MonoBehaviour
{
    [SerializeField]    Dropdown    m_dropdown;
    [SerializeField]    RecipeDB        m_menu;         // ���� ������ ������ ���
    [SerializeField]    Curriculum      m_curriculum;   // ������ ���� �Ŵ���
    string m_data;
    private void Start()
    {
        m_dropdown.onValueChanged.AddListener(ChangeString);
        ChangeString(0);
        GetComponent<Button>().onClick.AddListener(SendString);
    }
    public void SendString()
    {
        m_curriculum.CurriculumStart(m_data);
    }
    public void ChangeString(int index)
    {
        print(m_dropdown.options[index].text);
        m_data = m_dropdown.options[index].text;
    }
    public void SetDropdown()
    {
        if (m_dropdown == null || m_menu == null) return;
        // ��Ӵٿ ������ �߰�
        List<string> list = new List<string>();
        if (m_menu.GetList.Count == 0) list.Add("�޴� ����");
        else m_menu.GetList.ForEach(x => list.Add(x.Discription));
        m_dropdown.ClearOptions();
        m_dropdown.AddOptions(list);
    }

}
