using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent triggered;
    [SerializeField] private bool triggerOnce = false;
    private bool _hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            triggered.Invoke();
            if (triggerOnce)
                _hasTriggered = true;
        }
    }
}
