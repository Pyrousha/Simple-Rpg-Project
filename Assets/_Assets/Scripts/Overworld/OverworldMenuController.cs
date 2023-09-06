using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static OverworldMenuButton;

public class OverworldMenuController : Singleton<OverworldMenuController>
{
    private enum OverworldMenuSubmenus_Enum
    {
        Main,
        Items,
        SpellArts,
        Settings,
        Equip,
        Attributes,
        Save
    }

    [SerializeField] private UIButton itemsButton;
    [SerializeField] private UIButton spellArtsButton;
    [SerializeField] private UIButton SettingsButton;
    [SerializeField] private UIButton equipButton;
    [SerializeField] private UIButton attributesButton;
    [SerializeField] private UIButton saveButton;

    [Space(10)]
    [SerializeField] private GameObject prefab_itemKey;
    [SerializeField] private GameObject prefab_itemConsumable;
    [SerializeField] private GameObject prefab_equipment;
    [SerializeField] private GameObject prefab_abilitySpell;

    private OverworldMenuSubmenus_Enum currSubmenu;
    private OverworldEntity selectedCharacter;

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
            Debug.Log("Pressed cancel when menu open");
            switch (currSubmenu)
            {
                case OverworldMenuSubmenus_Enum.Main:
                    CloseMenu();
                    return;
            }
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

        currSubmenu = OverworldMenuSubmenus_Enum.Main;

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

    public void OnButtonClicked(MenuButtons_Overworld_Enum _buttonType)
    {
        if (isMenuActive == false)
            return;

        switch (_buttonType)
        {
            case MenuButtons_Overworld_Enum.Items:
                break;
            case MenuButtons_Overworld_Enum.Spells:
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

    public void OnClickedSubmenuSelectable(SubmenuSelectable submenuSelectable)
    {

    }
}
