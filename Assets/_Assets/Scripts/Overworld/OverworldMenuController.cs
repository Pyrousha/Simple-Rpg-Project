using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static OverworldMenuButton;

public class OverworldMenuController : Singleton<OverworldMenuController>
{
    private enum OverworldMenuSubmenus_Enum
    {
        Main

        //TODO: Fill with submenus (Like "Items", "Equip", etc.)
    }

    private OverworldMenuSubmenus_Enum currSubmenu;

    private Animator anim;
    private bool isMenuActive = false;

    [SerializeField] private Button[] buttons;

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

        foreach (Button button in buttons)
            button.enabled = true;

        buttons[0].Select(); //Select first button
    }

    private void CloseMenu()
    {
        isMenuActive = false;
        anim.SetBool("Active", isMenuActive);

        foreach (Button button in buttons)
            button.enabled = false;

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
}
