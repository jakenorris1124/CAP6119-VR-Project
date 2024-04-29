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
    private Camera _camera;
    
    private InputActionMap leftHandInteractionMap;
    private InputAction leftTrigger;
    
    private InputActionMap rightHandInteractionMap;
    private InputAction rightTrigger;

    private InputActionMap leftHandLocoMap;
    private InputAction leftStick;

    public InputDevice leftController;
    public InputDevice rightController;
    public List<InputDevice> devices;

    private GravityManager gravityManager;
    private SafetyManager _safetyManager;

    private bool holding;
    private bool _drawing;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        // Setup input action asset and get related components
        inputAA.Enable();
        leftHandInteractionMap = inputAA.FindActionMap("XRI LeftHand Interaction");
        leftTrigger = leftHandInteractionMap.FindAction("Activate");
        leftHandLocoMap = inputAA.FindActionMap("XRI LeftHand Locomotion");
        leftStick = leftHandLocoMap.FindAction("Move");
        
        rightHandInteractionMap = inputAA.FindActionMap("XRI RightHand Interaction");
        rightTrigger = rightHandInteractionMap.FindAction("Activate");
        
        // Initialize input devices
        devices = new List<InputDevice>();
        InitializeInputDevices();

        gravityManager = GameObject.Find("Gravity Manager").GetComponent<GravityManager>();
        
        _safetyManager = XROrigin.GetComponent<SafetyManager>();
        _rigidbody = XROrigin.GetComponent<Rigidbody>();
        _camera = XROrigin.transform.Find("Camera Offset").GetChild(0).GetComponent<Camera>();

        holding = false;
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
        if (_drawing)
        {
            yield break;
        }

        _drawing = true;

        InputDevice controller = leftTrigger.WasPressedThisFrame() ? leftController : rightController;
        InputAction trigger = leftTrigger.WasPressedThisFrame() ? leftTrigger : rightTrigger;
        
        if (!controller.isValid)
        {
            throw new Exception("Invalid device");
        }
        
        if (!controller.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 start))
        {
            throw new Exception("Can't get device position");
        }

        yield return new WaitUntil(() => !trigger.IsPressed());
        
        if (!controller.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 end))
        {
            throw new Exception("Can't get device position");
        }

        Vector3 direction = (end - start);
        direction = XROrigin.transform.rotation * direction;
        direction.Normalize();
        
        gravityManager.ChangeGravity(direction);
        _drawing = false;
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

    private bool BothSecondaryPress()
    {
        leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool leftSecondaryButton);
        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool rightSecondaryButton);

        return leftSecondaryButton && rightSecondaryButton;
    }

    private IEnumerator HoldBothSecondary()
    {
        holding = true;

        float time = 0f;
        while (BothSecondaryPress())
        {
            time += Time.deltaTime;
            if (time >= 2f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            yield return null;
        }

        if (time < 0.5f)
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex + 2 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(buildIndex + 2);
            }
        }

        holding = false;
    }

    private void Jump()
    {
        if (_safetyManager.IsGrounded())
        {
            _rigidbody.AddForce(XROrigin.transform.up * 10);
        }
    }

    private void Move(Vector2 input)
    {
        if (input == Vector2.zero)
            return;

        float angle = Vector2.SignedAngle(Vector2.up, input) * -1;
        
        Vector3 up = Physics.gravity * -1;
        Vector3 camForward = Vector3.ProjectOnPlane(_camera.transform.forward, up);

        Vector3 movement = Quaternion.AngleAxis(angle, up) * camForward * input.magnitude;
        
        
        XROrigin.transform.Translate(movement * (Time.deltaTime * 3), relativeTo: Space.World);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!leftController.isValid || !rightController.isValid)
        {
            InitializeInputDevices();
        }
        
        if ((leftTrigger.WasPressedThisFrame() || rightTrigger.WasPressedThisFrame()) && _safetyManager.IsGrounded())
        {
            StartCoroutine(ApplyGravityVector());
        }
        
        Move(leftStick.ReadValue<Vector2>());

        if (PrimaryButtonPress())
            Jump();

        if (!holding && BothSecondaryPress())
        {
            StartCoroutine(HoldBothSecondary());
        }
    }
}

