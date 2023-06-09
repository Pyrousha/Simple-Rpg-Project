using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterMenuDisplay : MonoBehaviour
{
    private OverworldEntity entity;

    [Space(10)]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI levelNum;
    [SerializeField] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;
    [SerializeField] private Menubar xpBar;

    private bool subscribedToAction = false;

    public void SetEntity(OverworldEntity _newEntity)
    {
        entity = _newEntity;

        if (entity == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            return;
        }

        characterPortrait.sprite = entity.BaseStats.Sprite_Down;
        characterName.text = entity.BaseStats.name;
        levelNum.text = "Lv " + entity.Level;
        hpBar.SetUIValues(entity.Hp, entity.MaxHp.Value);
        mpBar.SetUIValues(entity.Mp, entity.MaxMp.Value);
        xpBar.SetUIValues(entity.Xp);

        entity.SetHealthBars += UpdateHpUI;
        entity.SetManaBars += UpdateMpUI;
        entity.SetXpBarAndLevelNum += UpdateXpAndLevelUI;
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
    private void UpdateXpAndLevelUI(Utils.RangedInt _rangedInt, int _level)
    {
        xpBar.SetUIValues(_rangedInt);
        levelNum.text = "Lv " + _level;
    }

    void OnDestroy()
    {
        if (subscribedToAction)
        {
            entity.SetHealthBars -= UpdateHpUI;
            entity.SetManaBars -= UpdateMpUI;
            entity.SetXpBarAndLevelNum -= UpdateXpAndLevelUI;
        }
    }
}
