using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavigateRandomPoint : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private float _timeChangeLocation;
    private float _timer;

    private NavMeshAgent _agent;
    private MRUKRoom _room;

    // Start is called before the first frame update
    void Start()
    {
        _timer = _timeChangeLocation;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _room = MRUK.Instance?.GetCurrentRoom();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if( _timer >= _timeChangeLocation)
        {
            Vector3 newPos = RandomNavmeshLocation();
            if(_room.IsPositionInRoom(newPos))
            {
                _timer = 0;
                _agent.SetDestination(newPos);
            }
        }
    }

    public Vector3 RandomNavmeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, _radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
