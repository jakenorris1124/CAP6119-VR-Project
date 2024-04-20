using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTimer : MonoBehaviour
{
    [SerializeField] private float sceneAliveTime = 10f;

    private LocalSceneManager _localSceneManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _localSceneManager = GameObject.Find("Scene Manager").GetComponent<LocalSceneManager>();

        StartCoroutine(WaitThenEndScene());
    }

    private IEnumerator WaitThenEndScene()
    {
        for (float elapsedTime = 0; elapsedTime < sceneAliveTime; elapsedTime += Time.deltaTime)
        {
            yield return null;
        }
        
        _localSceneManager.EndScene();
    }
}
