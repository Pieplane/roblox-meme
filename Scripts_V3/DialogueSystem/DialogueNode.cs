using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNode", menuName ="Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string npcText;
    public List<PlayerChoice> playerChoices = new List<PlayerChoice>();
}
