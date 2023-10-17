using Item;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


/*
    data (x : �ܿ� �� ��)
 */
public class FluidFlowManager : BartenderData
{
    [SerializeField] private RecipeData m_Data;                                      // ���� ��ü�� ������ ������
    [SerializeField] private ItemDataComponent glassData;
    public RecipeData OwnData { get { return m_Data; } }
    public float capacity;                                          // LiquidContainer�� ��� �ִ� ��ü�� ����(0~1)
    public float volume;
    float _totalSendCapacity;
    float _wantSendCapacity;
    bool _soundDelay;
    private void Awake()
    {
        var itemData = glassData.GetItemData;

        if(itemData.GetType() == typeof(GlassItemData))
        {
            var temp = Instantiate( Resources.Load<RecipeData>("MakingLog"));
            var drink = (GlassItemData)itemData;
            Ingredients newIngredients = new Ingredients();
            newIngredients.Capacity = drink.Capacity;
            newIngredients.itemData = Instantiate(drink);
            newIngredients.modifier = ItemModifier.None;
            temp.Add(newIngredients);   
            m_Data = temp;
        }
        else if(itemData.GetType()==typeof(RecipeData))
        {
            m_Data = (RecipeData)glassData.GetItemData;
        }
        else
        {
            m_Data = new RecipeData();   
        }
        FluidRatioSetting();

        // �߰�
        // ���� �׼� ���, ���� ����
        _action = new SuccessAction(null, _successCallback, _action);
        _director = null;
        // ==
    }
    public override void AddCallback(UnityAction action, Vector3 data)
    {
        base.AddCallback(action, data);
        _wantSendCapacity = data.x;
        _totalSendCapacity = 0f;
    }
    public void FluidDrop(float capacity)
    {
        float volumn = 0;                                           // ��ü�� �� �뷮
        Ingredients temp = new Ingredients();
        foreach (var i in m_Data.data)
        {
            if (ItemData.GetGID(i.itemData.ID) == 122)          // �������� �� �� �ִ� ������ �� ���
                continue;
            volumn += i.Capacity;
        }
        foreach (var i in m_Data.data.ToList())                     // ����ȭ ��Ų �� ����
        {
            if (ItemData.GetGID(i.itemData.ID) == 122)          // �������� �� �� �ִ� ������ �� ���
                continue;
            temp.itemData = i.itemData;
            temp.modifier = i.modifier;
            temp.Capacity = i.Capacity;
            temp.Capacity *= capacity / volumn;
            (glassData.GetItemData as RecipeData)?.Subtract(temp);
        }
    }
    public void Send(FluidFlowManager target, float capacity)
    {
        float volumn = 0;                                           // ��ü�� �� �뷮
        float totalSendCapacity = 0f;
        foreach (var i in m_Data.data)
        {
            volumn += i.Capacity;
        }
        foreach(var i in m_Data.data.ToList())                      // ����ȭ ��Ų �� ����
        {
            if (ItemData.GetGID(i.itemData.ID) == 122)          // �������� �� �� �ִ� ������ �� ���
            {
                target.ReceiveItemData(i);
                continue;
            }
            Ingredients temp = new Ingredients();
            temp.itemData = i.itemData;
            temp.modifier = i.modifier;
            temp.Capacity = i.Capacity;
            temp.Capacity *= capacity / volumn;
            totalSendCapacity += temp.Capacity;
            target.ReceiveItemData(temp);
        }
        // �߰�
        // �ܿ� ���� ������ ���η� ����
        if(_action != null)
        if(target.GetComponents<ItemDataComponent>().Any(x=>
        ItemData.GetGroup( x.GetItemData.ID) == ItemGroup.Glass
        || x.GetItemData.ID == 1310000))
        {
            data.x = totalSendCapacity;
            _totalSendCapacity += totalSendCapacity;
            if(_totalSendCapacity >= _wantSendCapacity)
            {
                Success();
                _totalSendCapacity = 0f;
                _wantSendCapacity = 0f;
                _action = null;
            }
            if (!_soundDelay)
            {
                SoundManager.Instance?.PlaySound("Sound_�����,�������");
                StartCoroutine(DelaySound());
            }
        }
        // ==
    }
    IEnumerator DelaySound()
    {
        _soundDelay = true;
        yield return new WaitForSeconds(0.5f);
        _soundDelay = false;
    }
    public void ReceiveItemData(Ingredients data)
    {
        if(ItemData.GetGID(data.itemData.ID) == 122)
        {
            foreach (var i in m_Data.data)
            {
                if (i.Equals(data))
                {
                    if (i.Capacity > data.Capacity)
                    {
                        data.Capacity = i.Capacity;
                    }
                    return;
                }
            }
            (glassData.GetItemData as RecipeData)?.Add(data);
            return;
        }
        m_Data.Add(data);
        float dataVolume = 0f;
        foreach(var i in m_Data.data)
        {
            if (ItemData.GetGID(i.itemData.ID) == 122) continue;
            dataVolume += i.Capacity;
        }
        if (dataVolume > volume) FluidRatioSetting(); 
    }
    public void FluidRatioSetting()
    {
        float volumn = 0f;
        foreach (var i in m_Data.data)
        {
            if (ItemData.GetGID(i.itemData.ID) == 122) continue;
            volumn += i.Capacity;
        }
        foreach (var i in m_Data.data.ToList())                     // ����ȭ ��Ų �� ����
        {
            if (ItemData.GetGID(i.itemData.ID) == 122) continue;
            i.Capacity /= volumn;
            i.Capacity *= volume;
        }
    }
    public void DataClear()
    {
        foreach(var i in OwnData.data.ToList())
        {
            OwnData.Subtract(i);
        }
    }
}
