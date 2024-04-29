using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private MRUKRoom _room;

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
            Vector3 pos = _sceneNav.RandomNavmeshLocation((Mathf.Max(_room.GetRoomBounds().size.x / 2, _room.GetRoomBounds().size.z / 2)));
            if (_room.IsPositionInRoom(pos))
            {
                GameObject zombie = Instantiate(_enemy, _sceneNav.RandomNavmeshLocation((Mathf.Max(_room.GetRoomBounds().size.x / 2, _room.GetRoomBounds().size.z / 2))), Quaternion.identity);
                zombie.transform.LookAt(_player.transform.position);
                _sceneNav.SetAgentID(zombie.GetComponent<NavMeshAgent>());
                _time = 0;
            }
        }
    }

    public void SetRoom()
    {
        _room = MRUK.Instance?.GetCurrentRoom();
        if (!_room)
        {
            Debug.Log("Espabila");
            Application.Quit();
        }
    }
}
