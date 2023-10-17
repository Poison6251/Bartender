using ExcelDB;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExcelDBSytemUI : MonoBehaviour
{
#if UNITY_EDITOR   
    [SerializeField] ExcelDBSystem system;
    public void DBUpdateButton(string message)
    {
        // UI �о����
        // Button;Text;Animator;Animator;Animator;Animator
        string[] items = message.Split(';');
        Button button = GameObject.Find(items[0]).GetComponent<Button>();
        Text ui = GameObject.Find(items[1]).GetComponent<Text>();
        Animator UpdateButtonAnim = GameObject.Find(items[2]).GetComponent<Animator>();
        Animator urlErrorAnim = GameObject.Find(items[3]).GetComponent<Animator>(); 
        Animator dlPathErrorAnim = GameObject.Find(items[4]).GetComponent<Animator>(); 
        Animator rsPathErrorAnim = GameObject.Find(items[5]).GetComponent<Animator>();
        // �ʱ�ȭ
        UpdateButtonAnim?.Rebind();
        urlErrorAnim?.Rebind();
        dlPathErrorAnim?.Rebind();
        UpdateButtonAnim?.Play("Anim_Loading");
        ui.text = "";
        button.interactable = false;

        // �ٿ�
        if (system.URL == "")
        {
            Error(urlErrorAnim, "url�� Ȯ���� �ּ���.");
        }
        if (system.DBFilePath == "")
        {
            Error(dlPathErrorAnim, "DownLoad Path�� Ȯ���� �ּ���");
        }
        if(system.ResourceFilePath =="")
        {
            Error(rsPathErrorAnim, "Resource Path�� Ȯ���� �ּ���");
        }
        if (system.URL == "" || system.DBFilePath == "" || system.ResourceFilePath == "") return;
        if (!system.UpdateDB())
        {
            Error(urlErrorAnim, "url�� Ȯ���� �ּ���.\n���� ������ ������ �ּ���.");   
        }
        else
        {
            UpdateButtonAnim?.Play("Anim_Clear");
            ui.text = "�ٿ�ε� �Ϸ�.";
            ui.color = new Color(253, 253, 253);
            button.interactable = true;
        }
        // ����
        void Error(Animator errorAnim,string errorMessage)
        {
            if (ui.text != "") ui.text += '\n';
            ui.text += errorMessage;
            ui.color = Color.red;
            errorAnim?.Play("Anim_inputFieldError");
            UpdateButtonAnim?.Play("Anim_Error");
            
            StartCoroutine(DelayAction(() => { button.interactable = true; }, urlErrorAnim.GetCurrentAnimatorClipInfo(0).Length));
        }
    }

    public void DBReadButton(string message)
    {
        string[] items = message.Split(';');
        Button button = GameObject.Find(items[0]).GetComponent<Button>();
        Animator readButtonAnim = GameObject.Find(items[1]).GetComponent<Animator>();
        Text ui = GameObject.Find(items[2]).GetComponent<Text>();
        ui.text = "";
        button.interactable = false;
        readButtonAnim.Rebind();
        readButtonAnim?.Play("Anim_Loading");
        

        if(system.ResourceFilePath == "" || system.ResourceFilePath == null)
        {
            var anim = GameObject.Find("Resource Path").GetComponent<Animator>();
            anim.Play("Anim_inputFieldError");
            ui.text += "Resource Path�� Ȯ���� �ּ���";
            ui.color = Color.red;
            readButtonAnim?.Play("Anim_Error");
            StartCoroutine(DelayAction(() => { button.interactable = true; }, anim.GetCurrentAnimatorClipInfo(0).Length));
            return;
        }
        var tables = system.ReadDB();
        if (tables != null)
        {
            // ���̺� ������ ����
            var contents = GameObject.Find("Table Contents").transform;
            var origin = contents.GetChild(0);

            for(int i =0; i < contents.childCount; i++)
            {
                contents.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < tables.Count; i++)
            {
                Transform content = contents.transform.childCount <= i? 
                content = Instantiate(origin, contents): content = contents.GetChild(i);
                content.gameObject.SetActive(true);
                content.GetChild(1).GetComponent<Text>().text  = tables[i].Item2;
                var inputField = content.GetChild(4).GetComponent<TMP_InputField>();
                var generateButton = content.GetChild(3).GetComponent<Button>();
                generateButton.onClick.RemoveAllListeners();
                int count = i;
                generateButton.onClick.AddListener(() => 
                {
                    var anim = generateButton.GetComponent<Animator>();
                    anim.Play("Anim_Loading");
                    anim.Rebind();
                    var type = inputField.text;
                    generateButton.interactable = false;
                    if(system.Generate(tables, count, Type.GetType(type)))
                    {
                        anim.Play("Anim_Clear");
                    }
                    else
                    {
                        anim.Play("Anim_Error");
                    }
                    generateButton.interactable = true;


                });
            }

            readButtonAnim?.Play("Anim_Clear");
            ui.color = new Color(253, 253, 253);
            ui.text = "�ε� �Ϸ�";
            button.interactable = true;
        }
        else
        {
            ui.text = "�ε� ����";
            ui.color = Color.red;
            readButtonAnim?.Play("Anim_Error");
            StartCoroutine(DelayAction(() => { button.interactable = true; }, readButtonAnim.GetCurrentAnimatorClipInfo(0).Length));
        }
    }
    IEnumerator DelayAction(UnityAction action,float time)
    {
        yield return new WaitForSeconds(time+0.5f);
        action?.Invoke();
    }

    public void UpdateURL(TMP_InputField input)
    {
        system.URL = input.text;
    }
    public void UpdateDownloadPath(TMP_InputField input)
    {
        system.DBFilePath = input.text;
    }
    public void UpdateResourcePath(TMP_InputField input)
    {
        system.ResourceFilePath = input.text;
    }
    public void UpdateTypeName(TMP_InputField input)
    {
        var type = Type.GetType(input.text);
        if (type == null)
        {
            type = Type.GetType("Item." + input.text);
            input.text = type == null ?  input.text:"Item." + input.text;
        }


        if(type == null) 
        {
            var anim = input.GetComponent<Animator>();
            anim.Play("Anim_inputFieldError");
            input.DeactivateInputField();
            DelayAction(() => { input.ActivateInputField(); }, anim.GetCurrentAnimatorClipInfo(0).Length);
        }
    }
#endif
}
