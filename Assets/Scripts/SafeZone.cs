using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private Transform _bodyCenter;
    [SerializeField] private float radius = 0.8f;
    
    // Start is called before the first frame update
    void Start()
    {
        _bodyCenter = gameObject.transform.Find("Body Center");
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
}
