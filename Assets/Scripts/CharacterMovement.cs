using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController _myCharacterController;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private float _timeToRotate;
    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _myCharacterController = GetComponent<CharacterController>();
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 dir = Vector3.zero;

        dir += _camera.forward * input.y + _camera.right * input.x;

        _myCharacterController.SimpleMove(dir * _speed);

        if(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < 0 && _timer > _timeToRotate)
        {
            transform.Rotate(new Vector3(0, -90, 0));
            _timer = 0;
        }
        else if(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0 && _timer > _timeToRotate)
        {
            transform.Rotate(new Vector3(0, 90, 0));
            _timer = 0;
        }
    }
}
