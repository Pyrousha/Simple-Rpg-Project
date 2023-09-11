using UnityEngine;
using UnityEngine.EventSystems;

public class CombatMenuButton : UIButton, ISelectHandler
{
    public enum MenuButtons_Combat_Enum
    {
        Attack,
        Defend,
        Arts,
        Spells,
        Items,
        Flee
    }

    [SerializeField] private MenuButtons_Combat_Enum buttonType;

    public void TriggerClickOnCombatController()
    {
        CombatController.Instance.OnButtonClicked(buttonType, C_Selectable);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        CombatController.Instance.SetCurrSubmenu(CombatController.SubmenuEnum.None);

        base.OnSelect(eventData);
    }
}
