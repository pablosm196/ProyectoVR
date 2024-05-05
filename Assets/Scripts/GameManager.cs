using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private List<Transform> _spawnPoints;

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
            if(SceneManager.GetActiveScene().name == "Mixta")
            {
                if(_room == null)
                {
                    Debug.Log("No hay habitación");                
                    return;
                }
                Vector3 pos = _sceneNav.RandomNavmeshLocation(Mathf.Max(_room.GetRoomBounds().size.x, _room.GetRoomBounds().size.z), _player.transform.position);
                if (_room.IsPositionInRoom(pos))
                {
                    GameObject zombie = Instantiate(_enemy, pos, Quaternion.identity);
                    zombie.transform.LookAt(_player.transform.position);
                    _sceneNav.SetAgentID(zombie.GetComponent<NavMeshAgent>());
                    _time = 0;
                }
            }
            else
            {
                    Transform transform = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                    GameObject zombie = Instantiate(_enemy, transform.position, Quaternion.identity);
                    zombie.transform.LookAt(_player.transform.position);
                    _sceneNav.SetAgentID(zombie.GetComponent<NavMeshAgent>());
                    _time = 0;
            }
        }
    }

    public void SetRoom()
    {
        string s = "";
        for(int i = 0; i < _sceneNav.transform.childCount; i++)
            s += _sceneNav.transform.GetChild(i).name + "\n";
        Debug.Log(s);
        Debug.Log("Entra");
        _room = MRUK.Instance?.GetCurrentRoom();
        if (!_room)
        {
            Debug.Log("Espabila");
            Application.Quit();
        }
    }
}
