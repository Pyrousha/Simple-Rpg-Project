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

    [SerializeField] private SpriteRenderer combatSprite;
    [SerializeField] private SpriteRenderer spriteShadow;

    public Vector3 CenterOfSprite => combatSprite.transform.position;

    [field: SerializeField, Header("Enemy-Specific Field")]
    public Selectable EnemyButton { get; private set; }

    public OverworldEntity OverworldEntity { get; private set; }
    private bool subscribedToAction = false;

    public void SetOverworldEntity(OverworldEntity _entity)
    {
        OverworldEntity = _entity;

        if (_entity != null)
        {
            gameObject.SetActive(true);

            OverworldEntity.SetHealthBars += SetHealthUI;
            OverworldEntity.SetManaBars += SetManaUI;
            subscribedToAction = true;

            SetHealthUI(OverworldEntity.Hp, OverworldEntity.MaxHp.Value);
            SetManaUI(OverworldEntity.Mp, OverworldEntity.MaxMp.Value);

            combatSprite.sprite = OverworldEntity.BaseStats.Sprite_Down;
            spriteShadow.sprite = OverworldEntity.BaseStats.Sprite_Down;

            hpBar.gameObject.SetActive(true);
            mpBar.gameObject.SetActive(true);

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
        if (hpBar != null)
            hpBar.SetUIValues(_hp, _maxHp);
    }

    private void SetManaUI(int _mp, int _maxMp)
    {
        if (mpBar != null)
            mpBar.SetUIValues(_mp, _maxMp);
    }

    public void OnCombatFinished()
    {
        if (OverworldEntity != null)
            TryUnsubscribe();

        hpBar.gameObject.SetActive(false);
        mpBar.gameObject.SetActive(false);

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
}