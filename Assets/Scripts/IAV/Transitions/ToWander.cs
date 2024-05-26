using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ToWander")]
public class ToWander : Transition
{
    public override bool Check()
    {
        return _blackboard.ActualState == EnemyBlackboard.State.WANDER;
    }
}
