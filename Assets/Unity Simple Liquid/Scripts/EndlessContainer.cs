using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySimpleLiquid
{
    [RequireComponent(typeof(LiquidContainer),typeof(FluidFlowManager))]
    public class EndlessContainer : MonoBehaviour
    {
        private LiquidContainer liquidContainer;
        private FluidFlowManager fluidFlowManager;

        private void Awake()
        {
            liquidContainer = GetComponent<LiquidContainer>();
            fluidFlowManager = GetComponent<FluidFlowManager>();
        }

        void Update()
        {
            liquidContainer.FillAmountPercent = 1f;
            fluidFlowManager.FluidRatioSetting();
        }
    }
}

