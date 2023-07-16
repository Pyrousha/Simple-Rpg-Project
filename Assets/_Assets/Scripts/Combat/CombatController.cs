
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CombatMenuButton;
using System.Linq;
using BeauRoutine;

public class CombatController : Singleton<CombatController>
{
    [System.Serializable]
    public struct EntityWithAttack
    {
        public OverworldEntity Caster { get; private set; }
        public OverworldEntity Target { get; private set; }
        public AttackSpell AttackToUse { get; private set; }

        // public bool IsHealing {get; private set;}
        //TODO: Add parameters for healing/mana restoration spells

        public EntityWithAttack(OverworldEntity caster, OverworldEntity target, AttackSpell attackToUse)
        {
            Caster = caster;
            Target = target;
            AttackToUse = attackToUse;
        }
    }

    public enum SubmenuEnum
    {
        None,
        Arts,
        Spells,
        Items
    }

    [System.Serializable]
    public struct SelectableMapping
    {
        public GameObject CurrSelectable { get; private set; }
        public Selectable LastSelectable { get; private set; }

        public SelectableMapping(GameObject currSelectable, Selectable lastSelectable)
        {
            this.CurrSelectable = currSelectable;
            this.LastSelectable = lastSelectable;
        }

        // public void SetLastSelectable(Selectable _lastSelectable)
        // {
        //     Debug.Log($"Updated lastSelectable for object \"{CurrSelectable.name}\" (is now {_lastSelectable.name}, was {LastSelectable.name})");
        //     LastSelectable = _lastSelectable;
        // }
    }

    [SerializeField] private List<CombatEntity> enemyEntitySlots;
    [SerializeField] private List<CombatEntity> playerEntitySlots;
    private List<CombatEntity> activeEnemyEntities;
    private List<CombatEntity> activePlayerEntities;
    [Space(10)]
    [SerializeField] private Animator combatButtonsAnim;
    [SerializeField] private Selectable firstCombatButton;
    [Space(10)]
    [SerializeField] private UIButton artsButton;
    [SerializeField] private UIButton spellsButton;
    [SerializeField] private UIButton itemsButton;
    [SerializeField] private UIButton fleeButton;
    [Space(10)]
    [SerializeField] private Submenu_Combat artsSubmenu;
    [SerializeField] private Submenu_Combat spellsSubmenu;
    [SerializeField] private Submenu_Combat itemsSubmenu;
    [Space(10)]
    [SerializeField] private GameObject bladeSlashPrefab;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private float bladeDuration = 1.0f;
    private List<OverworldEntity> alivePlayers;
    private List<OverworldEntity> deadPlayers;
    private List<OverworldEntity> aliveEnemies;
    private List<OverworldEntity> deadEnemies;

    public bool InCombat { get; set; } = false;

    private bool isPlayerTurn = false;

    private List<EntityWithAttack> entitiesWithAttacksToUse = new List<EntityWithAttack>();

    private int currPlayerIndex;
    private AttackSpell selectedAttackSpell;
    private CombatEntity forwardEntity;

