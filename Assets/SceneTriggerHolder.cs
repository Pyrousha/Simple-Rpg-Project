using System.Collections.Generic;
using UnityEngine;

public class SceneTriggerHolder : Singleton<SceneTriggerHolder>
{
    [SerializeField] private List<SceneTrigger> SceneTriggers = new List<SceneTrigger>();

    public SceneTrigger GetTriggerWithID(string _id)
    {
        foreach (SceneTrigger trigger in SceneTriggers)
        {
            if (trigger.ID == _id)
                return trigger;
        }

        return null;
    }
}
