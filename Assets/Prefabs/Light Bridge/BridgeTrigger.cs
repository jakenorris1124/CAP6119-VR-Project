using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    private bool _triggered = false;
    

    private void OnTriggerEnter(Collider other)
    {
        UpdateState(true, other);
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateState(true, other);
    }

    private void OnTriggerExit(Collider other)
    {
        UpdateState(false, other);
    }

    private void UpdateState(bool state, Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        _triggered = state;
    }

    public bool Triggered()
    {
        return _triggered;
    }
}
