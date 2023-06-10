
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

        //TODO: Add parameters for healing/mana restoration spells

        public EntityWithAttack(CombatEntity caster, CombatEntity target, AttackSpell attackToUse)
        {
            Caster = caster;
            Target = target;
            AttackToUse = attackToUse;
        }
    }

    [SerializeField] private Animator combatButtonsAnim;
    [SerializeField] private Selectable firstCombatButton;
    [Space(10)]
    [SerializeField] private GameObject bladeSlashPrefab;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private float bladeDuration = 1.0f;
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
            if (currPlayerEntity.IsDead == false)
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
        if (isPlayerTurn == false)
            return;

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
        combatButtonsAnim.SetBool("Status", false);

        foreach (CombatEntity enemy in aliveEnemies)
        {
            //TODO: Allow enemies to use spells
            int targetPlayerIndex = Random.Range(0, alivePlayers.Count);

            entitiesWithAttacksToUse.Add(new EntityWithAttack(enemy, alivePlayers[targetPlayerIndex], enemy.BasicAttack));
        }

        StartCoroutine(EntitiesAttack());
    }

    private IEnumerator EntitiesAttack()
    {
        //Sort array by speed (decreasing)
        entitiesWithAttacksToUse.Sort((a, b) => b.Caster.Speed.Value.CompareTo(a.Caster.Speed.Value));

        //Do the attacks!
        for (int i = 0; i < entitiesWithAttacksToUse.Count; i++)
        {
            CombatEntity caster = entitiesWithAttacksToUse[i].Caster;
            CombatEntity target = entitiesWithAttacksToUse[i].Target;
            AttackSpell attack = entitiesWithAttacksToUse[i].AttackToUse;

            if (caster.IsDead)
                continue;

            GameObject bladeSlash = Instantiate(bladeSlashPrefab);
            //TODO: Make this not hirearchy-based
            bladeSlash.GetComponent<MoveToPoint>().MoveToPosition(caster.transform.GetChild(0).position, target.transform.GetChild(0).position, bladeDuration);

            yield return new WaitForSeconds(bladeDuration);

            float attackValue;
            if (attack.IsPhysical)
                attackValue = caster.Atk_P.Value;
            else
                attackValue = caster.Atk_M.Value;
            attackValue *= attack.AttackScalingMultiplier;

            //TODO: Add parameters for healing/mana restoration spells
            int damageTaken = target.TakeDamage(attackValue, attack.IsPhysical);

            //Show damage numbers
            Transform damageNumberTransform = Instantiate(damageNumberPrefab).transform;
            damageNumberTransform.parent = target.transform;
            damageNumberTransform.localScale = Vector3.one * 0.005f;
            damageNumberTransform.localRotation = Quaternion.identity;
            float angle = Random.Range(-45, 180 + 45) * Mathf.Deg2Rad;
            damageNumberTransform.localPosition = 0.5f * new Vector3(Mathf.Cos(angle), 0.5f + Mathf.Sin(angle), 0);

            //TODO: Add parameters for healing/mana restoration spells
            damageNumberTransform.GetComponent<DamageNumber>().SetVisuals(DamageNumber.DamageNumberEnum.Damage, damageTaken, bladeDuration);

            yield return new WaitForSeconds(0.25f);

            //Check if target just died
            if (target.IsDead)
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
                        yield break;
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
                        yield break;
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
