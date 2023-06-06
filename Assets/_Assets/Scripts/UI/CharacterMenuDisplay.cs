using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterMenuDisplay : MonoBehaviour
{
    [SerializeField] private CombatEntity entity;

    [Space(10)]
    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI levelNum;
    [SerializeField] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;
    [SerializeField] private Menubar xpBar;

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
    }
}
