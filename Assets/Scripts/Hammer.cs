using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    private float _force;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<EnemyHealth>() != null)
        {
            collision.collider.GetComponent<Rigidbody>().AddExplosionForce(_force, transform.position, 1);
            collision.collider.GetComponent<EnemyHealth>().LowDamage(GetComponent<Damage>()._damage);
        }
    }
}
