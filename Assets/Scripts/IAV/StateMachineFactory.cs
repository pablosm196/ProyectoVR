using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachineFactory : MonoBehaviour
{
    private static StateMachineFactory _instance;
    public static StateMachineFactory Instance { get { return _instance; } }

    private Dictionary<string, System.Type> _states;
    private Dictionary<string, System.Type> _transitions;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;

        _states = new Dictionary<string, System.Type>();
        _transitions = new Dictionary<string, System.Type>();

        foreach (System.Type type in GetType().Assembly.GetTypes())
        {
            foreach (System.Attribute attribute in System.Attribute.GetCustomAttributes(type))
            {
                if (attribute is id ID)
                {
                    if (type.BaseType == typeof(State))
                        _states.Add(ID.ID, type);
                    else if (type.BaseType == typeof(Transition))
                        _transitions.Add(ID.ID, type);
                }
            }
        }
    }


    public State GetState(string name)
    {
        ConstructorInfo ctor = _states[name].GetConstructor(new System.Type[0]);
        State instance = (State)ctor.Invoke(new System.Type[0]);
        return instance;
    }

    public Transition GetTransition(string name)
    {
        ConstructorInfo ctor = _transitions[name].GetConstructor(new System.Type[0]);
        Transition instance = (Transition)ctor.Invoke(new System.Type[0]);
        return instance;
    }
}
