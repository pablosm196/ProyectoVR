using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[id("Wait")]
public class WaitState : State
{
    private float _seconds;
    public float Seconds { get { return _seconds; } }
    private NavMeshAgent _agent;

    public float elapsedTime { get; private set; }

    public override void Enter()
    {
        elapsedTime = 0;
       _seconds = _blackboard.Seconds;
        _agent = _gameObject.GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
        _blackboard.SetObjetive(Vector3.zero);
        _blackboard.StartWander();
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public override void Exit()
    {
        _agent.isStopped = false;  
    }
}
