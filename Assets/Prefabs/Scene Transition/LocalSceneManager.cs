using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalSceneManager : MonoBehaviour
{
    [SerializeField] private float sceneFadeTime = 2f;
    [SerializeField] private GameObject black;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private bool isLastScene = false;

    private Material _mat;
    
    // Start is called before the first frame update
    void Start()
    {
        _mat = black.GetComponent<MeshRenderer>().material;

        StartCoroutine(Fade(true));
    }

    public void EndScene()
    {
        black.SetActive(true);
        left.SetActive(false);
        right.SetActive(false);
        StartCoroutine(Fade(false));
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float from = fadeIn ? 1f : 0f;
        float to = fadeIn ? 0f : 1f;
        
        for (float elapsedTime = 0; elapsedTime < sceneFadeTime; elapsedTime += Time.deltaTime)
        {
            float temp = Mathf.Lerp(from, to, elapsedTime / sceneFadeTime);
            _mat.SetFloat("_Alpha", temp);
            yield return null;
        }

        if (!fadeIn)
            ChangeToNextScene();
        else
            black.SetActive(false);
    }

    private void ChangeToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
