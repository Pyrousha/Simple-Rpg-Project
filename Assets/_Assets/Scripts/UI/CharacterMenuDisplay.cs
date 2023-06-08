using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterMenuDisplay : MonoBehaviour
{
    private CombatEntity entity;

    [Space(10)]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI levelNum;
    [SerializeField] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;
    [SerializeField] private Menubar xpBar;

    private bool subscribedToAction = false;

    public void SetEntity(CombatEntity _newEntity)
    {
        entity = _newEntity;
    }

    private void Start()
    {
        if (entity == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            return;
        }

        characterPortrait.sprite = entity.MenuPortraitSprite;
        characterName.text = entity.name;
        levelNum.text = "Lv " + entity.Level;
        hpBar.SetUIValues(entity.Hp, entity.MaxHp.Value);
        mpBar.SetUIValues(entity.Mp, entity.MaxMp.Value);
        xpBar.SetUIValues(entity.Xp);

        entity.SetHealthBars += UpdateHpUI;
        entity.SetManaBars += UpdateMpUI;
        subscribedToAction = true;
    }

    private void UpdateHpUI(int _hp, int _maxHp)
    {
        hpBar.SetUIValues(_hp, _maxHp);
    }
    private void UpdateMpUI(int _mp, int _maxMp)
    {
        mpBar.SetUIValues(_mp, _maxMp);
    }

    void OnDestroy()
    {
        if (subscribedToAction)
        {
            entity.SetHealthBars -= UpdateHpUI;
            entity.SetManaBars -= UpdateMpUI;
        }
    }
}
