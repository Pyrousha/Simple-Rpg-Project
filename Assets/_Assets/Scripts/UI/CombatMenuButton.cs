using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatMenuButton : MonoBehaviour, ISelectHandler
{
    private Selectable selectable;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

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
        CombatController.Instance.OnButtonClicked(buttonType, selectable);
    }

    public void OnSelect(BaseEventData eventData)
    {
        CombatController.Instance.SetCurrSubmenu(CombatController.SubmenuEnum.None);
    }
}
