using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private int sceneToLoadIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(sceneToLoadIndex);
    }
}
