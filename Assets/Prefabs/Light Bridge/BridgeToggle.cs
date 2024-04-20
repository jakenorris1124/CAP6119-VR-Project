using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeToggle : MonoBehaviour
{
    private bool state = true;

    [SerializeField] private AudioSource on;
    [SerializeField] private AudioSource off;

    [SerializeField] private bool startDisabled;

    private GameObject _passthrough;
    private MeshRenderer _passthroughMesh;
    
    private GameObject _solid;
    private MeshRenderer _solidMesh;
    private MeshCollider _solidCollider;

    private void Start()
    {
        _passthrough = gameObject.transform.Find("Passthrough").gameObject;
        _passthroughMesh = _passthrough.GetComponent<MeshRenderer>();
        
        
        _solid = gameObject.transform.Find("Solid").gameObject;
        _solidMesh = _solid.GetComponent<MeshRenderer>();
        _solidCollider = _solid.GetComponent<MeshCollider>();

        if (startDisabled)
        {
            state = false;
            Off(false);
        }
    }


    public void Toggle()
    {
        state = !state;

        if (state)
            On();
        else
            Off();
    }

    public void Off(bool sound = true)
    {
        if (sound)
            off.Play();


        _passthroughMesh.enabled = false;

        _solidMesh.enabled = false;
        _solidCollider.enabled = false;
    }

    public void On()
    {
        gameObject.SetActive(true);
        
        _passthroughMesh.enabled = true;

        _solidMesh.enabled = true;
        _solidCollider.enabled = true;
        
        on.Play();
    }
}
