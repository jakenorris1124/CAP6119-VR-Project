using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitoryDance : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private bool lastScene;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Dance());
    }

    private IEnumerator Dance()
    {
        camera.farClipPlane = 1f;
        yield return new WaitForSeconds(0.5f);
        
        for (float i = 0f; i < 5f; i += 0.5f)
        {
            camera.farClipPlane += 10f;
            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(5f);
        
        for (float i = 0f; i < 5f; i += 0.5f)
        {
            camera.farClipPlane -= 10f;
            yield return new WaitForSeconds(0.2f);
        }

        if (!lastScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
