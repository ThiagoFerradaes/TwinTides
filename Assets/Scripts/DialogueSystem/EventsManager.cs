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
    public int amountOfCampsDefeated = 0;

    [Header("Legendary Camp")]
    [SerializeField] DialogueSO legendaryCampDialogue;
    bool hasSeenLegendaryCampDialogue = false;

    [Header("BlackBeard Events")]
    [SerializeField] DialogueSO blackBeardFinalFormDialogue;
    [SerializeField] DialogueSO blackBeardDeathDialogue;
    bool hasSeenBBFinalFormDialogue = false;
    bool hasSeenBBDeathDialogue = false;

    [Header("Used all Keys Event")]
    [SerializeField] DialogueSO usedAllKeysDialogue;
    bool hasUsedAllKeys = false;

    [Header("End of tutorial Event")]
    [SerializeField] DialogueSO endOfTutorialEvent;
    bool hasSeenEndOfTutorialEvent = false;

    #region Initialize

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();


        Chest.OnKeyObtain += OnObteinKey;
        Camps.OnAllEnemiesDeadStatic += CheckAmountOfCamps;
        Camps.OnLegendaryCampDefeat += LegendaryCampEvent;
        BlackBeardMachineState.OnFinal += BlackBeardFinalFormEvent;
        BlackBeardMachineState.OnDeath += BlackBeardDeathEvent;
        TutorialManager.OnTutorialClosed += TutorialEvent;
        KeyLockManager.OnAllKeysUsed += AllKeysUsedEvent;

    }


    public override void OnDestroy() {
        try {
            Chest.OnKeyObtain -= OnObteinKey;
            Camps.OnAllEnemiesDeadStatic -= CheckAmountOfCamps;
            Camps.OnLegendaryCampDefeat -= LegendaryCampEvent;
            BlackBeardMachineState.OnFinal -= BlackBeardFinalFormEvent;
            BlackBeardMachineState.OnDeath -= BlackBeardDeathEvent;
            TutorialManager.OnTutorialClosed -= TutorialEvent;
            KeyLockManager.OnAllKeysUsed -= AllKeysUsedEvent;
        }
        catch { }
    }

    void EventDialogue(ref bool flag, DialogueSO dialogue) {
        if (!IsServer || flag) return;

        flag = true;

        int dialogueId = DialogueManager.Instance.DialogueToInt(dialogue);
        DialogueManager.Instance.TurnCanvasOn(dialogueId);
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

        if (amountOfCampsDefeated == 1 && !hasSeenFirstCampDialogue) EventDialogue(ref hasSeenFirstCampDialogue, firstCampDialogue);
        else if (amountOfCampsDefeated == 2 && !hasSeenSecondCampDialogue) EventDialogue(ref hasSeenSecondCampDialogue, secondCampDialogue);
    }
    #endregion

    #region LegendaryCampEvent
    private void LegendaryCampEvent() {
        EventDialogue(ref hasSeenLegendaryCampDialogue, legendaryCampDialogue);
    }
    #endregion

    #region BlackBeardEvents
    private void BlackBeardDeathEvent() {
        EventDialogue(ref hasSeenBBDeathDialogue, blackBeardDeathDialogue);
    }

    private void BlackBeardFinalFormEvent() {
        EventDialogue(ref hasSeenBBFinalFormDialogue, blackBeardFinalFormDialogue);
    }
    #endregion

    #region EndOfTutorialEvent
    private void TutorialEvent() {
        EventDialogue(ref hasSeenEndOfTutorialEvent, endOfTutorialEvent);
    }
    #endregion

    #region AllKeysUsedEvent
    private void AllKeysUsedEvent() {
        EventDialogue(ref hasUsedAllKeys, usedAllKeysDialogue);
    }
    #endregion
}
