using Item;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorial
{
    public class Curriculum : MonoBehaviour
    {
        [SerializeField]
        RACGetter _racGetter;                                  // ������ ������ �˻��ϴ� �����
        [SerializeField]
        List<ST_GO> _tools = new List<ST_GO>();                // ������ �˻��ϴ� �����
        [SerializeField]
        Animator _teacherModel, _scoopModel;                  // ������ ���ϰ� �� �������� ��
        [SerializeField]
        TextMeshProUGUI _ui;                                   // ������ ������ ����� UI

        Queue<Teacher> _chapters = new Queue<Teacher>();               // <��ü ���۵� �� �ϳ�> �� ������� ������ ���
        List<GameObject> _spawnObject = new List<GameObject>();        // ������ ������Ʈ�� �����ϱ� ���� �����
        GameObject _glass = null;                               // ��ȯ�� ��
        BartenderData _prevCallback;

        public void CurriculumStart(string recipeName)
        {
            var curriculumData = DalmoreSetting.DalmoreMenu.GetList.Find(x => x.Discription == recipeName);
            if (curriculumData == null) return;
            // �ʱ�ȭ
            _chapters.Clear();
            _spawnObject?.ForEach(x => Destroy(x));
            _spawnObject.Clear();
            _tools.ForEach(x => x.value.SetActive(false));
            _glass?.GetComponent<BreakableObject>()?.Break();
            _glass = null;
            // ������ ����
            for (int i = 0; i < curriculumData.data.Count; i++)
            {
                var chapter = curriculumData.data[i];
                if (chapter == null) continue;
                var page = new Page(chapter);
                if (page.Gesture == "Shake")
                {
                    page.Gesture += "1";
                    _chapters.Enqueue(ConvertToChapter(page));

                    // ����Ŀ�� ���� ��ü �� �־����� 2~5��
                    if (i == curriculumData.data.Count - 1 || curriculumData.data[i + 1].modifier.ToString() != "Shake")
                    {
                        page.Gesture = "Shake2";
                        _chapters.Enqueue(ConvertToChapter(page));
                        page.Gesture = "Shake3";
                        _chapters.Enqueue(ConvertToChapter(page));
                        page.Gesture = "Shake4";
                        _chapters.Enqueue(ConvertToChapter(page));
                        page.Gesture = "Shake5";
                        _chapters.Enqueue(ConvertToChapter(page));
                    }
                }
                else
                {
                    _chapters.Enqueue(ConvertToChapter(page));
                }
            }
            // �� ����
            var glassParent = _tools.Find(x => x.key == "Glass").value;
            _glass = Instantiate(curriculumData.glass, glassParent.transform);
            _glass.transform.localPosition = Vector3.zero;
            _glass.transform.eulerAngles = Vector3.zero;
            // ����
            _teacherModel.transform.parent.gameObject.SetActive(true);
            NextChapter();
        }
        [ContextMenu("Next")]
        public void NextChapter()
        {
            if (_chapters == null) return;
            bool isCleaning = true;   
            if (_chapters.Count == 0)
            {
                _ui.text = "�����ϼ̽��ϴ�.";
                return;
            }
            var teacher = _chapters.Dequeue();
            // ����Ŀ �� �� ���� ���� (�ӽ�)
            if (teacher.Gesture == "Shake2" || teacher.Gesture == "Shake3" || teacher.Gesture == "Shake4" || teacher.Gesture == "Shake5") isCleaning = false;
            if (isCleaning)
            {
                _spawnObject.ForEach(x => Destroy(x));
                _spawnObject.Clear();
                _tools.ForEach(x => x.value.SetActive(false));
            }
            
            // UI
            _ui.text = teacher.Guide;
            // ����
            teacher.Tools.ForEach(x => x.SetActive(true));
            // ��
            if (teacher.Bottle != null)
            {
                var parent = _tools.Find(x => x.key == "Bottle").value.transform;
                var obj = Instantiate(teacher.Bottle, parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                _spawnObject.Add(obj);
                teacher.Bottle = obj;
            }
            // ���
            if (teacher.Tools.Any(x => x == _tools.Find(y => y.key == "Scoop").value))
            {
                _scoopModel.runtimeAnimatorController = teacher.PreviewAnimator;
                _scoopModel.gameObject.SetActive(true);
                _teacherModel.gameObject.SetActive(false);
            }
            else if (teacher.Gesture == "Dart")
            {
                _teacherModel.runtimeAnimatorController = teacher.PreviewAnimator;
                _teacherModel.gameObject.SetActive(false);
                _scoopModel.gameObject.SetActive(false);
            }
            else
            {
                _teacherModel.runtimeAnimatorController = teacher.PreviewAnimator;
                _teacherModel.gameObject.SetActive(true);
                _scoopModel.gameObject.SetActive(false);
            }
            // ������ �ݹ� �̺�Ʈ ����
            AddCallbackNextChapter(teacher);
        }
        Teacher ConvertToChapter(Page page)
        {
            var teacher = new Teacher();
            
            // ���� ���� ����
            teacher.Tools = GetTools(page);
            teacher.PreviewAnimator = _racGetter.GetController(page.Gesture);
            teacher.Gesture = page.Gesture;

            // ���� ���� ����
            if (page.Gesture == "Scoop")
            {
                teacher.Guide = "�����۷� �����⿡�� ������ �� �ܿ� ��������.";
                teacher.PreviewAnimator = _racGetter.GetController("Scoop");
            }
            else if (page.Gesture == "Shake1")
            {
                teacher.Guide = "����Ŀ�� ";

                // ���� �ܿ� ���� ��
                if (ItemData.GetGroup(page.ID)== ItemGroup.Bottle)
                {
                    if (DalmoreSetting.DB_Item.TryGetItemData(page.ID, out ItemData data))
                    {
                        if (data.Prefab != null)
                        {
                            teacher.Guide += " " + data.Name + "(" + page.Capacity + "ml)";
                            teacher.Bottle = data.Prefab;
                            teacher.WantCapacity = page.Capacity;
                        }
                    }
                }

                teacher.Guide += "�� �ξ� �ּ���.";
            }
            else if (page.Gesture == "Shake2")
            {
                teacher.Guide = "����Ŀ�� ��Ʈ���̳ʿ� ĸ�� ���ʷ� �ݾ� �ּ���.";
            }
            else if (page.Gesture == "Shake3")
            {
                teacher.Guide = "����Ŀ�� ��� ����� ���� �ּ���.";
            }
            else if (page.Gesture == "Shake4")
            {
                teacher.Guide = "����Ŀ���� ĸ�� ��Ʈ���̳ʸ� �� �ּ���.";
            }
            else if (page.Gesture == "Shake5")
            {
                teacher.Guide = "�ܿ� ����Ŀ�� �ִ� ��ü�� �ξ� �ּ���.";
            }
            else if (page.Gesture == "Bottle")
            {
                teacher.Guide = "�ܿ� ";
                // ���� �ܿ� ���� ��
                if (ItemData.GetGroup(page.ID) == ItemGroup.Bottle)
                {
                    if (DalmoreSetting.DB_Item.TryGetItemData(page.ID, out ItemData data))
                    {
                        if (data.Prefab != null)
                        {
                            teacher.Guide += " " + data.Name + "(" + page.Capacity + "ml)";
                            teacher.Bottle = data.Prefab;
                            teacher.WantCapacity = page.Capacity;
                        }
                    }
                }
                teacher.Guide += "�� �ξ� �ּ���.";
            }
            else if(page.Gesture == "Dart")
            {
                teacher.Guide = "Throw Darts.\n\n";
                teacher.PreviewAnimator = null;
            }

            // ����
            return teacher;
        }
        List<GameObject> GetTools(Page page)
        {
            List<GameObject> tools = new List<GameObject>();
            // �� ����
            tools.Add(_tools.Find(x => x.key == "Glass").value);

            // ����ũ �� �� ���� ���
            if (page.Gesture == "Shake1")
            {
                tools.Add(_tools.Find(x => x.key == "Cobbler Shaker Body").value);
                tools.Add(_tools.Find(x => x.key == "Bottle").value);
            }
            else if (page.Gesture.Substring(0,Mathf.Clamp(page.Gesture.Length, page.Gesture.Length, 5)) == "Shake")
            {
                tools.Add(_tools.Find(x => x.key == "Cobbler Shaker Strainer").value);
                tools.Add(_tools.Find(x => x.key == "Cobbler Shaker Cap").value);
            }
            // ���� �ʿ��� �� ������, ������ ���
            else if (page.ID == 1220020)
            {
                tools.Add(_tools.Find(x => x.key == "IceMaker").value);
                tools.Add(_tools.Find(x => x.key == "IceMaker2").value);
                tools.Add(_tools.Find(x => x.key == "Scoop").value);
            }
            // �ܿ� ���� ���� ��
            else if (page.Gesture == "Bottle")
            {
                tools.Add(_tools.Find(x => x.key == "Bottle").value);
            }
            else if (page.Gesture == "Dart")
            {
                tools.Clear();
                tools.Add(_tools.Find(x => x.key == "DartPlane").value);
                tools.Add(_tools.Find(x => x.key == "DartStorage").value);
            }
            return tools;
        }
        void AddCallbackNextChapter(Teacher teacher)
        {
            _prevCallback?.ResetCallback();
            if (teacher.Gesture == "Shake1" ||   teacher.Gesture == "Bottle")
            {
                FluidFlowManager shaker = teacher.Bottle.GetComponent<FluidFlowManager>();
                Vector3 data = new Vector3();
                data.x = teacher.WantCapacity;
                shaker.AddCallback(()=>CoroutineRunner.Instance.DelayAction(0.5f,NextChapter), data);
                _prevCallback = shaker;
                
            }
            else if (teacher.Gesture == "Shake2")
            {
                AttachObject shaker = _tools.Find(x => x.key == "Cobbler Shaker Body").value.GetComponent<AttachObject>();
                Vector3 data = new Vector3();
                data.x = 1310017;
                shaker.AddCallback(() => CoroutineRunner.Instance.DelayAction(0.5f, NextChapter), data);
                _prevCallback = shaker;
            }
            else if (teacher.Gesture == "Shake3")
            {
                Shaker shaker = _tools.Find(x => x.key == "Cobbler Shaker Body").value.GetComponent<Shaker>();
                shaker.AddCallback(() => CoroutineRunner.Instance.DelayAction(0.5f, NextChapter), Vector3.zero);
                _prevCallback = shaker;
            }
            else if (teacher.Gesture == "Shake4")
            {
                AttachObject shaker = _tools.Find(x => x.key == "Cobbler Shaker Body").value.GetComponent<AttachObject>();
                Vector3 data = new Vector3();
                data.x = 1310000;
                shaker.AddCallback(() => CoroutineRunner.Instance.DelayAction(0.5f, NextChapter), data);
                _prevCallback = shaker;
            }
            else if (teacher.Gesture == "Shake5")
            {
                FluidFlowManager shaker = _tools.Find(x => x.key == "Cobbler Shaker Body").value.GetComponent<FluidFlowManager>();
                Vector3 data = new Vector3();
                data.x = teacher.WantCapacity;
                shaker.AddCallback(() => CoroutineRunner.Instance.DelayAction(0.5f, NextChapter), data);
                _prevCallback = shaker;
            }
            else if (teacher.Gesture == "Scoop")
            {
                Vector3 data = new Vector3();
                data.x = teacher.WantCapacity;
                Glass glass = _glass.GetComponent<Glass>();
                glass.AddCallback(() => CoroutineRunner.Instance.DelayAction(0.5f, NextChapter), data);
                _prevCallback = glass;

            }
        }
    }

    public struct Page
    {
        public string Gesture;
        public uint ID;
        public float Capacity;
        public Page(Ingredients curriculumData)
        {
            Gesture = curriculumData.modifier.ToString();
            ID = curriculumData.itemData.ID;
            if (ID == 1220020) Gesture = "Scoop";
            if (Gesture == "None") Gesture = "Bottle";
            Capacity = curriculumData.Capacity;
        }
        public Page(string gesture, uint id, float capacity)
        {
            Gesture = gesture;
            ID = id;
            Capacity = capacity;
        }
    }
    public struct Teacher
    {
        public RuntimeAnimatorController PreviewAnimator;   // ���õ� �������� ����� ���� �ִϸ�����
        public List<GameObject> Tools;                      // �������� ����ϴ� ������
        public string Guide;                                // �������� ����
        public GameObject Bottle;                           // ���Ǵ� ��
        public string Gesture;
        public float WantCapacity;
    }
    [System.Serializable]
    public struct ST_GO
    {
        public GameObject value;
        public string key;
    }
}