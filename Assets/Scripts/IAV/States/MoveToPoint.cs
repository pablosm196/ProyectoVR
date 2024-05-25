using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[id("MovePoint")]
public class MoveToPoint : State
{
    private NavMeshAgent _agent;


    public override void Enter()
    {
        _agent = _gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    public override void Update()
    {
        _agent.SetDestination(_blackboard.Objetive);
    }

    public override void Exit()
    {
        _blackboard.SetObjetive(Vector3.zero);
    }
}
