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
        RACGetter _racGetter;                                  // 조교의 동작을 검색하는 저장소
        [SerializeField]
        List<ST_GO> _tools = new List<ST_GO>();                // 도구를 검색하는 저장소
        [SerializeField]
        Animator _teacherModel, _scoopModel;                  // 동작을 취하게 할 조교님의 모델
        [SerializeField]
        TextMeshProUGUI _ui;                                   // 조교의 설명을 출력한 UI

        Queue<Teacher> _chapters = new Queue<Teacher>();               // <전체 동작들 중 하나> 를 순서대로 저장한 목록
        List<GameObject> _spawnObject = new List<GameObject>();        // 스폰한 오브젝트들 제거하기 위한 저장소
        GameObject _glass = null;                               // 소환된 잔
        BartenderData _prevCallback;

        public void CurriculumStart(string recipeName)
        {
            var curriculumData = DalmoreSetting.DalmoreMenu.GetList.Find(x => x.Discription == recipeName);
            if (curriculumData == null) return;
            // 초기화
            _chapters.Clear();
            _spawnObject?.ForEach(x => Destroy(x));
            _spawnObject.Clear();
            _tools.ForEach(x => x.value.SetActive(false));
            _glass?.GetComponent<BreakableObject>()?.Break();
            _glass = null;
            // 데이터 생성
            for (int i = 0; i < curriculumData.data.Count; i++)
            {
                var chapter = curriculumData.data[i];
                if (chapter == null) continue;
                var page = new Page(chapter);
                if (page.Gesture == "Shake")
                {
                    page.Gesture += "1";
                    _chapters.Enqueue(ConvertToChapter(page));

                    // 셰이커에 넣을 액체 다 넣었으면 2~5번
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
            // 잔 생성
            var glassParent = _tools.Find(x => x.key == "Glass").value;
            _glass = Instantiate(curriculumData.glass, glassParent.transform);
            _glass.transform.localPosition = Vector3.zero;
            _glass.transform.eulerAngles = Vector3.zero;
            // 시작
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
                _ui.text = "수고하셨습니다.";
                return;
            }
            var teacher = _chapters.Dequeue();
            // 셰이커 일 땐 도구 유지 (임시)
            if (teacher.Gesture == "Shake2" || teacher.Gesture == "Shake3" || teacher.Gesture == "Shake4" || teacher.Gesture == "Shake5") isCleaning = false;
            if (isCleaning)
            {
                _spawnObject.ForEach(x => Destroy(x));
                _spawnObject.Clear();
                _tools.ForEach(x => x.value.SetActive(false));
            }
            
            // UI
            _ui.text = teacher.Guide;
            // 도구
            teacher.Tools.ForEach(x => x.SetActive(true));
            // 술
            if (teacher.Bottle != null)
            {
                var parent = _tools.Find(x => x.key == "Bottle").value.transform;
                var obj = Instantiate(teacher.Bottle, parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                _spawnObject.Add(obj);
                teacher.Bottle = obj;
            }
            // 모션
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
            // 성공시 콜백 이벤트 연결
            AddCallbackNextChapter(teacher);
        }
        Teacher ConvertToChapter(Page page)
        {
            var teacher = new Teacher();
            
            // 공통 정보 저장
            teacher.Tools = GetTools(page);
            teacher.PreviewAnimator = _racGetter.GetController(page.Gesture);
            teacher.Gesture = page.Gesture;

            // 개별 정보 저장
            if (page.Gesture == "Scoop")
            {
                teacher.Guide = "스쿠퍼로 제빙기에서 얼음을 퍼 잔에 넣으세요.";
                teacher.PreviewAnimator = _racGetter.GetController("Scoop");
            }
            else if (page.Gesture == "Shake1")
            {
                teacher.Guide = "셰이커에 ";

                // 술만 잔에 따를 때
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

                teacher.Guide += "를 부어 주세요.";
            }
            else if (page.Gesture == "Shake2")
            {
                teacher.Guide = "셰이커에 스트레이너와 캡을 차례로 닫아 주세요.";
            }
            else if (page.Gesture == "Shake3")
            {
                teacher.Guide = "셰이커를 잡고 충분히 흔들어 주세요.";
            }
            else if (page.Gesture == "Shake4")
            {
                teacher.Guide = "셰이커에서 캡과 스트레이너를 빼 주세요.";
            }
            else if (page.Gesture == "Shake5")
            {
                teacher.Guide = "잔에 셰이커에 있는 액체를 부어 주세요.";
            }
            else if (page.Gesture == "Bottle")
            {
                teacher.Guide = "잔에 ";
                // 술만 잔에 따를 때
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
                teacher.Guide += "를 부어 주세요.";
            }
            else if(page.Gesture == "Dart")
            {
                teacher.Guide = "Throw Darts.\n\n";
                teacher.PreviewAnimator = null;
            }

            // 리턴
            return teacher;
        }
        List<GameObject> GetTools(Page page)
        {
            List<GameObject> tools = new List<GameObject>();
            // 잔 고정
            tools.Add(_tools.Find(x => x.key == "Glass").value);

            // 셰이크 할 때 도구 사용
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
            // 얼음 필요할 때 스쿠퍼, 제빙기 사용
            else if (page.ID == 1220020)
            {
                tools.Add(_tools.Find(x => x.key == "IceMaker").value);
                tools.Add(_tools.Find(x => x.key == "IceMaker2").value);
                tools.Add(_tools.Find(x => x.key == "Scoop").value);
            }
            // 잔에 술만 따를 때
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
        public RuntimeAnimatorController PreviewAnimator;   // 숙련된 조교님의 모습을 담은 애니메이터
        public List<GameObject> Tools;                      // 조교님이 사용하는 도구들
        public string Guide;                                // 조교님의 설명
        public GameObject Bottle;                           // 사용되는 술
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