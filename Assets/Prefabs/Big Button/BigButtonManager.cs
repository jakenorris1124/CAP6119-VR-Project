using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BigButtonManager : MonoBehaviour
{
    [SerializeField] private Material deactive;
    [SerializeField] private Material active;

    [SerializeField] private UnityEvent[] buttonPressed;
    [SerializeField] private UnityEvent[] buttonUnpressed;
    
    private bool _pressed;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Friend Cube") || _pressed)
            return;
        
        _pressed = true;
        SetMaterial(active);
        
        foreach (UnityEvent subscriber in buttonPressed) 
            subscriber.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Friend Cube") || !_pressed)
        {
            return;
        }

        _pressed = false;
        SetMaterial(deactive);
        
        foreach (UnityEvent subscriber in buttonUnpressed)
            subscriber.Invoke();
    }

    private void SetMaterial(Material material)
    {
        Material[] mats = gameObject.GetComponent<MeshRenderer>().materials;

        mats[0] = new Material(material);

        gameObject.GetComponent<MeshRenderer>().materials = mats;
    }
}