    private List<SelectableMapping> selectablesMap = new List<SelectableMapping>();

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
            overworldEntity.SetCombatEntity(combatEntity);
        }
        for (int i = 0; i < PartyManager.Instance.PartyMembers.Count; i++)
        {
            CombatEntity combatEntity = activePlayerEntities[i];
            OverworldEntity overworldEntity = PartyManager.Instance.PartyMembers[i];

            combatEntity.SetOverworldEntity(overworldEntity);
            overworldEntity.SetCombatEntity(combatEntity);
        }

        UpdateEnemyButtons();
    }

    private void UpdateEnemyButtons()
    {
        //Link buttons on enemies
        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            Navigation customNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,

                //Bind left
                selectOnLeft = aliveEnemies[(i - 1 + aliveEnemies.Count) % aliveEnemies.Count].CombatEntity.EnemyButton,

                //Bind right
                selectOnRight = aliveEnemies[(i + 1 + aliveEnemies.Count) % aliveEnemies.Count].CombatEntity.EnemyButton
            };

            aliveEnemies[i].CombatEntity.EnemyButton.navigation = customNav;
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

        foreach (OverworldEntity player in PartyManager.Instance.PartyMembers)
            player.SetIsDefending(false);

        //Show player menu
        combatButtonsAnim.SetBool("Status", true);
        SelectPlayer(0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_playerIndex"> index of the player in AlivePlayers </param>
    private void SelectPlayer(int _playerIndex)
    {
        firstCombatButton.Select();

        currPlayerIndex = _playerIndex;
        OverworldEntity currPlayer = alivePlayers[_playerIndex];

        fleeButton.interactable = _playerIndex == 0;
        artsButton.interactable = currPlayer.Arts.Count > 0;
        spellsButton.interactable = currPlayer.Spells.Count > 0;
        itemsButton.interactable = false; //TODO: InventoryManager.Instance.Items.Count > 0;


        //TODO: optimize this, probably
        List<Selectable> submenuButtons = spellsSubmenu.SetSpells(currPlayer.Spells, currPlayer.Mp);
        foreach (Selectable spell_submenuButton in submenuButtons)
            SetLastSelectable(spell_submenuButton.gameObject, spellsButton);

        submenuButtons = artsSubmenu.SetSpells(currPlayer.Arts, currPlayer.Mp);
        foreach (Selectable art_submenuButton in submenuButtons)
            SetLastSelectable(art_submenuButton.gameObject, artsButton);


        SetForwardEntity(currPlayer.CombatEntity);
        // submenuButtons = itemsSubmenu.SetSpells(InventoryManager.Instance.Items);
        // foreach (Selectable submenuButton in submenuButtons)
        //     SetLastSelectable(submenuButton, itemsSubmenu.parentSelectable);
    }

    private void SetForwardEntity(CombatEntity _entity)
    {
        forwardEntity?.SetIsForward(false);

        forwardEntity = _entity;

        forwardEntity?.SetIsForward(true);
    }

    public void OnButtonClicked(MenuButtons_Combat_Enum _buttonType, Selectable _selectable)
    {
        if (isPlayerTurn == false)
            return;

        switch (_buttonType)
        {
            case MenuButtons_Combat_Enum.Attack:
                selectedAttackSpell = alivePlayers[currPlayerIndex].BasicAttack;
                if (aliveEnemies.Count > 1)
                {
                    //Multiple enemies, need to select which one to attack
                    for (int i = 0; i < aliveEnemies.Count; i++)
                        SetLastSelectable(aliveEnemies[i].CombatEntity.EnemyButton.gameObject, _selectable);

                    aliveEnemies[0].CombatEntity.EnemyButton.Select();
                }
                else
                {
                    //Only 1 enemy, use attack on them automatically
                    SetPlayerTargetAndMoveToNextPlayer(aliveEnemies[0]);
                }
                break;

            case MenuButtons_Combat_Enum.Defend:
                alivePlayers[currPlayerIndex].SetIsDefending(true);
                TrySelectNextPlayer();
                break;

            case MenuButtons_Combat_Enum.Arts:
                SetCurrSubmenu(SubmenuEnum.Arts);
                break;

            case MenuButtons_Combat_Enum.Spells:
                SetCurrSubmenu(SubmenuEnum.Spells);
                break;

            case MenuButtons_Combat_Enum.Items:
                SetCurrSubmenu(SubmenuEnum.Items);
                break;

            case MenuButtons_Combat_Enum.Flee:
                //TODO: Calculate if flee suceeds or fails (for now assume flee always works)
                EndCombat(CombatWinCondition.Flee);
                break;
        }
    }

    public void OnSpellClicked(Selectable _selectable, AttackSpell _spell)
    {
        selectedAttackSpell = _spell;

        //TODO: Add better target selection for multihit spells
        if (_spell.HitsMultipleEnemies)
        {
            SetPlayerTargetAndMoveToNextPlayer(null);
        }
        else
        {
            if (aliveEnemies.Count > 1)
            {
                //Multiple enemies, need to select which one to attack
                for (int i = 0; i < aliveEnemies.Count; i++)
                    SetLastSelectable(aliveEnemies[i].CombatEntity.EnemyButton.gameObject, _selectable);

                aliveEnemies[0].CombatEntity.EnemyButton.Select();
            }
            else
            {
                //Only 1 enemy, use attack on them automatically
                SetPlayerTargetAndMoveToNextPlayer(aliveEnemies[0]);
            }
        }
    }

    public void SetCurrSubmenu(SubmenuEnum _submenu)
    {
        switch (_submenu)
        {
            case SubmenuEnum.None:
                artsSubmenu.SetStatus(false);
                spellsSubmenu.SetStatus(false);
                itemsSubmenu.SetStatus(false);
                break;

            case SubmenuEnum.Arts:
                spellsSubmenu.SetStatus(false);
                itemsSubmenu.SetStatus(false);

                artsSubmenu.SetStatus(true);
                break;

            case SubmenuEnum.Spells:
                artsSubmenu.SetStatus(false);
                itemsSubmenu.SetStatus(false);

                spellsSubmenu.SetStatus(true);
                break;

            case SubmenuEnum.Items:
                artsSubmenu.SetStatus(false);
                spellsSubmenu.SetStatus(false);

                itemsSubmenu.SetStatus(true);
                break;
        }
    }

    private void SetLastSelectable(GameObject _currSelectable, Selectable _lastSelectable)
    {
        for (int i = 0; i < selectablesMap.Count; i++)
        {
            SelectableMapping currMap = selectablesMap[i];
            if (currMap.CurrSelectable == _currSelectable && currMap.LastSelectable != _lastSelectable)
            {
                //replace currMap with new one
                selectablesMap[i] = new SelectableMapping(_currSelectable, _lastSelectable);
                return;
            }
        }

        //Selectable was not found, add to mapping
        selectablesMap.Add(new SelectableMapping(_currSelectable, _lastSelectable));
    }

    private void Update()
    {
        if (!InCombat)
            return;

        if (InputHandler.Instance.Cancel.Down && isPlayerTurn)
        {
            GameObject currSelectedObj = EventSystem.current.currentSelectedGameObject;
            if (currSelectedObj == null)
            {
                Debug.LogWarning("Tried to cancel when nothing is selected.");
            }

            foreach (SelectableMapping mapping in selectablesMap)
            {
                if (mapping.CurrSelectable == currSelectedObj)
                {
                    //Found mapping!
                    mapping.LastSelectable.Select();
                    return;
                }
            }

            Debug.LogWarning("Unable to find mapping for " + currSelectedObj.name);
        }
    }

    public void SetPlayerTargetAndMoveToNextPlayer(OverworldEntity _target)
    {
        entitiesWithAttacksToUse.Add(new EntityWithAttack(alivePlayers[currPlayerIndex], _target, selectedAttackSpell));
        SetCurrSubmenu(SubmenuEnum.None);

        TrySelectNextPlayer();
    }

    private void TrySelectNextPlayer()
    {
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
        SetForwardEntity(null);

        isPlayerTurn = false;
        combatButtonsAnim.SetBool("Status", false);
        DescriptionBox.Instance.SetStatus(false);

        foreach (OverworldEntity enemy in deadEnemies)
            enemy.SetIsDefending(false);
        foreach (OverworldEntity enemy in aliveEnemies)
        {
            enemy.SetIsDefending(false);

            //TODO: Allow enemies to use spells
            int targetPlayerIndex = Random.Range(0, alivePlayers.Count);

            entitiesWithAttacksToUse.Add(new EntityWithAttack(enemy, alivePlayers[targetPlayerIndex], enemy.BasicAttack));
        }

        Routine.Start(EntitiesAttack());
    }

    private IEnumerator EntitiesAttack()
    {
        //Sort array by attack priority, and then by speed
        entitiesWithAttacksToUse = entitiesWithAttacksToUse.OrderByDescending(a => a.AttackToUse.Priority).ThenByDescending(a => a.Caster.Speed.Value).ToList();

        //Do the attacks!
        for (int i = 0; i < entitiesWithAttacksToUse.Count; i++)
        {
            OverworldEntity caster = entitiesWithAttacksToUse[i].Caster;
            if (caster.IsDead)
                continue;

            AttackSpell attack = entitiesWithAttacksToUse[i].AttackToUse;

            SetForwardEntity(caster.CombatEntity);

            //Use mana
            if (caster.Mp < attack.ManaCost)
                Debug.LogError($"Entity {caster.gameObject.name} does not have enough mana to cast {attack.name} (requires {attack.ManaCost}, has {caster.Mp})");
            caster.UseMana(attack.ManaCost);

            List<OverworldEntity> targets;
            {
                OverworldEntity target = entitiesWithAttacksToUse[i].Target;

                if (target != null) //Only hitting 1 target
                {
                    // Find a new target if the current one is already dead
                    if (target.IsDead)
                    {
                        if (target.IsPlayer)
                            target = alivePlayers[Random.Range(0, alivePlayers.Count)];
                        else
                            target = aliveEnemies[Random.Range(0, aliveEnemies.Count)];
                    }

                    targets = new List<OverworldEntity>() { target };
                }
                else //Hitting multiple tagets
                {
                    //TODO: check if spell is healing/mana restoration
                    //For now: assume all spells deal damage
                    if (caster.IsPlayer)
                        targets = new List<OverworldEntity>(aliveEnemies);
                    else
                        targets = new List<OverworldEntity>(alivePlayers);
                }
            }


            const float timeBetweenMultihits = 0.1f;
            float timeWaited = 0;

            for (int j = 0; j < attack.NumAttacks; j++)
            {
                foreach (OverworldEntity target in targets)
                {
                    if (target.IsDead)
                    {
                        Debug.Log("Tried to spawn attack for " + target.name + " when it is already dead.");
                        continue;
                    }

                    //Create bladeSlash visuals
                    GameObject bladeSlash = Instantiate(bladeSlashPrefab);
                    bladeSlash.GetComponent<MoveToPoint>().MoveToPosition(caster.CombatEntity.CenterOfSprite, target.CombatEntity.CenterOfSprite, bladeDuration);
                }

                timeWaited += timeBetweenMultihits;
                yield return new WaitForSeconds(timeBetweenMultihits);
            }

            yield return new WaitForSeconds(bladeDuration - timeWaited);

            SetForwardEntity(null);

            for (int j = 0; j < attack.NumAttacks; j++)
            {
                foreach (OverworldEntity target in targets)
                {
                    if (target.IsDead)
                    {
                        Debug.Log("Tried to deal damage to " + target.name + " when it is already dead.");
                        continue;
                    }

                    float attackValue;
                    if (attack.IsPhysical)
                        attackValue = caster.Atk_P.Value;
                    else
                        attackValue = caster.Atk_M.Value;

                    //TODO: Add parameters for healing/mana restoration spells
                    int damageTaken = target.TakeDamage(attackValue, attack.IsPhysical, attack.AttackScalingMultiplier);

                    //Show damage numbers
                    Transform damageNumberTransform = Instantiate(damageNumberPrefab).transform;
                    damageNumberTransform.parent = target.CombatEntity.transform;
                    damageNumberTransform.localScale = Vector3.one * 0.005f;
                    damageNumberTransform.localRotation = Quaternion.identity;
                    float angle = Random.Range(-45, 180 + 45) * Mathf.Deg2Rad;
                    damageNumberTransform.localPosition = 0.5f * new Vector3(Mathf.Cos(angle), 0.5f + Mathf.Sin(angle), 0);

                    //TODO: Add parameters for healing/mana restoration spells
                    damageNumberTransform.GetComponent<DamageNumber>().SetVisuals(DamageNumber.DamageNumberEnum.Damage, damageTaken, bladeDuration);

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
                            else
                                UpdateEnemyButtons();
                        }
                    }
                }
                yield return new WaitForSeconds(timeBetweenMultihits);
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
        DescriptionBox.Instance.SetStatus(false);
        CombatTransitionController.Instance.EndCombat();
    }
}
