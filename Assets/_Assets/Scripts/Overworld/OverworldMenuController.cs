using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMenuController : MonoBehaviour
{
    private Animator anim;
    private bool isMenuActive = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.Instance.Menu.Down)
        {
            isMenuActive = !isMenuActive;
            anim.SetBool("Active", isMenuActive);
        }
    }
}
