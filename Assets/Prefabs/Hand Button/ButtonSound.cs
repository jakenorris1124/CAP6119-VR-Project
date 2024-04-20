using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ButtonSound : MonoBehaviour
{
    private AudioSource[] _press;

    private void Start()
    {
        _press = gameObject.GetComponents<AudioSource>();
    }

    public void Press()
    {
        int index = Random.Range(0, _press.Length);

        _press[index].Play();
    }
}
