using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private GameObject XROrigin;
    
    public void ChangeGravity(Vector3 direction)
    {
       Physics.gravity = direction;
       StartCoroutine(RotatePlayer(direction));
    }

    private IEnumerator RotatePlayer(Vector3 direction)
    {
        Vector3 up = XROrigin.transform.up;
        Vector3 inverted = new Vector3(direction.x * -1, direction.y * -1, direction.z * -1);

        for (float rotate = 0.01f; rotate <= 1f; rotate += 0.01f)
        {
            XROrigin.transform.up = Vector3.Slerp(up, inverted, rotate);
            yield return null;
        }
    }
}
