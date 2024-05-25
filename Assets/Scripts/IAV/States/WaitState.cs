using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("Wait")]
public class WaitState : State
{
    private float _seconds;
    public float Seconds { get { return _seconds; } }

    public float elapsedTime { get; private set; }

    public override void Enter()
    {
        elapsedTime = 0;
       _seconds = _blackboard.Seconds;
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
    }
}
