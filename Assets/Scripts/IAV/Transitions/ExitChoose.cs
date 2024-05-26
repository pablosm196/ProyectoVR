using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("ExitChoose")]
public class ExitChoose : Transition
{
    public override bool Check()
    {
        ChoosePoint choose = (ChoosePoint)_origin;
        return choose.Position != Vector3.zero;
    }
}
