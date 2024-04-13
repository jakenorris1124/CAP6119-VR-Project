using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeToggle : MonoBehaviour
{
    private bool state = true;

    [SerializeField] private AudioSource on;
    [SerializeField] private AudioSource off;
    

    public void Toggle()
    {
        state = !state;

        if (state)
            On();
        else
            Off();
    }
    
    public void Off()
    {
        off.Play();
        gameObject.SetActive(false);
    }

    public void On()
    {
        gameObject.SetActive(true);
        on.Play();
    }
}
