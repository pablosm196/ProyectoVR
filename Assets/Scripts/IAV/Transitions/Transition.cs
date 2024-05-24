using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    protected GameObject _gameObject;
    protected StateMachine _machine;

    public State _origin;
    public State _dest;
    
    public Transition() { }

    public virtual void Init(GameObject g, StateMachine machine)
    {
        _gameObject = g;
        _machine = machine;
    }
    public virtual bool Check() { return true; }
}
