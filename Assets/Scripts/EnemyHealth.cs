using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _damage;

    private AudioSource _audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            _health -= _damage;
            _audioSource.Play();
            if (_health <= 0)
                Destroy(gameObject);
        }
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
}
