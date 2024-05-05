using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int _health;

    private UIManager _uiManager;


    private AudioSource _audioSource;



    private void OnCollisionEnter(Collision collision)
    {
        Damage damage = collision.collider.GetComponent<Damage>();
        if (damage != null && collision.collider.GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            Destroy(collision.gameObject);
            _health -= damage._damage;
            _audioSource.Play();
            if (_health <= 0)
            {
                Destroy(gameObject);
                if(SceneManager.GetActiveScene().name == "Aldea_")
                    _uiManager.UpdateZombies();
            }
        }
    }

    public void LowDamage(int damage)
    {
        _health -= damage;
        _audioSource.Play();
        if (_health <= 0)
        {
            Destroy(gameObject);
            if (SceneManager.GetActiveScene().name == "Aldea_")
                _uiManager.UpdateZombies();
        }
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(SceneManager.GetActiveScene().name == "Aldea_")
            _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }
}
