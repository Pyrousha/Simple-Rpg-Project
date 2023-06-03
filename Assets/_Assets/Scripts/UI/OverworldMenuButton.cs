using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMenuButton : MonoBehaviour
{
    public enum MenuButtons_Overworld_Enum
    {
        Items,
        Spells,
        CloseMenu,
        Equip,
        Settings,
        Save
    }

    [SerializeField] private MenuButtons_Overworld_Enum buttonType;

    public void TriggerClickOnMenuController()
    {
        OverworldMenuController.Instance.OnButtonClicked(buttonType);
    }
}
