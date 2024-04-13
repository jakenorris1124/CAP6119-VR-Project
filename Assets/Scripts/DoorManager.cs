using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private GameObject left;
    private Transform _leftOpen;
    private Transform _leftClosed;
    
    [SerializeField] private GameObject right;
    private Transform _rightOpen;
    private Transform _rightClosed;

    [SerializeField] private float time = 1f;
    [SerializeField] private bool startOpen = false;
    
    private bool _state;
    private bool _transitioning = false;

    void Start()
    {
        _leftOpen = gameObject.transform.Find("Left_Open");
        _leftClosed = gameObject.transform.Find("Left_Closed");
        
        _rightOpen = gameObject.transform.Find("Right_Open");
        _rightClosed = gameObject.transform.Find("Right_Closed");

        if (startOpen)
        {
            left.transform.position = _leftOpen.position;
            right.transform.position = _rightOpen.position;
        }
    }

    public void Toggle()
    {
        _state = !_state;

        bool doesThisRequestInterruptAnother = TryInterrupt();

        if (_state)
        {
            Open(doesThisRequestInterruptAnother);
        }
        else
        {
            Close(doesThisRequestInterruptAnother);
        }
    }

    public void Open(bool useCurrTransform)
    {
        Transform _leftStart =  useCurrTransform ? left.transform : _leftClosed;
        Transform _rightStart = useCurrTransform ? right.transform : _rightClosed;
        
        StartCoroutine(SlideDoor(left, _leftStart, _leftOpen));
        StartCoroutine(SlideDoor(right, _rightStart, _rightOpen));
    }
    
    public void Close(bool useCurrTransform)
    {
        Transform _leftStart = useCurrTransform ? left.transform : _leftOpen;
        Transform _rightStart = useCurrTransform ? right.transform : _rightOpen;
        
        StartCoroutine(SlideDoor(left, _leftStart, _leftClosed));
        StartCoroutine(SlideDoor(right, _rightStart, _rightClosed));
    }

    private bool TryInterrupt()
    {
        if (_transitioning)
        {
            StopAllCoroutines();
            _transitioning = false;
            return true;
        }

        return false;
    }

    private IEnumerator SlideDoor(GameObject door, Transform start, Transform end)
    {
        _transitioning = true;
        
        for (var elapsedTime = 0f; elapsedTime <= time; elapsedTime += Time.deltaTime)
        {
            door.transform.position = Vector3.Lerp(start.position, end.position, elapsedTime/time);
            yield return null;
        }

        _transitioning = false;
    }
}
