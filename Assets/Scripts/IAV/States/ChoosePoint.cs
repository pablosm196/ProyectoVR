using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

[id("Choose")]
public class ChoosePoint : State
{
    private Vector3 _position;
    public Vector3 Position { get { return _position; } }
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    public override void Enter()
    {
        _position = Vector3.zero;
        _agent = _gameObject.GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        _position = RandomNavmeshLocation(_blackboard.Distance, _gameObject.transform.position);
        if (!_agent.CalculatePath(_position, _agent.path))
            _position = Vector3.zero;
        else
            _blackboard.SetObjetive(_position);
    }


    private Vector3 RandomNavmeshLocation(float radius, Vector3 pos = new Vector3())
    {
        Vector3 randomPoint = pos + Random.insideUnitSphere * radius;
        Vector3 finalPosition = Vector3.zero;
        for (int i = 0; i < 30; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
            }
        }
        return finalPosition;
    }
}
