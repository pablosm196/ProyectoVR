using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField]
    private Transform _visionPoint;
    [SerializeField]
    private float _maxDistance;
    [SerializeField]
    private float _width;
    [SerializeField]
    private float _height;

    private EnemyBlackboard _blackboard;

    // Start is called before the first frame update
    void Start()
    {
        _blackboard = GetComponent<EnemyBlackboard>();
    }

    [SerializeField]
    private LayerMask _layerMask;
    private void FixedUpdate()
    {
        Vector3 pos = new Vector3(transform.position.x, _visionPoint.position.y, transform.position.z);
        Collider[] obj = Physics.OverlapBox(pos + transform.forward * _maxDistance / 2, new Vector3(_width / 2, _height / 2, _maxDistance / 2), Quaternion.identity, _layerMask);
        if (obj.Length > 0 )
            _blackboard.StartAttacking();
        else if (_blackboard.ActualState == EnemyBlackboard.State.ATTACKING)
            _blackboard.StartWander();
    }
}
