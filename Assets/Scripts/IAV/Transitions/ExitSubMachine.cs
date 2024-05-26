using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ExitSubMachine")]
public class ExitSubMachine : Transition
{
    public override bool Check() 
    {
        SubStateMachine subState = (SubStateMachine)_origin;
        return subState.exit;
    }
}
