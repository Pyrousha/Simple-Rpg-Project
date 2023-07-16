using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroDialogueInteract : MonoBehaviour
{
    public List<IInteractable> Interactables { get; private set; } = new List<IInteractable>();
    private IInteractable highestPrioInteractable = null;

    // Update is called once per frame
    void Update()
    {
        if (!PauseController.Instance.IsPaused && DialogueUI.Instance.isOpen == false)
        {
            if (highestPrioInteractable != null)
            {
                Interactables[0].TryInteract(this);
            }
        }
    }

    public void AddInteractable(IInteractable dialogueActivator)
    {
        if (Interactables.Contains(dialogueActivator))
            return;

        Interactables.Add(dialogueActivator);

        //Sort List
        Interactables = Interactables.OrderByDescending(a => a.Priority).ToList();

        highestPrioInteractable = Interactables[0];
        PopupIndicator.Instance.ShowPopup(highestPrioInteractable.Transform, highestPrioInteractable.InteractPopupText);
    }

    public void RemoveInteractable(IInteractable dialogueActivator)
    {
        int index = Interactables.IndexOf(dialogueActivator);
        if (index >= 0)
        {
            //List contains this interactable
            Interactables.RemoveAt(index);
        }

        if (Interactables.Count > 0)
        {
            highestPrioInteractable = Interactables[0];
            PopupIndicator.Instance.ShowPopup(highestPrioInteractable.Transform, highestPrioInteractable.InteractPopupText);
        }
        else
        {
            highestPrioInteractable = null;
            PopupIndicator.Instance.ClosePopup();
        }
    }
}
