using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionUI : Singleton<ScreenTransitionUI>
{
    [field: SerializeField] public Animator DarkAnim { get; private set; }
    [field: SerializeField] public Animator PlayerAnim { get; private set; }
}
