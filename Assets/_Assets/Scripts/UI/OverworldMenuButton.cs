using UnityEngine;
using UnityEngine.UI;

public class OverworldMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject objToEnable;
    [SerializeField] private Selectable firstSelectable;

    public enum MenuButtons_Overworld_Enum
    {
        Items,
        Spells,
        Attributes,
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
