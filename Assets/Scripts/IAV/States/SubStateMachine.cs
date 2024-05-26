using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[id("SubStateMachine")]
public class SubStateMachine : State
{
    private StateMachineDefinition _definition;

    private State _current;

    private Dictionary<string, State> _states;

    private Dictionary<State, List<Transition>> _transitions;

    private int _index;

    private bool _cycle;

    private bool _exit;

    public bool exit { get { return _exit; } }

    public void SetDefinition(StateMachineDefinition def)
    {
        _definition = def;
    }

    public override void Enter()
    {
        _states = new Dictionary<string, State>();
        _transitions = new Dictionary<State, List<Transition>>();

        _cycle = _definition.cycle;
        _exit = false;
        _index = 0;


        for (int i = 0; i < _definition.states.Length; ++i)
        {
            State state = StateMachineFactory.Instance.GetState(_definition.states[i]);
            _states.Add(_definition.names[i], state);
            if (state is SubStateMachine)
            {
                SubStateMachine substate = (SubStateMachine)state;
                substate.SetDefinition(_gameObject.GetComponent<EnemyBlackboard>().GetDefinition(_definition.names[i]));
            }
            state.Init(_gameObject, _machine, _gameObject.GetComponent<EnemyBlackboard>());
            _transitions.Add(state, new List<Transition>());

        }

        for (int i = 0; i < _definition.origin.Length; i++)
        {
            Transition transition = StateMachineFactory.Instance.GetTransition(_definition.transitions[i]);
            _transitions[_states[_definition.origin[i]]].Add(transition);
            transition._origin = _states[_definition.origin[i]];
            transition._dest = _states[_definition.destination[i]];
            transition.Init(_gameObject, _machine, _gameObject.GetComponent<EnemyBlackboard>());

        }
        _current = _states[_definition.states[0]];
        _current.Enter();
    }

    // Update is called once per frame
    public override void Update()
    {
        _current.Update();

        int i = 0;
        while (i < _transitions[_current].Count && !_transitions[_current][i].Check()) ++i;
        if (i != _transitions[_current].Count)
        {
            _current.Exit();
            if (_index < _transitions.Count - 1 || _cycle)
            {
                _current = _transitions[_current][i]._dest;
                _current.Enter();
                _index++;
            }
            else
                _exit = true;
        }
    }

    public override void Exit()
    {
        _blackboard.StartWander();
    }
}
