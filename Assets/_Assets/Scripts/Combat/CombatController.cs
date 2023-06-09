
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CombatMenuButton;

public class CombatController : Singleton<CombatController>
{
    public struct EntityWithAttack
    {
        public CombatEntity Caster { get; private set; }
        public CombatEntity Target { get; private set; }
        public AttackSpell AttackToUse { get; private set; }

        public EntityWithAttack(CombatEntity caster, CombatEntity target, AttackSpell attackToUse)
        {
            Caster = caster;
            Target = target;
            AttackToUse = attackToUse;
        }
    }

    [SerializeField] private Animator combatButtonsAnim;
    [SerializeField] private Selectable firstCombatButton;
    private Selectable lastSelectedButton;
    private List<CombatEntity> alivePlayers;
    private List<CombatEntity> deadPlayers;
    private List<CombatEntity> aliveEnemies;
    private List<CombatEntity> deadEnemies;

    private bool isPlayerTurn = false;

    private List<EntityWithAttack> entitiesWithAttacksToUse = new List<EntityWithAttack>();

    private int currPlayerIndex;
    private AttackSpell selectedAttackSpell;

    public void ResetEntityLists()
    {
        alivePlayers = new List<CombatEntity>();
        deadPlayers = new List<CombatEntity>();
        foreach (CombatEntity currPlayerEntity in PartyManager.Instance.PartyMembers)
        {
            if (currPlayerEntity.Dead == false)
                alivePlayers.Add(currPlayerEntity);
            else
                deadPlayers.Add(currPlayerEntity);
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
        entitiesWithAttacksToUse = new List<EntityWithAttack>();

        //Show player menu
        combatButtonsAnim.SetBool("Status", true);
        SelectPlayer(0);
    }

    private void SelectPlayer(int _playerIndex)
    {
        firstCombatButton.Select();

        currPlayerIndex = _playerIndex;
    }

    public void OnButtonClicked(MenuButtons_Combat_Enum _buttonType, Selectable _selectable)
    {
        lastSelectedButton = _selectable;

        switch (_buttonType)
        {
            case MenuButtons_Combat_Enum.Attack:
                selectedAttackSpell = alivePlayers[currPlayerIndex].BasicAttack;
                if (aliveEnemies.Count > 1)
                {
                    //Multiple enemies, need to select which one to attack
                    aliveEnemies[0].EnemyButton.Select();
                }
                else
                {
                    //Only 1 enemy, use attack on them automatically
                    SetPlayerTargetAndMoveToNextPlayer(aliveEnemies[0]);
                }

                break;
            case MenuButtons_Combat_Enum.Spells:
                break;
            case MenuButtons_Combat_Enum.Items:
                break;
            case MenuButtons_Combat_Enum.Flee:
                //TODO: Calculate if flee suceeds or fails (for now assume flee always works)
                EndCombat(CombatWinCondition.Flee);
                break;
        }
    }

    private void Update()
    {
        if (InputHandler.Instance.Cancel.Down && isPlayerTurn)
        {
            if (lastSelectedButton != null)
            {
                //TODO: allow pressing back button to select last party member (cancel last attack/spell)
                lastSelectedButton.Select();
                lastSelectedButton = null;
            }
        }
    }

    public void SetPlayerTargetAndMoveToNextPlayer(CombatEntity _target)
    {
        entitiesWithAttacksToUse.Add(new EntityWithAttack(alivePlayers[currPlayerIndex], _target, selectedAttackSpell));

        if (currPlayerIndex + 1 < alivePlayers.Count)
        {
            //Still have more players to specify attacks for
            SelectPlayer(currPlayerIndex + 1);
        }
        else
        {
            //All players have attacks setup, have enemies calculate what attacks to do
            SpecifyAttacksForEnemies();
        }
    }

    private void SpecifyAttacksForEnemies()
    {
        isPlayerTurn = false;

        foreach (CombatEntity enemy in aliveEnemies)
        {
            //TODO: Allow enemies to use spells
            int targetPlayerIndex = Random.Range(0, alivePlayers.Count);

            entitiesWithAttacksToUse.Add(new EntityWithAttack(enemy, alivePlayers[targetPlayerIndex], enemy.BasicAttack));
        }

        EntitiesAttack();
    }

    private void EntitiesAttack()
    {
        //Sort array by speed (decreasing)
        entitiesWithAttacksToUse.Sort((a, b) => b.Caster.Speed.Value.CompareTo(a.Caster.Speed.Value));

        //Do the attacks!
        for (int i = 0; i < entitiesWithAttacksToUse.Count; i++)
        {
            CombatEntity caster = entitiesWithAttacksToUse[i].Caster;
            CombatEntity target = entitiesWithAttacksToUse[i].Target;
            AttackSpell attack = entitiesWithAttacksToUse[i].AttackToUse;

            int attackValue;
            if (attack.IsPhysical)
                attackValue = caster.Atk_P.Value;
            else
                attackValue = caster.Atk_M.Value;
            attackValue = Mathf.RoundToInt(attackValue * attack.AttackScalingMultiplier);

            if (target.TakeDamage(attackValue, attack.IsPhysical))
            {
                //This target was just killed
                if (target.IsPlayer)
                {
                    //Player was just killed
                    deadPlayers.Add(target);
                    alivePlayers.Remove(target);

                    if (alivePlayers.Count == 0)
                    {
                        EndCombat(CombatWinCondition.Lose);
                        return;
                    }
                }
                else
                {
                    //Enemy was just killed
                    deadEnemies.Add(target);
                    aliveEnemies.Remove(target);

                    if (aliveEnemies.Count == 0)
                    {
                        EndCombat(CombatWinCondition.Win);
                        return;
                    }
                }
            }
        }

        OnPlayerTurnStarted();
    }

    public enum CombatWinCondition
    {
        Win,
        Flee,
        Lose
    }

    private void EndCombat(CombatWinCondition _winCondition)
    {
        switch (_winCondition)
        {
            case CombatWinCondition.Win:
                //Add gold and XP
                foreach (CombatEntity enemy in deadEnemies)
                {
                    InventoryManager.Instance.AddGold(enemy.BaseStats.Drops.GP);
                    Debug.Log("Gained " + enemy.BaseStats.Drops.GP + " GP from enemy " + enemy.name);

                    foreach (CombatEntity livingPlayer in alivePlayers)
                    {
                        Debug.Log(livingPlayer.name + " gained " + enemy.BaseStats.Drops.XP + " XP from enemy " + enemy.name);
                        livingPlayer.GainXP(enemy.BaseStats.Drops.XP);
                    }
                }

                //TODO: add items
                //TODO: show win UI

                break;
            case CombatWinCondition.Flee:
                break;
            case CombatWinCondition.Lose:
                //TODO: Show lose UI
                break;
        }

        combatButtonsAnim.SetBool("Status", false);
        CombatTransitionController.Instance.EndCombat();
    }
}
