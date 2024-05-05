using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _timeToLive;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _timeToLive)
        {
            Destroy(gameObject);
        }
    }
}
