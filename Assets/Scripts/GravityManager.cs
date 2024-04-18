using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private GameObject XROrigin;
    private GameObject _cam;
    private Rigidbody _rigidbody;
    private SafetyManager _playerSafetyManager;
    
    enum Axis
    {
        X, Y, Z
    }

    private Axis _currentAxis;
    
    
    /// <summary>
    /// Time it takes to rotate the player once gravity is changed in seconds.
    /// </summary>
    [SerializeField] private float rotationTime = 1f;

    [SerializeField] private float speed = 80f;

    private void Start()
    {
        _rigidbody = XROrigin.GetComponent<Rigidbody>();
        _playerSafetyManager = XROrigin.GetComponent<SafetyManager>();
        _currentAxis = Axis.Y;
        _cam = XROrigin.transform.Find("Camera Offset").transform.Find("Main Camera").gameObject;
    }
    

    public void ChangeGravity(Vector3 direction, bool clamp = true, bool rotatePlayer = true)
    {
        if (clamp)
            direction = Clamp(direction);
        
        Physics.gravity = Gravitize(direction);;
        _rigidbody.velocity = Vector3.zero;

        if (rotatePlayer)
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

    private static Vector3 Gravitize(Vector3 direction)
    {
        return direction * 9.81f;
    }
    
    
    private Vector3 GetNewForward(Vector3 direction)
    {
        Vector3 up = XROrigin.transform.up;
        Vector3 forward = XROrigin.transform.forward;

        Vector3 critical = Vector3.Cross(up, Clamp(XROrigin.transform.right));
        float angle = Vector3.Angle(forward, critical);
        
        Vector3 newForward = Vector3.Cross(up, direction * -1);
        
        if (newForward == Vector3.zero)
        {
            return forward * -1;
        }
        
        newForward = Quaternion.AngleAxis(angle * -1, direction * -1) * newForward;
        return newForward;
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
        Vector3 inverted = direction * -1;

        Quaternion currRotation = XROrigin.transform.rotation;
        Quaternion newRotation = Quaternion.LookRotation(GetNewForward(direction), inverted);
        
        
        for (var elapsedTime = 0f; elapsedTime <= rotationTime; elapsedTime += Time.deltaTime)
        {
            XROrigin.transform.rotation = Quaternion.Slerp(currRotation, newRotation, elapsedTime / rotationTime);
            yield return null;
        }
        
        // Update rigidbody tensor
        _rigidbody.inertiaTensorRotation = XROrigin.transform.rotation;
    }
}
