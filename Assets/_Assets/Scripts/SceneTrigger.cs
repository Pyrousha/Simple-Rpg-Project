using BeauRoutine;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [field: SerializeField] public string ID;
    [SerializeField] private int newSceneIndex;

    [field: SerializeField] public Transform SpawnPosition { get; private set; }

    public void LoadNextScene()
    {
        Routine.Start(this, SceneTransitioner.Instance.StartLoadScene(newSceneIndex, ID));
    }
}
