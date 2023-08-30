using UnityEngine;

public class SceneTriggerHolder : Singleton<SceneTriggerHolder>
{
    [SerializeField] private SceneTrigger[] sceneTriggers;

    public SceneTrigger GetTriggerWithID(string _id)
    {
        foreach (SceneTrigger trigger in sceneTriggers)
        {
            if (trigger.ID == _id)
                return trigger;
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Transform parentObj = transform.parent;
        while (parentObj.GetComponent<OverworldParent>() == null)
        {
            parentObj = parentObj.parent;
        }

        sceneTriggers = parentObj.GetComponentsInChildren<SceneTrigger>();
    }
#endif
}
