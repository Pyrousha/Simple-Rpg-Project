using UnityEngine;
using UnityEngine.EventSystems;

public class OverworldMenuButton : UIButton, ISelectHandler
{
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
        OverworldMenuController.Instance.OnMainButtonClicked(buttonType);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        OverworldMenuController.Instance.OnMainButtonSelected(buttonType, C_Selectable);

        base.OnSelect(eventData);
    }
}
