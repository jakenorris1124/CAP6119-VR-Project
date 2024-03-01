using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManager : MonoBehaviour
{
    public void ChangeGravity(Vector3 direction)
    {
       Physics.gravity = direction;
    }
}
