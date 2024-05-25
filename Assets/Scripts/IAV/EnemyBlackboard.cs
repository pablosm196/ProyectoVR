using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBlackboard : MonoBehaviour
{
    [SerializeField]
    private float _seconds;
    public float Seconds { get { return _seconds; } }
    [SerializeField]
    private float _distance;
    public float Distance { get { return _distance; } }

    public StateMachineDefinition[] _defs;
    private bool _listening;
    private bool _smelling;
    private bool _attacking;
    private Vector3 _objetive;
    public Vector3 Objetive {  get { return _objetive; } }
    private SmellPoint _smellPoint;
    private GameObject _player;

    public StateMachineDefinition GetDefinition(string name)
    {
        int i = 0;
        while (i < _defs.Length && _defs[i].name != name) ++i;
        return i == _defs.Length ? null : _defs[i];
    }

    public void StartListening(Vector3 pos)
    {
        _listening = true;
        _smelling = _attacking = false;
        _objetive = pos;
    }

    public void StartSmelling(SmellPoint point)
    {
        _smelling = true;
        _listening = _attacking = false;
        UpdateSmell(point);
        _objetive = _smellPoint.transform.position;
    }

    public void StartAttacking()
    {
        _attacking = true;
        _listening = _smelling = false;
        _objetive = _player.transform.position;
    }

    public void SetObjetive(Vector3 pos)
    {
        _objetive = pos;
    }

    public void UpdateSmell(SmellPoint _other)
    {
        if(_smellPoint == null ||
            _other.Intensity / Vector3.Distance(_other.transform.position, transform.position) >
            _smellPoint.Intensity / Vector3.Distance(_smellPoint.transform.position, transform.position))
        {
            _smellPoint = _other;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _listening = _smelling = _attacking = false;
        _objetive = Vector3.zero;
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
