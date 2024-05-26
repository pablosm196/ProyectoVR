using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smell : MonoBehaviour
{
    [SerializeField]
    private float _timeToSmell;
    private float _time;
    [SerializeField]
    private float _maxDistance;
    [SerializeField]
    private GameObject _point;
    private GameObject _lastPoint;

    // Start is called before the first frame update
    void Start()
    {
        _lastPoint = null;
        _time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if(_time >= _timeToSmell)
        {
            _time = 0;
            if(_lastPoint == null || Vector3.Distance(transform.position, _lastPoint.transform.position) > _maxDistance)
            {
                _lastPoint = Instantiate(_point, transform.position, Quaternion.identity);
            }
            else
            {
                _lastPoint.GetComponent<SmellPoint>().ResetIntensity();
            }
        }
    }
}
