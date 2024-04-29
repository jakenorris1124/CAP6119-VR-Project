using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class EnsureStartingOrientation : MonoBehaviour
{
    private Camera _camera;
    private XROrigin _xrOrigin;

    // Start is called before the first frame update
    void Awake()
    {
        _camera = gameObject.transform.Find("Camera Offset").GetChild(0).GetComponent<Camera>();
        _xrOrigin = gameObject.GetComponent<XROrigin>();

        StartCoroutine(FixRotation());
    }

    private IEnumerator FixRotation()
    {
        Vector3 forward = transform.forward;
        Vector3 camForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up);

        while (forward == camForward)
        {
            camForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up);
            yield return null;
        }
        
        float angle = Vector3.SignedAngle(forward, camForward, Vector3.up) * -1;
        
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}
