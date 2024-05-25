using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StateMachineDefinition", order = 1)]
public class StateMachineDefinition : ScriptableObject
{
    public string[] states;
    public string[] names;
    public string[] origin;
    public string[] destination;
    public string[] transitions;

    [Space(10)]
    [Header("Para las SubStateMachine")]
    public bool cycle;
}
