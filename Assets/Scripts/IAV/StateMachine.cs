using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    private StateMachineDefinition _definition;

    private State _current;

    private Dictionary<string, State> _states;

    private Dictionary<State, List<Transition>> _transitions;

    // Start is called before the first frame update
    void Start()
    {   
        _states = new Dictionary<string, State>();
        _transitions = new Dictionary<State, List<Transition>>();


        for(int i = 0; i <_definition.states.Length; ++i)
        {
            State state = StateMachineFactory.Instance.GetState(_definition.states[i]);
            _states.Add(_definition.names[i], state);
            if(state is SubStateMachine)
            {
                SubStateMachine substate = (SubStateMachine)state;
                substate.SetDefinition(GetComponent<EnemyBlackboard>().GetDefinition(_definition.names[i]));
            }
            state.Init(gameObject, this, GetComponent<EnemyBlackboard>());
            _transitions.Add(state, new List<Transition>());

        }

        for(int i = 0; i < _definition.origin.Length; i++)
        {
            _transitions[_states[_definition.origin[i]]].Add(StateMachineFactory.Instance.GetTransition(_definition.transitions[i]));
            _transitions[_states[_definition.origin[i]]][i]._origin = _states[_definition.origin[i]];
            _transitions[_states[_definition.origin[i]]][i]._dest = _states[_definition.destination[i]];
        }
        _current = _states[_definition.states[0]];
        _current.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        _current.Update();

        int i = 0;
        while (i < _transitions[_current].Count && !_transitions[_current][i].Check()) ++i;
        if (i != _transitions[_current].Count)
        {
            _current.Exit();
            _current = _transitions[_current][i]._dest;
            _current.Enter();
        }
    }
}
