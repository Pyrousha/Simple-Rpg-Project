using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverworldMenuController : Singleton<OverworldMenuController>
{
    private Animator anim;
    private bool isMenuActive = false;
    public bool IsMenuActive => isMenuActive;

    public enum MenuButtons_OverworldEnum
    {
        Items,
        Spells,
        CloseMenu,
        Equip,
        Settings,
        Save
    }

    [SerializeField] private Button[] buttons;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.Instance.Menu.Down && !DialogueUI.Instance.isOpen)
        {
            if (isMenuActive)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    private void OpenMenu()
    {
        isMenuActive = true;
        anim.SetBool("Active", isMenuActive);

        buttons[0].Select(); //Select first button
    }

    private void CloseMenu()
    {
        isMenuActive = false;
        anim.SetBool("Active", isMenuActive);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnButtonClicked(MenuButtons_OverworldEnum _buttonType)
    {
        if (isMenuActive == false)
            return;

        switch (_buttonType)
        {
            case MenuButtons_OverworldEnum.Items:
                break;
            case MenuButtons_OverworldEnum.Spells:
                break;
            case MenuButtons_OverworldEnum.CloseMenu:
                CloseMenu();
                break;
            case MenuButtons_OverworldEnum.Equip:
                break;
            case MenuButtons_OverworldEnum.Settings:
                break;
            case MenuButtons_OverworldEnum.Save:
                break;
        }
    }
}
