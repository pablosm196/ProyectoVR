using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[id("Follow")]
public class FollowPlayerState: State
{
    private GameObject _player;
    private NavMeshAgent _agent;

    public override void Enter()
    {
        _player = GameObject.Find("Player");
        _agent = _gameObject.GetComponent<NavMeshAgent>();
        _agent.speed = 2.0f;
    }

    public override void Update()
    {
        _blackboard.SetObjetive(_player.transform.position);
        _agent.SetDestination(_player.transform.position);
    }

    public override void Exit()
    {
        _agent.speed = 1.0f;
    }
}
