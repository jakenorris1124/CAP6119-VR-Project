using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SafetyManager : MonoBehaviour
{
    private Transform _bodyCenter;
    [SerializeField] private float radius = 0.8f;
    
    private LayerMask _levelGeometry;
    
    // Start is called before the first frame update
    void Start()
    {
        _bodyCenter = gameObject.transform.Find("Body Center");
        
        string[] layers = { "Level Geometry", "Light Bridge" };
        _levelGeometry = LayerMask.GetMask(layers);
    }

    /// <summary>
    /// Attempts to move the player out of a position where they could get stuck in the level
    /// geometry (referred to as a dangerous position). If the player is not in a dangerous position, nothing happens.
    /// </summary>
    public void TrySafetyFix()
    {
        if (IsSafe(out List<Vector3> dangerPoints))
            return;

        foreach (Vector3 dangerPoint in dangerPoints)
        {
            Vector3 direction = _bodyCenter.position - dangerPoint;

            gameObject.transform.position += direction;
        }
    }

    /// <summary>
    /// Checks whether or not the player is dangerous position.
    /// </summary>
    /// <param name="dangerPoints">Stores all of the danger points if the player is in a dangerous position.</param>
    /// <returns>True if the player is in a safe position, false if they are in a dangerous position.</returns>
    private bool IsSafe(out List<Vector3> dangerPoints)
    {
        bool safe = true;
        dangerPoints = new List<Vector3>();
        
        LayerMask level = LayerMask.GetMask("Level Geometry");
        Collider[] dangerousObjects = Physics.OverlapSphere(_bodyCenter.position, radius, level);

        foreach (Collider dangerousObject in dangerousObjects)
        {
            safe = false;
            
            dangerPoints.Add(dangerousObject.ClosestPoint(_bodyCenter.position));
        }
        
        return safe;
    }
    
    /// <summary>
    /// Checks if the player is currently on the ground with a little leniency (i.e, being slightly off the ground
    /// still counts as being grounded).
    /// </summary>
    /// <returns>True if the player is on the ground, false if they are not.</returns>
    public bool IsGrounded()
    {
        Transform bodyCenterTransform = _bodyCenter.transform;
        return Physics.Raycast(bodyCenterTransform.position, bodyCenterTransform.up * -1, 1f, _levelGeometry);
    }
}
