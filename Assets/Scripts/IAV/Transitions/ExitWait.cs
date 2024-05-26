using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ExitWait")]
public class ExitWait : Transition
{
    public override bool Check()
    {
        WaitState wait = (WaitState)_origin;
        return wait.elapsedTime > wait.Seconds;
    }
}
