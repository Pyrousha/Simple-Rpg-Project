using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private int newSceneIndex;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 followerDirection;

    public void LoadNextScene()
    {
        Routine.Start(this, SceneTransitioner.Instance.StartLoadScene(newSceneIndex, spawnPosition, followerDirection));
    }
}
