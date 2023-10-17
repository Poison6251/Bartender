using UnityEngine;
using UnitySimpleLiquid;

/*
    [채널]
    data 변수 (x : x축 인력; y : y축 인력; z : z축 인력;)
 */

public class Shaker : BartenderData
{
    [SerializeField] ParticleSystem _shakeEffect;
    [SerializeField] private float limitPower;                  // 인력이 limitPower 이상이면 셰이크 발생
    [SerializeField] private float successCount;                // 셰이크 해야 하는 횟수
    private Vector3 totalVector;                                // 누적 벡터
    private Vector3 prevPos;                                    // 이전 프레임 벡터
    private int shakeCount;                                     // 셰이크 한 횟수
    private LiquidContainer liquid;
    private AttachObject attach;
    private FluidFlowManager fluidFlowManager;
    private bool isEnable;
    protected void Awake()
    {
        // 어태치 정보를 가져온다.
        attach =GetComponent<AttachObject>();

        // 액체를 동작시킬 참조를 가져온다.
        liquid = GetComponent<LiquidContainer>();
        fluidFlowManager = GetComponent<FluidFlowManager>();

        // 수식어 액션과 셰이커 연출을 추가한다.
        _action = new SuccessModifierAction(gameObject, _successCallback,_action);
        _director = new ShakerDirector(gameObject, _shakeEffect);
    }
    private void OnDisable()
    {
        // 액체를 초기화한다.
        if(GetComponent<LiquidContainer>()!=null)
        GetComponent<LiquidContainer>().FillAmountPercent = 0;
        if(GetComponent<ItemDataComponent>() !=null)
        GetComponent<ItemDataComponent>().Init();
    }
    public void Enable(bool enable)
    {
        // 셰이커의 활성 상태를 변경한다.
        if (isEnable == enable) return;

        // 셰이커를 초기화하고 동작시킨다.
        isEnable = enable;
        liquid.IsOpen = !enable;
        if (enable)
        {
            totalVector = Vector3.zero;
            prevPos = gameObject.transform.position;
            data = Vector3.zero;
            shakeCount = 0;
        }
    }
    private void Update()
    {
        // 셰이커가 최종형태인지 확인한다.
#if UNITY_EDITOR
        Enable(true);
#else
        Enable(attach.AttachedID == 1310017);
#endif
        if (!isEnable) return;

        // 관성을 계산해서 임계치를 넘기면 1회 셰이크를 발생시킨다.
        Vector3 currentVector = gameObject.transform.position - prevPos;
        Vector4 inertia = PowerCalculate(totalVector, currentVector);
        if (inertia.w > limitPower)
        {
            Shake(inertia);
            totalVector = Vector3.zero;
        }

        // 현재 위치를 업데이트한다.
        prevPos = gameObject.transform.position;
    }
    private Vector4 PowerCalculate(Vector3 prevPower,Vector3 currentPower)
    {
        float[] result = new float[3];
        Vector2[] temp = new Vector2[3];

        // 각 축의 관성을 계산한다.
        temp[0] = GetInertia(prevPower.x, currentPower.x);
        temp[1] = GetInertia(prevPower.y, currentPower.y);
        temp[2] = GetInertia(prevPower.z, currentPower.z);

        // 최종 관성을 계산한다.
        float power = temp[0].y + temp[1].y + temp[2].y;
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].y < 0)
            {
                result[i] = temp[i].x * Mathf.Abs(temp[i].y);
                continue;
            }
            result[i] = temp[i].x * temp[i].y;
        }

        // xyz에는 각 축의 벡터, w에는 최종 관성을 반환한다.
        return new Vector4(result[0], result[1], result[2],power);
    }
    private Vector2 GetInertia(float prev,float current)
    {
        Vector2 result;
        // 힘의 방향을 담는다.
        result.x = current < 0 ? -1 : 1;
        // 힘의 크기를 담는다.
        result.y = Mathf.Abs(current - prev);
        if(prev * current > 0)
        {
            result.y *= -1;
        }
        // xy에 각각 방향,크기를 반환한다.
        return result;
    }
    private void Shake(Vector4 inertia)
    {
        // 셰이크한 횟수를 갱신한다.
        shakeCount++;
        // 관성 정보를 넘긴다.
        data = inertia;
        // 데이터 전송
        SendData();
        // 성공 확인
        if (shakeCount == successCount)
        {
            Success();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ice")
        {
            fluidFlowManager.OwnData.Add(other.gameObject.GetComponent<DalmoreDropInteraction>().Ingredient);
            other.gameObject.GetComponent<DalmoreDropInteraction>().dropEvent?.Invoke();
        }
    }
    public void WashShaker()
    {
        //PurityValue = purityMaxValue;
    }
}
