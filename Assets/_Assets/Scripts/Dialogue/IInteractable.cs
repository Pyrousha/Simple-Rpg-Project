using UnityEngine;

public interface IInteractable
{
    public string InteractPopupText { get; set; }
    public Transform Transform { get; set; }

    public int Priority { get; set; }

    void TryInteract(HeroDialogueInteract player);
}
