using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private MySceneNavigation _sceneNav;

    [SerializeField]
    private float _spawnEnemyTime;
    private float _time;

    // Start is called before the first frame update
    void Start()
    {
        _time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if( _time >= _spawnEnemyTime)
        {
            _time = 0;

            GameObject zombie = Instantiate(_enemy, _player.transform.position + _player.transform.forward * 0.05f, Quaternion.identity);
            zombie.transform.LookAt(_player.transform.position);
            _sceneNav.SetAgentID(zombie.GetComponent<NavMeshAgent>());
        }
    }
}
