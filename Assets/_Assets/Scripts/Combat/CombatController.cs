
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CombatMenuButton;

public class CombatController : Singleton<CombatController>
{
    [System.Serializable]
    public struct EntityWithAttack
    {
        public OverworldEntity Caster { get; private set; }
        public OverworldEntity Target { get; private set; }
        public AttackSpell AttackToUse { get; private set; }

        //TODO: Add parameters for healing/mana restoration spells

        public EntityWithAttack(OverworldEntity caster, OverworldEntity target, AttackSpell attackToUse)
        {
            Caster = caster;
            Target = target;
            AttackToUse = attackToUse;
        }
    }

    [SerializeField] private List<CombatEntity> enemyEntitySlots;
    [SerializeField] private List<CombatEntity> playerEntitySlots;
    private List<CombatEntity> activeEnemyEntities;
    private List<CombatEntity> activePlayerEntities;
    [Space(10)]
    [SerializeField] private Animator combatButtonsAnim;
    [SerializeField] private Selectable firstCombatButton;
    [Space(10)]
    [SerializeField] private GameObject bladeSlashPrefab;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private float bladeDuration = 1.0f;
    private Selectable lastSelectedButton;
    private List<OverworldEntity> alivePlayers;
    private List<OverworldEntity> deadPlayers;
    private List<OverworldEntity> aliveEnemies;
    private List<OverworldEntity> deadEnemies;

    private bool isPlayerTurn = false;

    private List<EntityWithAttack> entitiesWithAttacksToUse = new List<EntityWithAttack>();

    private int currPlayerIndex;
    private AttackSpell selectedAttackSpell;

    public void SetEntitiesForCombat(List<OverworldEntity> _enemiesToFight)
    {
        foreach (CombatEntity entity in enemyEntitySlots)
            entity.SetOverworldEntity(null);
        foreach (CombatEntity entity in playerEntitySlots)
            entity.SetOverworldEntity(null);

        activeEnemyEntities = new List<CombatEntity>(enemyEntitySlots);
        activePlayerEntities = new List<CombatEntity>(playerEntitySlots);

        alivePlayers = new List<OverworldEntity>();
        deadPlayers = new List<OverworldEntity>();
        foreach (OverworldEntity currPlayerEntity in PartyManager.Instance.PartyMembers)
        {
            if (currPlayerEntity.IsDead == false)
                alivePlayers.Add(currPlayerEntity);
            else
                deadPlayers.Add(currPlayerEntity);
        }

        aliveEnemies = new List<OverworldEntity>(_enemiesToFight);
        deadEnemies = new List<OverworldEntity>();

        //The goal here is to have all entities that will be fighting on the player or enemy side to be centered, while also being ordered left to right
        bool deleteRight = true;
        while (activeEnemyEntities.Count > aliveEnemies.Count)
        {
            if (deleteRight)
                activeEnemyEntities.RemoveAt(activeEnemyEntities.Count - 1);
            else
                activeEnemyEntities.RemoveAt(0);
            deleteRight = !deleteRight;
        }
        deleteRight = true;
        while (activePlayerEntities.Count > PartyManager.Instance.PartyMembers.Count)
        {
            if (deleteRight)
                activePlayerEntities.RemoveAt(activePlayerEntities.Count - 1);
            else
                activePlayerEntities.RemoveAt(0);
            deleteRight = !deleteRight;
        }

        //TODO: Add catch for not enough combatEntities

        //Assign each OverworldEntity to a combatEntity (and vice versa)
        for (int i = 0; i < activeEnemyEntities.Count; i++)
        {
            CombatEntity combatEntity = activeEnemyEntities[i];
            OverworldEntity overworldEntity = aliveEnemies[i];

            combatEntity.SetOverworldEntity(overworldEntity);
            overworldEntity.CombatEntity = combatEntity;
        }
        for (int i = 0; i < PartyManager.Instance.PartyMembers.Count; i++)
        {
            CombatEntity combatEntity = activePlayerEntities[i];
            OverworldEntity overworldEntity = PartyManager.Instance.PartyMembers[i];

            combatEntity.SetOverworldEntity(overworldEntity);
            overworldEntity.CombatEntity = combatEntity;
        }

        //Link buttons on enemies
        for (int i = 0; i < activeEnemyEntities.Count; i++)
        {
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;

            //Bind left
            customNav.selectOnLeft = activeEnemyEntities[(i - 1 + activeEnemyEntities.Count) % activeEnemyEntities.Count].EnemyButton;

            //Bind right
            customNav.selectOnRight = activeEnemyEntities[(i + 1 + activeEnemyEntities.Count) % activeEnemyEntities.Count].EnemyButton;

            activeEnemyEntities[i].EnemyButton.navigation = customNav;
        }
    }

    public void OnCombatStarted()
    {
        //TODO: Calculate if player or enemy should go first (right now assume player goes first)
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
                    aliveEnemies[0].CombatEntity.EnemyButton.Select();
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

    public void SetPlayerTargetAndMoveToNextPlayer(OverworldEntity _target)
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

        foreach (OverworldEntity enemy in aliveEnemies)
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
            OverworldEntity caster = entitiesWithAttacksToUse[i].Caster;
            OverworldEntity target = entitiesWithAttacksToUse[i].Target;
            AttackSpell attack = entitiesWithAttacksToUse[i].AttackToUse;

            if (caster.IsDead)
                continue;

            //TODO: Check if target is dead, If so: 
            //  -If attack used was single-target, find another target
            //  -If attack was multi-target, just ignore
            if (target.IsDead)
            {
                if (target.IsPlayer)
                    target = alivePlayers[Random.Range(0, alivePlayers.Count)];
                else
                    target = aliveEnemies[Random.Range(0, aliveEnemies.Count)];
            }

            GameObject bladeSlash = Instantiate(bladeSlashPrefab);
            bladeSlash.GetComponent<MoveToPoint>().MoveToPosition(caster.CombatEntity.CenterOfSprite, target.CombatEntity.CenterOfSprite, bladeDuration);

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
            damageNumberTransform.parent = target.CombatEntity.transform;
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
                target.CombatEntity.GetComponent<AlignWithCamera>().StartFallOver();

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
                foreach (OverworldEntity enemy in deadEnemies)
                {
                    InventoryManager.Instance.AddGold(enemy.BaseStats.Drops.GP);
                    Debug.Log("Gained " + enemy.BaseStats.Drops.GP + " GP from enemy " + enemy.name);

                    foreach (OverworldEntity livingPlayer in alivePlayers)
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

        //TODO: Put this in a coroutine or something
        foreach (CombatEntity combatEntity in activeEnemyEntities)
            combatEntity.OnCombatFinished();
        foreach (CombatEntity combatEntity in activePlayerEntities)
            combatEntity.OnCombatFinished();

        combatButtonsAnim.SetBool("Status", false);
        CombatTransitionController.Instance.EndCombat();
    }
}
