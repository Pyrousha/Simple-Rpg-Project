using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static OverworldMenuButton;

public class OverworldMenuController : Singleton<OverworldMenuController>
{
    private enum SubmenuState
    {
        CasterSelect,
        ItemSelect,
        TargetSelect
    }

    [SerializeField] private UIButton itemsButton;
    [SerializeField] private UIButton spellArtsButton;
    [SerializeField] private UIButton settingsButton;
    [SerializeField] private UIButton equipButton;
    [SerializeField] private UIButton attributesButton;
    [SerializeField] private UIButton saveButton;

    [Space(10)]
    [SerializeField] private GameObject attributesParent_General;
    [SerializeField] private GameObject attributesParent_PartyMember;
    [SerializeField] private GameObject settingsParent;
    [SerializeField] private GameObject saveParent;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI contextText;

    [Space(10)]
    private List<ItemSpellButton> itemButtons = new List<ItemSpellButton>();
    [SerializeField] private LinkSelectables subButtonParent;
    [SerializeField] private GameObject prefab_itemButton;

    private MenuButtons_Overworld_Enum? submenuType = null;
    //private SubmenuState? submenuState = null;

    private OverworldEntity selectedCharacter;
    private AttackSpell selectedAttackSpell;
    private Item selectedItem;
    private bool isSelectedASpell;

    private Selectable currSelectable;
    private Selectable lastSelectedOverride = null;

    private Animator anim;
    private bool isMenuActive = false;

    private Dictionary<GameObject, Selectable> selectablesMap = new Dictionary<GameObject, Selectable>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (CombatController.Instance.InCombat)
            return;

        if (isMenuActive && InputHandler.Instance.Cancel.Down)
        {
            if (submenuType == null)
            {
                CloseMenu();
                return;
            }

            if (lastSelectedOverride == null)
            {
                lastSelectedOverride.Select();
                lastSelectedOverride = null;
                return;
            }

            GameObject currSelectedObj = EventSystem.current.currentSelectedGameObject;
            if (currSelectedObj == null)
            {
                Debug.LogWarning("Tried to cancel when nothing is selected.");
            }

            if (selectablesMap.ContainsKey(currSelectedObj))
            {
                selectablesMap[currSelectedObj].Select();
                return;
            }

            Debug.LogWarning("Unable to find mapping for " + currSelectedObj.name + ", closing menu.");
            CloseMenu();
            return;
        }

