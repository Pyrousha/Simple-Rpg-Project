using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : Singleton<PauseController>
{
    public bool IsPaused { get; private set; } = false;

    public event System.Action<bool> OnPausedStateChanged_Event;

    private List<GameObject> pausers = new List<GameObject>();

    public void AddPauser(GameObject _newPauser)
    {
        if (pausers.Contains(_newPauser))
        {
            Debug.LogError("List of pausers already contains " + _newPauser.name);
            return;
        }

        pausers.Add(_newPauser);

        TrySetIsPaused(true);
    }

    public void RemovePauser(GameObject _newPauser)
    {
        if (!pausers.Contains(_newPauser))
        {
            Debug.LogError("List of pausers does not contain " + _newPauser.name);
            return;
        }

        pausers.Remove(_newPauser);

        if (pausers.Count == 0)
            TrySetIsPaused(false);
    }

    public void TrySetIsPaused(bool _newIsPaused)
    {
        if (IsPaused != _newIsPaused)
        {
            //State is actually changing               
            IsPaused = _newIsPaused;
            OnPausedStateChanged_Event?.Invoke(IsPaused);
        }
    }

    /// <summary>
    /// Called when combat is started, forces the game to unpause
    /// </summary>
    public void Fallback_CombatStarted()
    {
        pausers = new List<GameObject>();
        TrySetIsPaused(false);
    }
}
