using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ToListen")]
public class ToListen : Transition
{
    public override bool Check()
    {
        return _blackboard.ActualState == EnemyBlackboard.State.LISTENING;
    }
}
