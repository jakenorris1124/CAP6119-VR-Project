using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private GameObject XROrigin;
    
    /// <summary>
    /// Time it takes to rotate the player once gravity is changed in seconds.
    /// </summary>
    [SerializeField] private float rotationTime = 1f;
    
    public void ChangeGravity(Vector3 direction, bool clamp = true)
    {
        if (clamp)
            direction = Clamp(direction);
        
        Physics.gravity = direction;
        StartCoroutine(RotatePlayer(direction));
    }

    /// <summary>
    /// Clamps the input vector to the nearest cardinal direction
    /// </summary>
    /// <param name="direction">The vector to clamp</param>
    /// <example>If a Vector3(0.4, 0.3, 0.9) was input, then a Vector3(0, 0, 1) would
    /// be returned</example>
    /// <returns>The nearest cardinal direction to the input value.</returns>
    private static Vector3 Clamp(Vector3 direction)
    {
        Vector3 strict = direction.Abs();

        if (strict.x > strict.y && strict.x > strict.z)
            return new Vector3(1 * GetSign(direction.x), 0, 0);
        if (strict.y > strict.x && strict.y > strict.z)
            return new Vector3(0, 1 * GetSign(direction.y), 0);
        
        return new Vector3(0, 0, 1 * GetSign(direction.z));
    }

    /// <summary>
    /// Gets the sign of <paramref name="value"/>
    /// </summary>
    /// <param name="value">The value to check the sign of</param>
    /// <returns>1 if <paramref name="value"/> is positive, -1 if it is negative</returns>
    private static int GetSign(float value)
    {
        return value > 0 ? 1 : -1;
    }

    private IEnumerator RotatePlayer(Vector3 direction)
    {
        Vector3 up = XROrigin.transform.up;
        Vector3 inverted = new Vector3(direction.x * -1, direction.y * -1, direction.z * -1);

        for (var elapsedTime = 0f; elapsedTime <= rotationTime; elapsedTime += Time.deltaTime)
        {
            XROrigin.transform.up = Vector3.Slerp(up, inverted, elapsedTime);
            yield return null;
        }
    }
}
