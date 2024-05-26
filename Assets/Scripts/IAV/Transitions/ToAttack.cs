using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ToAttack")]
public class ToAttack : Transition
{
    public override bool Check()
    {
        return _blackboard.ActualState == EnemyBlackboard.State.ATTACKING;
    }
}
