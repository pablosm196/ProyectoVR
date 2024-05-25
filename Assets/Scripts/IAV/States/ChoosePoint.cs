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
    // Start is called before the first frame update
    public override void Enter()
    {
        _position = Vector3.zero;
    }

    // Update is called once per frame
    public override void Update()
    {
        _position = RandomNavmeshLocation(_blackboard.Distance, _gameObject.transform.position);
    }

    public override void Exit()
    {
        _blackboard.SetObjetive(_position);
    }

    private Vector3 RandomNavmeshLocation(float radius, Vector3 pos = new Vector3())
    {
        Vector2 ran = Random.insideUnitCircle * radius;
        Vector2 randomDirection = new Vector3(pos.x + ran.x, 0, pos.z + ran.y);
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
