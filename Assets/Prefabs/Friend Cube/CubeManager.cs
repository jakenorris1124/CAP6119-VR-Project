using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    private Vector3 _spawn;
    // Start is called before the first frame update
    void Start()
    {
        _spawn = gameObject.transform.position;
    }


    public void Respawn()
    {
        gameObject.transform.position = _spawn;
    }
}
