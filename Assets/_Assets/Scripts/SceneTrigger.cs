using BeauRoutine;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [field: SerializeField, Tooltip("Where to put the player (Which \"Portal\" to use when loading the new scene)")] public string ID;
    [SerializeField, Tooltip("Which scene/level/map to load (check the \"Build Settings\" window)")] private int newSceneIndex;

    [field: SerializeField] public Transform SpawnPosition { get; private set; }

    public void LoadNextScene()
    {
        Routine.Start(this, SceneTransitioner.Instance.StartLoadScene(newSceneIndex, ID));
    }
}
