using UnityEngine;

public class FlowChanger : MonoBehaviour
{
    public static FlowChanger Instance { get; private set; }
    private FlowManager flowManager;

    private void Awake()
    {
        flowManager = GetComponent<FlowManager>();
        if (Instance == null)
        {
            Instance = this;
            flowManager.ChangeFlow(EFlowState.title);
        }
    }

    [ContextMenu("TitleToBar")]
    public void TitleToBar()
    {
        if (flowManager.GetState == EFlowState.title)
        {
            flowManager.ChangeFlow(EFlowState.bar);
        }
    }
    [ContextMenu("BarToTitle")]
    public void BarToTitle()
    {
        if (flowManager.GetState == EFlowState.bar)
        {
            flowManager.ChangeFlow(EFlowState.title);
        }
    }
    [ContextMenu("TitleToTutorial")]
    public void TitleToTutorial()
    {
        if (flowManager.GetState == EFlowState.title)
        {
            flowManager.ChangeFlow(EFlowState.tutorial);
        }
    }
    [ContextMenu("TutorialToBar")]
    public void TutorialToBar()
    {
        if (flowManager.GetState == EFlowState.tutorial)
        {
            flowManager.ChangeFlow(EFlowState.bar);
        }
    }
    [ContextMenu("TutorialToTitle")]
    public void TutorialToTitle()
    {
        if (flowManager.GetState == EFlowState.tutorial)
        {
            flowManager.ChangeFlow(EFlowState.title);
        }
    }
}
