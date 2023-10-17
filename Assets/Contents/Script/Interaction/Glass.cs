using UnityEngine;
using UnityEngine.Events;

public class Glass : BartenderData
{
    private FluidFlowManager fluidFlowManager;
    int _addIceCount;
    int _wantIceCount;

    void Awake()
    {
        fluidFlowManager = GetComponent<FluidFlowManager>();
        _action = new SuccessAction(gameObject, _successCallback);
        _director = null;
    }
    public override void AddCallback(UnityAction action, Vector3 data)
    {
        base.AddCallback(action, data);
        _wantIceCount = (int)data.x;
        _addIceCount = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            fluidFlowManager.OwnData.Add(other.gameObject.GetComponent<DalmoreDropInteraction>().Ingredient);
            other.gameObject.GetComponent<DalmoreDropInteraction>().dropEvent?.Invoke();
            SoundManager.Instance?.PlaySound("Sound_¾óÀ½ÄÅ");
            if(_addIceCount >= _wantIceCount)
            {
                Success();
                _addIceCount = 0;
                _wantIceCount = int.MaxValue;
                ResetCallback();
            }
        }
    }
}
