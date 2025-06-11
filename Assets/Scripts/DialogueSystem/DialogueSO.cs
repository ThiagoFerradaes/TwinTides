using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Dialogues")]
public class DialogueSO : ScriptableObject
{
    public List<DialogueClass> ListOfDialogues = new();
}

[System.Serializable]
public class DialogueClass {
    public DialogueCharacter Character;
    [TextArea(5,10)]public string Text;
}

public enum DialogueCharacter { MEL, MAEVIS, BLACKBEARD, RANDOMZOMBIE, RANDOMNOISE, CREW }
