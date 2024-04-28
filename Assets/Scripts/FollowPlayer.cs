using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private Transform _player;
    private NavMeshAgent _agent;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        _agent.SetDestination(_player.position);
    }
}
