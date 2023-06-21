using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTransitionController : Singleton<CombatTransitionController>
{
    [SerializeField] private GameObject overworldParent;
    [SerializeField] private GameObject combatParent;
    [SerializeField] private Transform combatCenter;
    [Space(10)]
    [SerializeField] private Transform overworldCamera;
    [SerializeField] private Transform combatCamera_parent;
    [SerializeField] private Transform combatCamera;
    public Transform CombatCamera => combatCamera;
    [Space(10)]
    [SerializeField] private Transform player_overworld;
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
        CombatController.Instance.InCombat = true;
        CombatController.Instance.SetEntitiesForCombat(_enemiesToFight);

        enemy_overworld = _overworldEnemy;

        entitiesInCombat = new List<CombatEntity>();
        foreach (OverworldEntity entity in PartyManager.Instance.PartyMembers)
            entitiesInCombat.Add(entity.CombatEntity);
        foreach (OverworldEntity entity in _enemiesToFight)
            entitiesInCombat.Add(entity.CombatEntity);

        StartCoroutine(ToCombat());
    }

    private IEnumerator ToCombat()
    {
        Vector3 heroToEnemy = enemy_overworld.position - player_overworld.position;
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

        overworldParent.SetActive(false);
        combatParent.SetActive(true);

        combatCameraAnim.SetBool("Status", true);
        combatFloorAnim.SetBool("Status", true);

        yield return null;

        // combatCameraAnim.enabled = true;
        // combatFloorAnim.enabled = true;
        StartCoroutine(SpinCameraParent(cameraStartZAngle, 0));


        foreach (CombatEntity entity in entitiesInCombat)
        {
            if (entity.OverworldEntity.IsPlayer)
                StartCoroutine(LerpCombatEntity(entity, entity.InCombatPosition, 1));
            else
                StartCoroutine(LerpCombatEntity(entity, entity.InCombatPosition, 3));
        }
        // StartCoroutine(LerpCombatEntity(player_combat, player_combat_targetPos.position, 1));
        // StartCoroutine(LerpCombatEntity(enemy_combat, enemy_combat_targetPos.position, 3));

        yield return new WaitForSeconds(lerpDuration);

        CombatController.Instance.OnCombatStarted();
    }

    private void PlaceCombatSpriteRelativeToOverworldSprite(CombatEntity _entity)
    {
        Transform overworldObj = _entity.OverworldEntity.transform;
        Transform combatObj = _entity.transform;

        float horizontalOffset = overworldObj.position.x - overworldCamera.position.x;
        float verticalOffset = overworldObj.position.y - overworldCamera.position.y - 0.5f;

        float yPos = combatObj.position.y;
        Vector3 newCombatPos = combatCamera.position + combatCamera.right * horizontalOffset + combatCamera.up * verticalOffset;
        newCombatPos.y = yPos;

        combatObj.position = newCombatPos;
        _entity.SetStartTransitionPosition(newCombatPos);
        if (_entity.OverworldEntity.IsPlayer)
            _entity.SetFacingDir(new Vector3(0, 0, 1));
        else
            _entity.SetFacingDir(new Vector3(0, 0, -1));
        //_entity.SetFacingDir((combatCenter.position - _entity.transform.position).normalized);
    }

    private IEnumerator SpinCameraParent(float _startZAngle, float _targetZAngle)
    {
        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / cameraSpinDuration);

            combatCamera_parent.eulerAngles = new Vector3(90, 0, Mathf.Lerp(_startZAngle, _targetZAngle, elapsedPercentage));

            yield return null;
        }

        combatCamera_parent.eulerAngles = new Vector3(90, 0, _targetZAngle);
    }

    private IEnumerator LerpCombatEntity(CombatEntity _entity, Vector3 _targetPos, float _targetLocalScale)
    {
        Vector3 startPos = _entity.transform.position;
        Vector3 startScale = _entity.transform.localScale;
        Vector3 targetScale = Vector3.one * _targetLocalScale;

        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / lerpDuration);

            _entity.transform.position = Vector3.Lerp(startPos, _targetPos, elapsedPercentage);
            _entity.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedPercentage);

            if (_entity.OverworldEntity.IsPlayer)
                _entity.SetFacingDir(new Vector3(0, 0, 1));
            else
                _entity.SetFacingDir(new Vector3(0, 0, -1));
            // _entity.SetFacingDir((combatCenter.position - _entity.transform.position).normalized);

            yield return null;
        }
    }

    public void EndCombat()
    {
        StartCoroutine(EndCombatCoroutine());
    }

    private IEnumerator EndCombatCoroutine()
    {
        foreach (CombatEntity entity in entitiesInCombat)
        {
            StartCoroutine(LerpCombatEntity(entity, entity.StartTransitionPosition, 1));
        }

        // StartCoroutine(LerpCombatEntity(player_combat, player_combat_startingPos, 1));
        // StartCoroutine(LerpCombatEntity(enemy_combat, enemy_combat_startingPos, 1));

        StartCoroutine(SpinCameraParent(0, cameraStartZAngle));

        combatCameraAnim.SetBool("Status", false);
        combatFloorAnim.SetBool("Status", false);

        yield return new WaitForSeconds(lerpDuration);

        enemy_overworld.gameObject.SetActive(false);

        combatParent.SetActive(false);
        overworldParent.SetActive(true);
        CombatController.Instance.InCombat = false;
    }
}
