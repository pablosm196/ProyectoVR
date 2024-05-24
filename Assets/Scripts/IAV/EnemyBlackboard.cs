using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlackboard : MonoBehaviour
{
    [SerializeField]
    private float _seconds;
    public float Seconds { get { return _seconds; } }
    public StateMachineDefinition[] _defs;

    public StateMachineDefinition GetDefinition(string name)
    {
        int i = 0;
        while (i < _defs.Length && _defs[i].name != name) ++i;
        return i == _defs.Length ? null : _defs[i];
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_defs[0].name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
