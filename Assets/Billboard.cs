using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _main;
    // Start is called before the first frame update
    void Start()
    {
        _main = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(_main, Vector3.up);
    }
}
