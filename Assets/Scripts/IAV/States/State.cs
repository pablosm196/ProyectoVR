using UnityEngine;


public class id : System.Attribute
{
    private string _id;
    public id(string id)
    {
        _id = id;
    }

    public string ID { get { return _id; } }
}

public class State 
{
    protected GameObject _gameObject;
    protected StateMachine _machine;
    protected EnemyBlackboard _blackboard;

    public State() { }

    public virtual void Init(GameObject g, StateMachine machine, EnemyBlackboard blackboard)
    {
        _gameObject = g;
        _machine = machine;
        _blackboard = blackboard;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}
