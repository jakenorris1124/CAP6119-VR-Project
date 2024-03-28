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
    
    private Transform _bodyCenter;
    private LayerMask _levelGeometry;
    
    private InputActionMap leftHandInteractionMap;
    private InputAction leftTrigger;

    public InputDevice leftController;
    public List<InputDevice> devices;

    private GravityManager gravityManager;
    
    public bool IsPressed = false; // used to display button state in the Unity Inspector window
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Setup input action asset and get related components
        inputAA.Enable();
        leftHandInteractionMap = inputAA.FindActionMap("XRI LeftHand Interaction");
        leftTrigger = leftHandInteractionMap.FindAction("Activate");
        
        // Initialize input devices
        InitializeInputDevices();

        gravityManager = GameObject.Find("Gravity Manager").GetComponent<GravityManager>();
        devices = new List<InputDevice>();
        
        _bodyCenter = XROrigin.gameObject.transform.Find("Body Center");
        _levelGeometry = LayerMask.GetMask("Level Geometry");
    }

    /// <summary>
    /// Initializes the input devices specified in the function
    /// </summary>
    /// <remarks>
    /// Adapted from https://fistfullofshrimp.com/unity-vr-controller-data/
    /// </remarks>
    private void InitializeInputDevices()
    {
        if (!leftController.isValid)
        {
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref leftController);
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

    /// <summary>
    /// Checks if the player is currently on the ground with a little leniency (i.e, being slightly off the ground
    /// still counts as being grounded).
    /// </summary>
    /// <returns>True if the player is on the ground, false if they are not.</returns>
    private bool IsGrounded()
    {
        Transform bodyCenterTransform = _bodyCenter.transform;
        return Physics.Raycast(bodyCenterTransform.position, bodyCenterTransform.up * -1, 1f, _levelGeometry);
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftController.isValid)
        {
            InitializeInputDevices();
        }
        
        if (leftTrigger.WasPressedThisFrame() && IsGrounded())
        {
            StartCoroutine(ApplyGravityVector());
        }
    }
}