        if (InputHandler.Instance.Menu.Down)
        {
            if (isMenuActive)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    private void OpenMenu()
    {
        if (PauseController.Instance.IsPaused)
            return;

        PauseController.Instance.AddPauser(gameObject);

        isMenuActive = true;
        anim.SetBool("Active", isMenuActive);

        itemsButton.Select(); //Select first button
    }

    private void CloseMenu()
    {
        isMenuActive = false;
        anim.SetBool("Active", isMenuActive);

        EventSystem.current.SetSelectedGameObject(null);

        PauseController.Instance.RemovePauser(gameObject);
    }

    private void PopulateRightMenu(int _buttonsNeeded, MenuButtons_Overworld_Enum _menuType)
    {
        //Disable rightMenu buttons
        for (int i = 0; i < itemButtons.Count; i++)
        {
            itemButtons[i].gameObject.SetActive(false);
        }

        if (_buttonsNeeded == 0)
            return;

        //Spawn more buttons if not enough
        int buttonsToSpawn = _buttonsNeeded - itemButtons.Count;
        for (int i = 0; i < buttonsToSpawn; i++)
        {
            ItemSpellButton newButton = Instantiate(prefab_itemButton, subButtonParent.transform).GetComponent<ItemSpellButton>();
            itemButtons.Add(newButton);
        }

        //Populate items
        if (_menuType == MenuButtons_Overworld_Enum.Items || _menuType == MenuButtons_Overworld_Enum.Equip)
        {
            List<Item> items = InventoryManager.Instance.Items.Keys.ToList();

            //Enable buttons and set items
            for (int i = 0; i < _buttonsNeeded; i++)
            {
                itemButtons[i].gameObject.SetActive(true);
                itemButtons[i].SetItem(items[i], "x" + InventoryManager.Instance.Items[items[i]]);

                LinkSpecifiedToCurrSelectable(itemButtons[i]);

                itemButtons[i].SetButtonInteractable(true);
            }
        }
        else
        {
            //Populate spells
            if (_menuType == MenuButtons_Overworld_Enum.Spells)
            {
                //Enable buttons and set spells
                for (int i = 0; i < _buttonsNeeded; i++)
                {
                    AttackSpell currSpell = selectedCharacter.SpellsAndArts[i];

                    itemButtons[i].gameObject.SetActive(true);
                    itemButtons[i].SetSpell(currSpell);

                    LinkSpecifiedToCurrSelectable(itemButtons[i]);

                    bool canUseSpell = (currSpell.TargetAllies && selectedCharacter.Mp >= currSpell.ManaCost);
                    itemButtons[i].SetButtonInteractable(true, canUseSpell);
                }
            }
        }

        subButtonParent.Link();
    }

    public void OnMainButtonSelected(MenuButtons_Overworld_Enum _buttonType, Selectable _newSelectable)
    {
        if (isMenuActive == false)
            return;

        submenuType = null;
        currSelectable = _newSelectable;

        SetText("Select a menu");

        switch (_buttonType)
        {
            case MenuButtons_Overworld_Enum.Items:
                PopulateRightMenu(InventoryManager.Instance.Items.Count, _buttonType);
                break;

            case MenuButtons_Overworld_Enum.Spells:
                //Disable rightMenu buttons
                for (int i = 0; i < itemButtons.Count; i++)
                {
                    itemButtons[i].gameObject.SetActive(false);
                }
                break;

            case MenuButtons_Overworld_Enum.Attributes:
                //Disable rightMenu buttons
                for (int i = 0; i < itemButtons.Count; i++)
                {
                    itemButtons[i].gameObject.SetActive(false);
                }
                break;

            case MenuButtons_Overworld_Enum.Equip:
                PopulateRightMenu(InventoryManager.Instance.Items.Count, _buttonType);
                break;

            case MenuButtons_Overworld_Enum.Settings:
                //Disable rightMenu buttons
                for (int i = 0; i < itemButtons.Count; i++)
                {
                    itemButtons[i].gameObject.SetActive(false);
                }
                break;

            case MenuButtons_Overworld_Enum.Save:
                //Disable rightMenu buttons
                for (int i = 0; i < itemButtons.Count; i++)
                {
                    itemButtons[i].gameObject.SetActive(false);
                }
                break;
        }
    }

    public void OnMainButtonClicked(MenuButtons_Overworld_Enum _buttonType)
    {
        if (isMenuActive == false)
            return;

        submenuType = _buttonType;

        switch (_buttonType)
        {
            case MenuButtons_Overworld_Enum.Items:
                if (InventoryManager.Instance.Items.Count > 0)
                {
                    itemButtons[0].C_Selectable.Select();
                    //submenuState = SubmenuState.ItemSelect;
                    SetText("Select an item to use");
                }
                else
                    SetTextTemporary("No items in inventory!", 1);

                break;

            case MenuButtons_Overworld_Enum.Spells:
                SetText("Select party member to use a spell");
                SelectFirstPartyMember();
                break;

            case MenuButtons_Overworld_Enum.Attributes:
                break;

            case MenuButtons_Overworld_Enum.Equip:
                break;

            case MenuButtons_Overworld_Enum.Settings:
                break;

            case MenuButtons_Overworld_Enum.Save:
                break;
        }
    }

    private void SelectFirstPartyMember(bool _linkCurrSelectable = true)
    {
        List<Selectable> alivePartyMembers = PartyManager.Instance.AlivePartyMembers();

        if (_linkCurrSelectable)
        {
            foreach (Selectable charSelectable in alivePartyMembers)
                LinkSpecifiedToCurrSelectable(charSelectable);
        }

        currSelectable = alivePartyMembers[0];
        currSelectable.Select();
    }

    public void OnSelectedPartyMember(OverworldEntity _entity, Selectable _selectable)
    {
        selectedCharacter = _entity;
        currSelectable = _selectable;

        switch (submenuType)
        {
            case MenuButtons_Overworld_Enum.Spells:
                PopulateRightMenu(_entity.SpellsAndArts.Count, MenuButtons_Overworld_Enum.Spells);
                break;

            case MenuButtons_Overworld_Enum.Attributes:
                SetTextTemporary("Character attributes not yet implemented");
                break;

            case MenuButtons_Overworld_Enum.Equip:
                SetTextTemporary("Character equip menu not yet implemented");
                break;
        }
    }

    public void OnClickedPartyMember(OverworldEntity _entity)
    {
        selectedCharacter = _entity;

        switch (submenuType)
        {
            case MenuButtons_Overworld_Enum.Spells:

                if (selectedCharacter.SpellsAndArts.Count > 0)
                {
                    itemButtons[0].C_Selectable.Select();

                    //submenuState = SubmenuState.ItemSelect;
                    SetText("Select an art/spell to use");
                }
                else
                    SetTextTemporary("OOP This fella don't know shit!", 1);
                break;

            case MenuButtons_Overworld_Enum.Attributes:
                SetTextTemporary("Character attributes not yet implemented");
                break;

            case MenuButtons_Overworld_Enum.Equip:
                SetTextTemporary("Character equip menu not yet implemented");
                break;
        }
    }

    public void OnSelectedItemSpell(ItemSpellButton _itemSpell)
    {
        lastSelectedOverride = _itemSpell.C_Selectable;
    }

    public void OnClickedItemSpell(ItemSpellButton _itemSpell)
    {
        if (_itemSpell.IsFullyInteractable)
        {
            isSelectedASpell = _itemSpell.IsSpell;

            if (_itemSpell.IsSpell)
                selectedAttackSpell = _itemSpell.Spell;
            else
                selectedItem = _itemSpell.Item;

            //Go to character select
            SelectFirstPartyMember(false);
        }
        else
        {
            //Invalid spell clicked, show feedback
            if (_itemSpell.Spell.TargetAllies == false)
                SetTextTemporary("This spell must be used in combat");
            else
                SetTextTemporary("Not enough MP!");
        }
    }

    private void LinkSpecifiedToCurrSelectable(Selectable _sourceSelectable)
    {
        if (selectablesMap.ContainsKey(_sourceSelectable.gameObject))
        {
            //Found Gameobject, update lastSelectable
            selectablesMap[_sourceSelectable.gameObject] = currSelectable;
            return;
        }

        //Selectable was not found, add to mapping
        selectablesMap.Add(_sourceSelectable.gameObject, currSelectable);
    }

    /// <summary>
    /// Called when the player clicks on a submenu that has dynamic selectables (such as items, spells, party members)
    /// </summary>
    /// <param name="submenuSelectable"></param>
    public void OnClickedSubmenuSelectable(SubmenuSelectable submenuSelectable)
    {
        switch (submenuSelectable.Type)
        {
            case SubmenuSelectable.SubmenuType.Item_Key:
                //Do nothing, key items can't be used
                break;
            case SubmenuSelectable.SubmenuType.Item_Consumable:
                //Select party member to use consumable on
                break;
            case SubmenuSelectable.SubmenuType.Equipment:
                //Select party member to equip armor on
                break;
            case SubmenuSelectable.SubmenuType.AbilitySpell:
                //Select party member to use spell on 
                break;
        }
    }

    private void SetTextTemporary(string _text, float _duration = 1)
    {
        tempTextRoutine.Stop();
        tempTextRoutine = Routine.Start(SetTextRoutine(_text, _duration));

        IEnumerator SetTextRoutine(string _text, float _duration)
        {
            string currText = contextText.text;

            SetText(_text, false);

            yield return _duration;

            SetText(currText, false);
        }
    }

    private Routine tempTextRoutine = Routine.Null;

    public void SetText(string _text, bool _stopRoutine = true)
    {
        if (_stopRoutine)
            tempTextRoutine.Stop();

        contextText.text = _text;
    }
}
