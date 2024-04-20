using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;


public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAA;
    [SerializeField] private GameObject XROrigin;
    private Rigidbody _rigidbody;
    
    private InputActionMap leftHandInteractionMap;
    private InputAction leftTrigger;

    public InputDevice leftController;
    public InputDevice rightController;
    public List<InputDevice> devices;

    private GravityManager gravityManager;
    private SafetyManager _safetyManager;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        // Setup input action asset and get related components
        inputAA.Enable();
        leftHandInteractionMap = inputAA.FindActionMap("XRI LeftHand Interaction");
        leftTrigger = leftHandInteractionMap.FindAction("Activate");
        
        // Initialize input devices
        devices = new List<InputDevice>();
        InitializeInputDevices();

        gravityManager = GameObject.Find("Gravity Manager").GetComponent<GravityManager>();
        
        _safetyManager = XROrigin.GetComponent<SafetyManager>();
        _rigidbody = XROrigin.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initializes the input devices specified in the function
    /// </summary>
    /// <remarks>
    /// Adapted from https://fistfullofshrimp.com/unity-vr-controller-data/
    /// </remarks>
    private void InitializeInputDevices()
    {
        leftController = new InputDevice();
        if (!leftController.isValid)
        {
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref leftController);
        }

        rightController = new InputDevice();
        if (!rightController.isValid)
        {
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref rightController);
        }
    }

    /// <summary>
    /// Gets a reference to an input device as specified by input device characteristics
    /// </summary>
    /// <remarks>
    /// Adapted from https://fistfullofshrimp.com/unity-vr-controller-data/
    /// </remarks>
    /// <param name="characteristics">The characteristics of the device you would like to specify</param>
    /// <param name="inputDevice">Variable to save the input device to</param>
    private void InitializeInputDevice(InputDeviceCharacteristics characteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> deviceList = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, deviceList);

        if (deviceList.Count > 0)
        {
            inputDevice = deviceList[0];
            devices.Add(inputDevice);
        }
    }

    private IEnumerator ApplyGravityVector()
    {
        if (!leftController.isValid)
        {
            throw new Exception("Invalid device");
        }
        
        if (!leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 start))
        {
            throw new Exception("Can't get device position");
        }

        yield return new WaitUntil(() => !leftTrigger.IsPressed());
        
        if (!leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 end))
        {
            throw new Exception("Can't get device position");
        }

        Vector3 direction = (end - start);
        direction = XROrigin.transform.rotation * direction;
        direction.Normalize();
        
        gravityManager.ChangeGravity(direction);
    }

    private bool PrimaryButtonPress()
    {
        bool primaryButtonDown = false;
        
        foreach (InputDevice device in devices)
        {
            device.TryGetFeatureValue(CommonUsages.primaryButton, out  primaryButtonDown);
            if (primaryButtonDown)
                break;
        }

        return primaryButtonDown;
    }

    private void Jump()
    {
        if (_safetyManager.IsGrounded())
        {
            _rigidbody.AddForce(XROrigin.transform.up * 10);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!leftController.isValid)
        {
            InitializeInputDevices();
        }
        
        if (leftTrigger.WasPressedThisFrame() && _safetyManager.IsGrounded())
        {
            StartCoroutine(ApplyGravityVector());
        }

        if (PrimaryButtonPress())
            Jump();
    }
}

