using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tiempo;
    [SerializeField]
    private TextMeshProUGUI _zombies;

    private int _nZombies;
    private float _timer;

    public void UpdateZombies()
    {
        _nZombies++;
        _zombies.text = "Zombies: " + _nZombies;
    }

    // Start is called before the first frame update
    void Start()
    {
        _nZombies = 0;
        _timer = 0;
        _tiempo.text = "Tiempo: 0";
        _zombies.text = "Zombies: 0";
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        _tiempo.text = "Tiempo: " + (int)_timer;
    }
}
