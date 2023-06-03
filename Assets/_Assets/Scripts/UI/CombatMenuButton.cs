using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatMenuButton : MonoBehaviour
{
    private Selectable selectable;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    public enum MenuButtons_Combat_Enum
    {
        Attack,
        Spells,
        Items,
        Flee
    }

    [SerializeField] private MenuButtons_Combat_Enum buttonType;

    public void TriggerClickOnCombatController()
    {
        CombatController.Instance.OnButtonClicked(buttonType, selectable);
    }
}
