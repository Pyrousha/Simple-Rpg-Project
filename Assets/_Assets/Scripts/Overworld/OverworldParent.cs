using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldParent : Singleton<OverworldParent>
{
    [field: SerializeField] public Transform OverworldCamera { get; private set; }
}
