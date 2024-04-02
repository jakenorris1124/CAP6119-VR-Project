using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBridge : MonoBehaviour
{
    private BridgeTrigger _solidTrigger;
    private BridgeTrigger _passTrigger;

    private MeshCollider _solidCollider;

    private bool _lockout = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _solidTrigger = gameObject.transform.Find("Solid Trigger").GetComponent<BridgeTrigger>();
        _solidCollider = gameObject.transform.Find("Solid").GetComponent<MeshCollider>();
        
        _passTrigger = gameObject.transform.Find("Passthrough Trigger").GetComponent<BridgeTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TryLock()
    {
        if (_passTrigger.Triggered() && !_solidTrigger.Triggered())
            _lockout = true;
    }

    private void TryUnlock()
    {
        if (!_lockout && (!_passTrigger.Triggered() && !_solidTrigger.Triggered()))
            _lockout = false;
    }

    private void ModifyCollisionState()
    {
        if (_lockout)
        {
            _solidCollider.enabled = false;
            return;
        }


        if (_solidTrigger.Triggered())
            _solidCollider.enabled = true;

    }
}
