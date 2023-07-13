using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BeauRoutine;
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

    private void SetSprite(Sprite _sprite)
    {
        combatSprite.sprite = _sprite;
        spriteShadow.sprite = _sprite;
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

    private Routine moveCoroutine = Routine.Null;
    public void SetIsForward(bool _isFoward)
    {
        moveCoroutine.Stop();

        Vector3 targPos = InCombatPosition;
        if (_isFoward)
        {
            //Party members move forward when "forward", enemies move back (both move towards the center of the screen) 
            if (OverworldEntity.IsPlayer)
                targPos += new Vector3(0, 0, 0.5f);
            else
                targPos -= new Vector3(0, 0, 0.5f);
        }

        moveCoroutine = Routine.Start(this, MoveToPos(targPos, 0.25f));
    }

    private IEnumerator MoveToPos(Vector3 _targetPos, float _duration)
    {
        yield return transform.MoveTo(_targetPos, _duration).Ease(Curve.QuadOut);
    }

    /// <summary>
    /// Sets the sprite of this entity based on the relation of its facing direction and the camera's position
    /// </summary>
    /// <param name="_globalForward"></param>
    public void UpdateFacingSprite(Vector3 _globalForward)
    {
        // if (OverworldEntity.IsPlayer)
        //     SetSprite(OverworldEntity.BaseStats.Sprite_Up);
        // else
        //     SetSprite(OverworldEntity.BaseStats.Sprite_Down);

        // return;



        Vector2 planarForward = new Vector2(_globalForward.x, _globalForward.z);
        Vector3 cameraForward3D = CombatTransitionController.Instance.CombatCamera.forward;
        Vector2 cameraForward = new Vector2(cameraForward3D.x, cameraForward3D.z);

        float degDiff = Vector2.SignedAngle(planarForward, cameraForward);
        if (degDiff < 0)
            degDiff += 360;
        Debug.Log(degDiff);
        if (degDiff < 45 || degDiff >= 315)
        {
            SetSprite(OverworldEntity.BaseStats.Sprite_Up);
            return;
        }
        if (degDiff >= 45 && degDiff < 135)
        {
            SetSprite(OverworldEntity.BaseStats.Sprite_Right);
            return;
        }
        if (degDiff >= 135 && degDiff < 225)
        {
            SetSprite(OverworldEntity.BaseStats.Sprite_Down);
            return;
        }
        if (degDiff >= 225 && degDiff < 315)
        {
            SetSprite(OverworldEntity.BaseStats.Sprite_Left);
            return;
        }
    }
}