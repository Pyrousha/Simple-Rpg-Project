using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterMenuDisplay : UIButton, ISelectHandler
{
    public OverworldEntity Entity { get; private set; }

    [SerializeField] private Image characterPortrait;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI levelNum;
    [SerializeField] private Menubar hpBar;
    [SerializeField] private Menubar mpBar;
    [SerializeField] private Menubar xpBar;

    private bool subscribedToAction = false;

    public void SetEntity(OverworldEntity _newEntity)
    {
        Entity = _newEntity;

        if (Entity == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            return;
        }

        characterPortrait.sprite = Entity.BaseStats.Sprite_Down;
        characterName.text = Entity.BaseStats.name;
        levelNum.text = "Lv " + Entity.Level;
        hpBar.SetUIValues(Entity.Hp, Entity.MaxHp.Value);
        mpBar.SetUIValues(Entity.Mp, Entity.MaxMp.Value);
        xpBar.SetUIValues(Entity.Xp);

        Entity.SetHealthBars += UpdateHpUI;
        Entity.SetManaBars += UpdateMpUI;
        Entity.SetXpBarAndLevelNum += UpdateXpAndLevelUI;
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

    new void OnDestroy()
    {
        if (subscribedToAction)
        {
            Entity.SetHealthBars -= UpdateHpUI;
            Entity.SetManaBars -= UpdateMpUI;
            Entity.SetXpBarAndLevelNum -= UpdateXpAndLevelUI;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        OverworldMenuController.Instance.OnSelectedPartyMember(Entity, C_Selectable);

        base.OnSelect(eventData);
    }

    public void OnClicked()
    {
        OverworldMenuController.Instance.OnClickedPartyMember(Entity);
    }
}
