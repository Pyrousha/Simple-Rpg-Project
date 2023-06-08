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
    private List<CombatEntity> alivePlayers;
    private List<CombatEntity> aliveEnemies;
    private List<CombatEntity> deadEnemies;

    private bool isPlayerTurn = false;

    public void ResetEntityLists()
    {
        alivePlayers = new List<CombatEntity>();
        foreach (CombatEntity currPlayerEntity in PartyManager.Instance.PartyMembers)
        {
            if (currPlayerEntity.Dead == false)
                alivePlayers.Add(currPlayerEntity);
        }

        aliveEnemies = new List<CombatEntity>();
        deadEnemies = new List<CombatEntity>();
    }

    public void AddEnemy(CombatEntity _enemy)
    {
        aliveEnemies.Add(_enemy);
    }

    public void OnCombatStarted()
    {
        //TODO: Calculate if player or enemy should go first (rign now assume player goes first)
        OnPlayerTurnStarted();
    }

    private void OnPlayerTurnStarted()
    {
        isPlayerTurn = true;

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
                aliveEnemies[0].EnemyButton.Select();
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

    private void Update()
    {
        if (InputHandler.Instance.Cancel.Down && isPlayerTurn)
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.Select();
                lastSelectedButton = null;
            }
        }
    }

    public void OnCombatEnded()
    {
        combatButtonsAnim.SetBool("Status", false);
    }

    public void UseMoveOnEnemy(CombatEntity _enemyEntity)
    {
        //TODO: add other moves than just normal attack

        Debug.Log("Attacking Enemy " + _enemyEntity.gameObject.name);
    }
}
