using System;
using UnityEngine;


public enum EFlowState
{
    none,title, bar,tutorial
}

public class FlowManager : MonoBehaviour
{
    private IFlow m_flow = null;
    private EFlowState m_state = EFlowState.none;
    public EFlowState GetState
    {
        get
        {
            return m_state;
        }
    }
    //[SerializeField] private Flow title, bar, make, store;
    [SerializeField] private SerializableObject<IFlow> title, bar,tutorial;
    public void ChangeFlow(EFlowState goToFlow)
    {
        m_flow?.Exit();
        m_flow = GetFlow(goToFlow);
        m_state = goToFlow;
        m_flow.Enter();
    }

    private IFlow GetFlow(EFlowState goToFlow)
    {

        switch (goToFlow) 
        {
            case EFlowState.title:
                return title.Ref;
            case EFlowState.bar:
                return bar.Ref;
            case EFlowState.tutorial:
                return tutorial.Ref;
            default:
                return null;
        }

        
    }


}


