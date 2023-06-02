using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMenuButton : MonoBehaviour
{
    [SerializeField] private OverworldMenuController.MenuButtons_OverworldEnum buttonType;

    public void TriggerClickOnMenuController()
    {
        OverworldMenuController.Instance.OnButtonClicked(buttonType);
    }
}
