using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("SubStateMachine")]
public class SubStateMachine : State
{
    private StateMachineDefinition _machineDefinition;

    public void SetDefinition(StateMachineDefinition def)
    {
        _machineDefinition = def;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
