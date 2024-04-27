using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GameObject _bullet;
    [SerializeField]
    private float _timeToShoot;
    private float _elapsedTime;
    [SerializeField]
    private float _speed;

    private void Start()
    {
        _elapsedTime = _timeToShoot;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= _timeToShoot && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            _elapsedTime = 0;
            GameObject bullet = Instantiate(_bullet, transform.position, _bullet.transform.rotation);
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity = _speed * transform.forward;
            bulletRB.useGravity = true;
        }
    }
}
