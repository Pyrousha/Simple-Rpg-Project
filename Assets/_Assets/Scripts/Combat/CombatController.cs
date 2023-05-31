using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private GameObject overworldParent;
    [SerializeField] private GameObject combatParent;
    [Space(10)]
    [SerializeField] private Transform overworldCamera;
    [SerializeField] private Transform combatCamera_parent;
    [SerializeField] private Transform combatCamera;
    [Space(10)]
    [SerializeField] private Transform player_overworld;
    [SerializeField] private Transform enemy_overworld;
    [SerializeField] private Transform player_combat;
    [SerializeField] private Transform enemy_combat;
    [SerializeField] private Transform player_combat_targetPos;
    [SerializeField] private Transform enemy_combat_targetPos;
    [Space(10)]
    [SerializeField] private Animator combatCameraAnim;
    [SerializeField] private Animator combatFloorAnim;
    [Space(5)]
    [SerializeField] private float lerpDuration = 1.0f;
    [SerializeField] private float cameraSpinDuration = 1.0f;

    public void TriggerCombat()
    {
        StartCoroutine(ToCombat());
    }

    private IEnumerator ToCombat()
    {
        Vector3 heroToEnemy = enemy_overworld.position - player_overworld.position;
        float heroToEnemyAngle = Mathf.Atan2(heroToEnemy.y, heroToEnemy.x) * Mathf.Rad2Deg;
        float startAngle = -heroToEnemyAngle + 90.0f;
        if (startAngle > 180)
            startAngle -= 360;
        // Debug.Log(startAngle);
        combatCamera_parent.eulerAngles = new Vector3(90, 0, startAngle);

        yield return null;
        // while (InputHandler.Instance.Interact.Down == false) { yield return null; }

        PlaceCombatSpriteRelativeToOverworldSprite(player_overworld, player_combat);
        PlaceCombatSpriteRelativeToOverworldSprite(enemy_overworld, enemy_combat);

        yield return null;
        // while (InputHandler.Instance.Interact.Down == false) { yield return null; }

        overworldParent.SetActive(false);
        combatParent.SetActive(true);

        combatCameraAnim.SetTrigger("CombatStarted");
        combatFloorAnim.SetTrigger("CombatStarted");

        yield return null;
        // while (InputHandler.Instance.Interact.Down == false) { yield return null; }

        combatCameraAnim.enabled = true;
        combatFloorAnim.enabled = true;
        StartCoroutine(SpinCameraParent(startAngle));

        StartCoroutine(LerpCombatEntity(player_combat, player_combat_targetPos.position, 1));
        StartCoroutine(LerpCombatEntity(enemy_combat, enemy_combat_targetPos.position, 4));
    }

    private void PlaceCombatSpriteRelativeToOverworldSprite(Transform overworldObj, Transform combatObj)
    {
        float horizontalOffset = overworldObj.position.x - overworldCamera.position.x;
        float verticalOffset = overworldObj.position.y - overworldCamera.position.y - 0.5f;

        float yPos = combatObj.position.y;
        Vector3 newCombatPos = combatCamera.position + combatCamera.right * horizontalOffset + combatCamera.up * verticalOffset;
        newCombatPos.y = yPos;

        combatObj.position = newCombatPos;
    }

    private IEnumerator LerpCombatEntity(Transform _entity, Vector3 _targetPos, float _targetScaleMultiplier)
    {
        Vector3 startPos = _entity.position;
        Vector3 startScale = _entity.localScale;
        Vector3 targetScale = startScale * _targetScaleMultiplier;

        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / lerpDuration);

            _entity.position = Vector3.Lerp(startPos, _targetPos, elapsedPercentage);
            _entity.localScale = Vector3.Lerp(startScale, targetScale, elapsedPercentage);

            yield return null;
        }

        _entity.position = _targetPos;
        _entity.localScale = targetScale;
    }

    private IEnumerator SpinCameraParent(float _zAngle)
    {
        float targetZAngle = 0;

        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / cameraSpinDuration);

            combatCamera_parent.eulerAngles = new Vector3(90, 0, Mathf.Lerp(_zAngle, targetZAngle, elapsedPercentage));

            yield return null;
        }

        combatCamera_parent.eulerAngles = new Vector3(90, 0, targetZAngle);
    }
}