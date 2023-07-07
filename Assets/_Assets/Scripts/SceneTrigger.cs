using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private int newSceneIndex;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 followerDirection;

    public void LoadNextScene()
    {
        SceneTransitioner.Instance.LoadScene(newSceneIndex, spawnPosition, followerDirection);
    }
}
