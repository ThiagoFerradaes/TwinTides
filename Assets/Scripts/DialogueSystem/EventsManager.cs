using Unity.Netcode;
using UnityEngine;

public class EventsManager : NetworkBehaviour
{
    [Header("Key Event")]
    [SerializeField] DialogueSO keyDialogue;
    bool hasSeenKeyDialogue = false;

    [Header("Deafeat Camps")]
    [SerializeField] DialogueSO firstCampDialogue;
    [SerializeField] DialogueSO secondCampDialogue;
    bool hasSeenFirstCampDialogue = false;
    bool hasSeenSecondCampDialogue = false;
    int amountOfCampsDefeated = 0;

    [Header("Legendary Camp")]
    [SerializeField] DialogueSO legendaryCampDialogue;
    bool hasSeenLegendaryCampDialogue = false;

    #region Initialize
    void Start()
    {
        Chest.OnKeyObtain += OnObteinKey;
        Camps.OnAllEnemiesDeadStatic += CheckAmountOfCamps;
        Camps.OnLegendaryCampDefeat += LegendaryCampEvent;
    }



    public override void OnDestroy() {
        try {
            Chest.OnKeyObtain -= OnObteinKey;
            Camps.OnAllEnemiesDeadStatic -= CheckAmountOfCamps;
            Camps.OnLegendaryCampDefeat -= LegendaryCampEvent;
        }
        catch { }
    }
    #endregion
    #region KeyEvent
    private void OnObteinKey() {
        if (hasSeenKeyDialogue) return;
        KeyEventRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void KeyEventRpc() {
        hasSeenKeyDialogue = true;

        if (!IsServer) return;

        int dialogueId = DialogueManager.Instance.DialogueToInt(keyDialogue);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }
    #endregion

    #region CampsEvent
    private void CheckAmountOfCamps() {
        if (!IsServer) return;

        amountOfCampsDefeated++;

        if (amountOfCampsDefeated == 1 && !hasSeenFirstCampDialogue) FirstCampEvent();
        else if (amountOfCampsDefeated == 2 && !hasSeenSecondCampDialogue) SecondCampEvent();
    }

    void FirstCampEvent() {
        hasSeenFirstCampDialogue = true;
        int dialogueId = DialogueManager.Instance.DialogueToInt(firstCampDialogue);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }

    void SecondCampEvent() {
        hasSeenSecondCampDialogue = true;
        int dialogueId = DialogueManager.Instance.DialogueToInt(secondCampDialogue);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }
    #endregion

    #region LegendaryCampEvent
    private void LegendaryCampEvent() {
        if (!IsServer || hasSeenLegendaryCampDialogue) return;

        hasSeenLegendaryCampDialogue = true;

        int dialogueId = DialogueManager.Instance.DialogueToInt(legendaryCampDialogue);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
    }
    #endregion
}
