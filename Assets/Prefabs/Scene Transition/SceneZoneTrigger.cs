using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneZoneTrigger : MonoBehaviour
{
    private LocalSceneManager _localSceneManager;

    private bool triggered = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _localSceneManager = GameObject.Find("Scene Manager").GetComponent<LocalSceneManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            _localSceneManager.EndScene();
            triggered = true;
        }
    }
}
