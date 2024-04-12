using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SafetyManager : MonoBehaviour
{
    private Transform _bodyCenter;
    [SerializeField] private float radius = 0.8f;
    private LayerMask _solidGround;
    
    private Queue<FauxTransform> _safeSpots;
    private float _accumulatedTime = 0f;
    private bool _hyperLocationSave = false;
    private GravityManager _gravityManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _bodyCenter = gameObject.transform.Find("Body Center");
        
        string[] layers = { "Level Geometry", "Light Bridge" };
        _solidGround = LayerMask.GetMask(layers);
        
        _safeSpots = new Queue<FauxTransform>();

        _gravityManager = GameObject.Find("Gravity Manager").GetComponent<GravityManager>();
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

    private void Update()
    {
        if (_hyperLocationSave) 
            return;
        
        _accumulatedTime += Time.deltaTime;
            
        if (_accumulatedTime >= 1f)
        {
            try
            {
                SaveLocation();
            }
            catch (CantSaveLocationException e)
            {
                StartCoroutine(HyperSaveLocation());
            }
        }
    }

    /// <summary>
    /// Checks if the player is currently on the ground with a little leniency (i.e, being slightly off the ground
    /// still counts as being grounded).
    /// </summary>
    /// <returns>True if the player is on the ground, false if they are not.</returns>
    public bool IsGrounded()
    {
        Transform bodyCenterTransform = _bodyCenter.transform;
        return Physics.Raycast(bodyCenterTransform.position, bodyCenterTransform.up * -1, 1f, _solidGround);
    }

    /// <summary>
    /// Saves the players location. Throws a CantSaveLocationException if the location cannot be saved.
    /// </summary>
    private void SaveLocation()
    {
        if (IsGrounded())
        {
            _safeSpots.Enqueue(new FauxTransform(gameObject.transform.position, gameObject.transform.rotation));
            _accumulatedTime = 0f;

            if (_safeSpots.Count >= 5)
            {
                _safeSpots.Dequeue();
            }
        }
        else
            throw new CantSaveLocationException();
    }

    private IEnumerator HyperSaveLocation()
    {
        _hyperLocationSave = true;

        while (true)
        {
            try
            {
                SaveLocation();
                _hyperLocationSave = false;
                yield break;
            }
            catch (CantSaveLocationException e) { }
            
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Out of Bounds")
            ReturnToSafety();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tainted"))
            ReturnToSafety();
    }

    public void ReturnToSafety()
    {
        CopyTransform(_safeSpots.Peek());
        _gravityManager.ChangeGravity(transform.up * -1, rotatePlayer: false);
    }
    
    // Adapted from https://gamedev.stackexchange.com/questions/204851/copying-transforms-from-one-object-to-another
    private void CopyTransform(FauxTransform source)
    {
        source.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);
        gameObject.transform.SetPositionAndRotation(pos, rot);
    }

    private class FauxTransform
    {
        private Vector3 _position;
        private Quaternion _rotation;
        
        public FauxTransform(Vector3 pos, Quaternion rot)
        {
            _position = pos;
            _rotation = rot;
        }

        public void GetPositionAndRotation(out Vector3 pos, out Quaternion rot)
        {
            pos = _position;
            rot = _rotation;
        }
    }

    private class CantSaveLocationException : Exception
    {
        public CantSaveLocationException()
        {
        }

        public CantSaveLocationException(string message) : base(message)
        {
        }

        public CantSaveLocationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}


