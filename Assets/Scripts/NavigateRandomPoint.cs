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
    private MySceneNavigation _sceneNav;
    private MRUKRoom _room;

    // Start is called before the first frame update
    void Start()
    {
        _timer = _timeChangeLocation;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _room = MRUK.Instance?.GetCurrentRoom();
        _sceneNav = GameObject.Find("NavMesh").GetComponent<MySceneNavigation>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if( _timer >= _timeChangeLocation)
        {
            Vector3 newPos = _sceneNav.RandomNavmeshLocation(_radius);
            if(_room.IsPositionInRoom(newPos))
            {
                _timer = 0;
                _agent.SetDestination(newPos);
            }
        }
    }
}
