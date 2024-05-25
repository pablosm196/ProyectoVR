using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellPoint : MonoBehaviour
{
    public float Intensity { get { return _collider.radius; } }
    [SerializeField]
    private float _maxIntensity;
    [SerializeField]
    private float _minIntensity;
    private SphereCollider _collider;

    public void ResetIntensity()
    {
        _collider.radius = _maxIntensity;
    }
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = _maxIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        _collider.radius -= Time.deltaTime;
        if(_collider.radius < _minIntensity ) 
        {
            Destroy(gameObject);
        }
    }
}
