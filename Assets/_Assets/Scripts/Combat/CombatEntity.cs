using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using static BaseCombatEntity;

public class CombatEntity : MonoBehaviour
{
    [SerializeField] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;

    [field: SerializeField] public AlignWithCamera alignWithCamera { get; private set; }
    [SerializeField] private SpriteRenderer combatSprite;
    [SerializeField] private SpriteRenderer spriteShadow;
    [field: SerializeField] public Animator DefendAnim { get; private set; }

    public Vector3 CenterOfSprite => combatSprite.transform.position;

    [field: SerializeField, Header("Enemy-Specific Field")]
    public Selectable EnemyButton { get; private set; }

    public OverworldEntity OverworldEntity { get; private set; }
    private bool subscribedToAction = false;

    public Vector3 InCombatPosition { get; private set; }
    public Vector3 StartTransitionPosition { get; private set; }
    public void SetStartTransitionPosition(Vector3 _pos)
    {
        StartTransitionPosition = _pos;
    }

    private void Awake()
    {
        InCombatPosition = transform.position;
    }

    public void SetOverworldEntity(OverworldEntity _entity)
    {
        OverworldEntity = _entity;

        if (_entity != null)
        {
            gameObject.SetActive(true);

            if (_entity.IsDead == false)
                alignWithCamera.StandUp();

            OverworldEntity.SetHealthBars += SetHealthUI;
            OverworldEntity.SetManaBars += SetManaUI;
            subscribedToAction = true;

            SetHealthUI(OverworldEntity.Hp, OverworldEntity.MaxHp.Value);
            SetManaUI(OverworldEntity.Mp, OverworldEntity.MaxMp.Value);

            combatSprite.sprite = OverworldEntity.BaseStats.Sprite_Down;
            spriteShadow.sprite = OverworldEntity.BaseStats.Sprite_Down;

            hpBar?.gameObject.SetActive(true);
            mpBar?.gameObject.SetActive(true);

            if (!_entity.IsPlayer)
                EnemyButton.interactable = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }



    private void SetHealthUI(int _hp, int _maxHp)
    {
        hpBar?.SetUIValues(_hp, _maxHp);
    }

    private void SetManaUI(int _mp, int _maxMp)
    {
        mpBar?.SetUIValues(_mp, _maxMp);
    }

    public void OnCombatFinished()
    {
        if (OverworldEntity != null)
            TryUnsubscribe();

        hpBar?.gameObject.SetActive(false);
        mpBar?.gameObject.SetActive(false);

        if (EnemyButton != null)
            EnemyButton.interactable = false;
    }

    void OnDestroy()
    {
        TryUnsubscribe();
    }

    private void TryUnsubscribe()
    {
        if (subscribedToAction)
        {
            OverworldEntity.SetHealthBars -= SetHealthUI;
            OverworldEntity.SetManaBars -= SetManaUI;
            subscribedToAction = false;
        }
    }

    public void OnKilled(bool _isPlayer)
    {
        if (_isPlayer == false)
            hpBar?.gameObject.SetActive(false);
    }

    private Coroutine moveCoroutine = null;
    public void SetIsForward(bool _isFoward)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        Vector3 targPos = InCombatPosition;
        if (_isFoward)
        {
            if (OverworldEntity.IsPlayer)
                targPos += new Vector3(0, 0, 0.5f);
            else
                targPos -= new Vector3(0, 0, 0.5f);
        }

        moveCoroutine = StartCoroutine(MoveToPos(targPos, 0.25f));
    }

    private IEnumerator MoveToPos(Vector3 _targetPos, float _duration)
    {
        Vector3 _startPos = transform.position;

        float startTime = Time.time;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = Mathf.Min(1, (Time.time - startTime) / _duration);
            elapsedPercentage = Utils.Accelerate(elapsedPercentage);

            transform.position = Vector3.Lerp(_startPos, _targetPos, elapsedPercentage);

            yield return null;
        }
    }
}