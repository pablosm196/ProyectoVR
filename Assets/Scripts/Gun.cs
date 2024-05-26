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
    [SerializeField]
    LevelManager _levelManager;
    [SerializeField]
    private float _rangeSound;

    private AudioSource _audioSource;

    private void Shoot()
    {
        _audioSource.Play();
        _elapsedTime = 0;
        GameObject bullet = Instantiate(_bullet, transform.position + (transform.forward * 0.2f), _bullet.transform.rotation);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.velocity = _speed * transform.forward;
        bulletRB.useGravity = true;

        Collider[] objs = Physics.OverlapSphere(transform.position, _rangeSound);

        foreach (Collider obj in objs)
        {
            if(obj.GetComponent<EnemyBlackboard>() != null)
            {
                obj.GetComponent<EnemyBlackboard>().StartListening(transform.position);
            }
        }
    }

    private void Start()
    {
        _elapsedTime = _timeToShoot;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= _timeToShoot && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Shoot();
        }

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            _levelManager.ChangeScene("Menu");
        }
    }
}
