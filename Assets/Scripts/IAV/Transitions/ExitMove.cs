using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ExitMove")]
public class ExitMove : Transition
{
    public override bool Check()
    {
        return Vector3.Distance(_gameObject.transform.position, _blackboard.Objetive) <= 0.1;
    }
}
