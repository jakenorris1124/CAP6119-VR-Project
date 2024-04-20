using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicToggle : MonoBehaviour
{
    [SerializeField] private bool state = true;
    
    public void Toggle()
    {
        state = !state;

        gameObject.SetActive(state);
    }
}
