using UnityEngine;
using UnitySimpleLiquid;

/*
    [ä��]
    data ���� (x : x�� �η�; y : y�� �η�; z : z�� �η�;)
 */

public class Shaker : BartenderData
{
    [SerializeField] ParticleSystem _shakeEffect;
    [SerializeField] private float limitPower;                  // �η��� limitPower �̻��̸� ����ũ �߻�
    [SerializeField] private float successCount;                // ����ũ �ؾ� �ϴ� Ƚ��
    private Vector3 totalVector;                                // ���� ����
    private Vector3 prevPos;                                    // ���� ������ ����
    private int shakeCount;                                     // ����ũ �� Ƚ��
    private LiquidContainer liquid;
    private AttachObject attach;
    private FluidFlowManager fluidFlowManager;
    private bool isEnable;
    protected void Awake()
    {
        // ����ġ ������ �����´�.
        attach =GetComponent<AttachObject>();

        // ��ü�� ���۽�ų ������ �����´�.
        liquid = GetComponent<LiquidContainer>();
        fluidFlowManager = GetComponent<FluidFlowManager>();

        // ���ľ� �׼ǰ� ����Ŀ ������ �߰��Ѵ�.
        _action = new SuccessModifierAction(gameObject, _successCallback,_action);
        _director = new ShakerDirector(gameObject, _shakeEffect);
    }
    private void OnDisable()
    {
        // ��ü�� �ʱ�ȭ�Ѵ�.
        if(GetComponent<LiquidContainer>()!=null)
        GetComponent<LiquidContainer>().FillAmountPercent = 0;
        if(GetComponent<ItemDataComponent>() !=null)
        GetComponent<ItemDataComponent>().Init();
    }
    public void Enable(bool enable)
    {
        // ����Ŀ�� Ȱ�� ���¸� �����Ѵ�.
        if (isEnable == enable) return;

        // ����Ŀ�� �ʱ�ȭ�ϰ� ���۽�Ų��.
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
        // ����Ŀ�� ������������ Ȯ���Ѵ�.
#if UNITY_EDITOR
        Enable(true);
#else
        Enable(attach.AttachedID == 1310017);
#endif
        if (!isEnable) return;

        // ������ ����ؼ� �Ӱ�ġ�� �ѱ�� 1ȸ ����ũ�� �߻���Ų��.
        Vector3 currentVector = gameObject.transform.position - prevPos;
        Vector4 inertia = PowerCalculate(totalVector, currentVector);
        if (inertia.w > limitPower)
        {
            Shake(inertia);
            totalVector = Vector3.zero;
        }

        // ���� ��ġ�� ������Ʈ�Ѵ�.
        prevPos = gameObject.transform.position;
    }
    private Vector4 PowerCalculate(Vector3 prevPower,Vector3 currentPower)
    {
        float[] result = new float[3];
        Vector2[] temp = new Vector2[3];

        // �� ���� ������ ����Ѵ�.
        temp[0] = GetInertia(prevPower.x, currentPower.x);
        temp[1] = GetInertia(prevPower.y, currentPower.y);
        temp[2] = GetInertia(prevPower.z, currentPower.z);

        // ���� ������ ����Ѵ�.
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

        // xyz���� �� ���� ����, w���� ���� ������ ��ȯ�Ѵ�.
        return new Vector4(result[0], result[1], result[2],power);
    }
    private Vector2 GetInertia(float prev,float current)
    {
        Vector2 result;
        // ���� ������ ��´�.
        result.x = current < 0 ? -1 : 1;
        // ���� ũ�⸦ ��´�.
        result.y = Mathf.Abs(current - prev);
        if(prev * current > 0)
        {
            result.y *= -1;
        }
        // xy�� ���� ����,ũ�⸦ ��ȯ�Ѵ�.
        return result;
    }
    private void Shake(Vector4 inertia)
    {
        // ����ũ�� Ƚ���� �����Ѵ�.
        shakeCount++;
        // ���� ������ �ѱ��.
        data = inertia;
        // ������ ����
        SendData();
        // ���� Ȯ��
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
