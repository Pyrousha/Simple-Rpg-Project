using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using UnityEngine;

public class CombatTransitionController : Singleton<CombatTransitionController>
{
    [SerializeField] private GameObject combatParent;
    [SerializeField] private GameObject persistentOverworldParent;
    [Space(10)]
    [SerializeField] private Transform combatCamera_parent;
    [SerializeField] private Transform combatCamera;
    public Transform CombatCamera => combatCamera;
    [Space(10)]
    private Transform enemy_overworld;
    [SerializeField] private Transform player_combat;
    [SerializeField] private Transform enemy_combat;
    [Space(10)]
    [SerializeField] private Animator combatCameraAnim;
    [SerializeField] private Animator combatFloorAnim;
    [Space(5)]
    [SerializeField] private float lerpDuration = 1.0f;
    [SerializeField] private float cameraSpinDuration = 1.0f;

    private List<CombatEntity> entitiesInCombat;
    float cameraStartZAngle;

    public void TriggerCombat(Transform _overworldEnemy, List<OverworldEntity> _enemiesToFight)
    {
        PauseController.Instance.Fallback_CombatStarted();

        CombatController.Instance.InCombat = true;
        CombatController.Instance.SetEntitiesForCombat(_enemiesToFight);

        enemy_overworld = _overworldEnemy;

        entitiesInCombat = new List<CombatEntity>();
        foreach (OverworldEntity entity in PartyManager.Instance.PartyMembers)
            entitiesInCombat.Add(entity.CombatEntity);
        foreach (OverworldEntity entity in _enemiesToFight)
            entitiesInCombat.Add(entity.CombatEntity);

        Routine.Start(this, ToCombat());
    }

    private IEnumerator ToCombat()
    {
        Vector3 heroToEnemy = enemy_overworld.position - PartyManager.Instance.GetFirstAlivePlayer().transform.position;
        float heroToEnemyAngle = Mathf.Atan2(heroToEnemy.y, heroToEnemy.x) * Mathf.Rad2Deg;
        cameraStartZAngle = -heroToEnemyAngle + 90.0f;
        if (cameraStartZAngle > 180)
            cameraStartZAngle -= 360;
        combatCamera_parent.eulerAngles = new Vector3(90, 0, cameraStartZAngle);

        yield return null;
        // while (InputHandler.Instance.Interact.Down == false) { yield return null; }

        foreach (CombatEntity entity in entitiesInCombat)
        {
            PlaceCombatSpriteRelativeToOverworldSprite(entity);
        }
        // PlaceCombatSpriteRelativeToOverworldSprite(player_overworld, player_combat);
        // PlaceCombatSpriteRelativeToOverworldSprite(enemy_overworld, enemy_combat);

        yield return null;

        // player_combat_startingPos = player_combat.position;
        // enemy_combat_startingPos = enemy_combat.position;

        OverworldParent.Instance.gameObject.SetActive(false);
        persistentOverworldParent.SetActive(false);
        combatParent.SetActive(true);

        combatCameraAnim.SetBool("Status", true);
        combatFloorAnim.SetBool("Status", true);

        yield return null;

        // combatCameraAnim.enabled = true;
        // combatFloorAnim.enabled = true;
        Routine.Start(this, SpinCameraParent(cameraStartZAngle, 0));


        foreach (CombatEntity entity in entitiesInCombat)
        {
            if (entity.OverworldEntity.IsPlayer)
                Routine.Start(this, LerpCombatEntity(entity, entity.InCombatPosition, 1));
            else
                Routine.Start(this, LerpCombatEntity(entity, entity.InCombatPosition, 3));
        }

        yield return new WaitForSeconds(lerpDuration);

        CombatController.Instance.OnCombatStarted();
    }

    private void PlaceCombatSpriteRelativeToOverworldSprite(CombatEntity _entity)
    {
        Transform overworldObj = _entity.OverworldEntity.transform;
        Transform combatObj = _entity.transform;

        float horizontalOffset = overworldObj.position.x - OverworldParent.Instance.OverworldCamera.position.x;
        float verticalOffset = overworldObj.position.y - OverworldParent.Instance.OverworldCamera.position.y - 0.5f;

        float yPos = combatObj.position.y;
        Vector3 newCombatPos = combatCamera.position + combatCamera.right * horizontalOffset + combatCamera.up * verticalOffset;
        newCombatPos.y = yPos;

        combatObj.position = newCombatPos;
        _entity.SetStartTransitionPosition(newCombatPos);
        if (_entity.OverworldEntity.IsPlayer)
            _entity.UpdateFacingSprite(new Vector3(0, 0, 1));
        else
            _entity.UpdateFacingSprite(new Vector3(0, 0, -1));
        //_entity.SetFacingDir((combatCenter.position - _entity.transform.position).normalized);
    }

    private IEnumerator SpinCameraParent(float _startZAngle, float _targetZAngle)
    {
        yield return Tween.Float(0, 1, (elapsedPercentage) =>
        {
            combatCamera_parent.eulerAngles = new Vector3(90, 0, Mathf.Lerp(_startZAngle, _targetZAngle, elapsedPercentage));
        }, cameraSpinDuration).Ease(Curve.QuadOut);
    }

    private IEnumerator LerpCombatEntity(CombatEntity _entity, Vector3 _targetPos, float _targetLocalScale, bool _isCombatEnding = false)
    {
        _entity.SetSpriteAlpha(1);

        Vector3 startPos = _entity.transform.position;
        Vector3 startScale = _entity.transform.localScale;
        Vector3 targetScale = Vector3.one * _targetLocalScale;

        yield return Tween.Float(0, 1, (elapsedPercentage) =>
        {
            if (_entity.OverworldEntity.IsPlayer)
            {
                _entity.transform.position = Vector3.Lerp(startPos, _targetPos, elapsedPercentage);
                _entity.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedPercentage);
                _entity.UpdateFacingSprite(new Vector3(0, 0, 1));
            }
            else
            {
                if (_isCombatEnding == false)
                    _entity.transform.position = Vector3.Lerp(startPos, _targetPos, elapsedPercentage);
                else
                    _entity.SetSpriteAlpha(1 - elapsedPercentage * 1.5f);

                _entity.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedPercentage);
                _entity.UpdateFacingSprite(new Vector3(0, 0, -1));
            }

        }, lerpDuration);
    }

    public void EndCombat()
    {
        Routine.Start(this, EndCombatCoroutine());
    }

    private IEnumerator EndCombatCoroutine()
    {
        foreach (CombatEntity entity in entitiesInCombat)
        {
            Routine.Start(this, LerpCombatEntity(entity, entity.StartTransitionPosition, 1, true));
        }

        // StartCoroutine(LerpCombatEntity(player_combat, player_combat_startingPos, 1));
        // StartCoroutine(LerpCombatEntity(enemy_combat, enemy_combat_startingPos, 1));

        Routine.Start(this, SpinCameraParent(0, cameraStartZAngle));

        combatCameraAnim.SetBool("Status", false);
        combatFloorAnim.SetBool("Status", false);

        yield return new WaitForSeconds(lerpDuration);

        EnemySpawner.Instance.DestroyAllEnemies();

        enemy_overworld.gameObject.SetActive(false);

        combatParent.SetActive(false);
        OverworldParent.Instance.gameObject.SetActive(true);
        persistentOverworldParent.SetActive(true);
        CombatController.Instance.InCombat = false;
    }
}
