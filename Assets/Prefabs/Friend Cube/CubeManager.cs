using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeManager : MonoBehaviour
{
    private Vector3 _spawn;
    private AudioSource[] _collide;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawn = gameObject.transform.position;
        
        _collide = gameObject.GetComponents<AudioSource>();
    }


    public void Respawn()
    {
        gameObject.transform.position = _spawn;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Level Geometry"));
        {
            int index = Random.Range(0, _collide.Length);

            Vector3 velocity = other.relativeVelocity.normalized;

            _collide[index].volume *= velocity.magnitude;
            _collide[index].Play();

            _collide[index].volume = 1f;
        }
    }
}
