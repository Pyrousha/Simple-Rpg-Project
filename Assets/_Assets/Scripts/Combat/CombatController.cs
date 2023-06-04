using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CombatMenuButton;

public class CombatController : Singleton<CombatController>
{
    [SerializeField] private Animator combatButtonsAnim;
    [SerializeField] private Selectable firstCombatButton;
    private Selectable lastSelectedButton;


    public void OnCombatStarted()
    {
        //TODO: Calculate if player or enemy should go first (rign now assume player goes first)
        OnPlayerTurnStarted();
    }

    public void OnCombatEnded()
    {
        combatButtonsAnim.SetBool("Status", false);
    }

    private void OnPlayerTurnStarted()
    {
        //Show player menu
        combatButtonsAnim.SetBool("Status", true);
        firstCombatButton.Select();
    }

    public void OnButtonClicked(MenuButtons_Combat_Enum _buttonType, Selectable _selectable)
    {
        lastSelectedButton = _selectable;

        switch (_buttonType)
        {
            case MenuButtons_Combat_Enum.Attack:
                break;
            case MenuButtons_Combat_Enum.Spells:
                break;
            case MenuButtons_Combat_Enum.Items:
                break;
            case MenuButtons_Combat_Enum.Flee:
                //TODO: Calculate if flee suceeds or fails (for now assume flee always works)
                CombatTransitionController.Instance.EndCombat();
                OnCombatEnded();
                break;
        }
    }
}
