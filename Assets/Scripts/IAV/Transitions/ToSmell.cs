using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ToSmell")]
public class WanderToSmell : Transition
{
    public override bool Check()
    {
        return _blackboard.ActualState == EnemyBlackboard.State.SMELLING;
    }
}
