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
    private List<ItemButton> itemButtons = new List<ItemButton>();
    [SerializeField] private LinkSelectables subButtonParent;
    [SerializeField] private GameObject prefab_itemButton;

    private MenuButtons_Overworld_Enum? submenuType = null;
    private SubmenuState? submenuState = null;

    private OverworldEntity selectedCharacter;
    private AttackSpell selectedAttackSpell;
    private Item selectedItem;

    private Selectable currSelectable;

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
            if (submenuState == null)
            {
                CloseMenu();
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
            ItemButton newButton = Instantiate(prefab_itemButton, subButtonParent.transform).GetComponent<ItemButton>();
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

                AddToSelectablesMap(itemButtons[i].gameObject, currSelectable);
            }

            return;
        }
        else
        {
            //Populate spells
            if (_menuType == MenuButtons_Overworld_Enum.Spells)
            {
                //Enable buttons and set spells
                for (int i = 0; i < _buttonsNeeded; i++)
                {
                    AttackSpell currSpell = selectedCharacter.Spells[i];

                    itemButtons[i].gameObject.SetActive(true);
                    itemButtons[i].SetSpell(currSpell);

                    AddToSelectablesMap(itemButtons[i].gameObject, currSelectable);
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

    private void SelectFirstPartyMember()
    {
        List<Selectable> alivePartyMembers = PartyManager.Instance.AlivePartyMembers();

        foreach (Selectable charSelectable in alivePartyMembers)
            AddToSelectablesMap(currSelectable.gameObject, charSelectable);

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
                PopulateRightMenu(selectedCharacter.Spells.Count, MenuButtons_Overworld_Enum.Spells);
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

                if (itemButtons.Count > 0)
                {
                    itemButtons[0].C_Selectable.Select();
                    //submenuState = SubmenuState.ItemSelect;
                    SetText("Select an item to use");
                }
                else
                    SetTextTemporary("No items in inventory!", 1);
                break;

            case MenuButtons_Overworld_Enum.Attributes:
                SetTextTemporary("Character attributes not yet implemented");
                break;

            case MenuButtons_Overworld_Enum.Equip:
                SetTextTemporary("Character equip menu not yet implemented");
                break;
        }
    }

    private void AddToSelectablesMap(GameObject _currSelectable, Selectable _lastSelectable)
    {
        if (selectablesMap.ContainsKey(_currSelectable))
        {
            //Found Gameobject, update lastSelectable
            selectablesMap[_currSelectable] = _lastSelectable;
            return;
        }

        //Selectable was not found, add to mapping
        selectablesMap.Add(_currSelectable, _lastSelectable);
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

            SetText(_text);

            yield return _duration;

            SetText(currText);
        }
    }

    private Routine tempTextRoutine = Routine.Null;

    public void SetText(string _text)
    {
        tempTextRoutine.Stop();

        contextText.text = _text;
    }
}
