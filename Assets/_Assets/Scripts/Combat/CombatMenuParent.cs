using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatMenuParent : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    public void OnCombatStarted()
    {
        buttons[0].Select();
    }
}
