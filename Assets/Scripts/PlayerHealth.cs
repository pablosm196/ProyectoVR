using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<EnemyHealth>() != null)
        {
            levelManager.ChangeScene("Menu");
        }
    }
}
