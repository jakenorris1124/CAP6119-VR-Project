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
            TurnedOnHandler();
        else
            TurnedOffHandler();
    }
    
    private void TurnedOffHandler()
    {
        off.Play();
        gameObject.SetActive(false);
    }

    private void TurnedOnHandler()
    {
        gameObject.SetActive(true);
        on.Play();
    }
}
