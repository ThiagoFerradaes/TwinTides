using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : NetworkBehaviour {

    public static DialogueManager Instance;

    [Header("List of dialogues")]
    [SerializeField] List<DialogueTrigger> listOfDialogues = new();
    DialogueTrigger currentDialogue;

    [Header("Dialogue Components")]
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] Image characterImage;
    [SerializeField] Image SkipImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI dialogueBox;
    [SerializeField] TextMeshProUGUI skipBox;
    [SerializeField] TextMeshProUGUI finishedBox;

    [Header("Sprites")]
    [SerializeField] Sprite melSprite;
    [SerializeField] Sprite maevisSprite;
    [SerializeField] Sprite blackBeardSprite;

    [Header("Dialogue Atributes")]
    [SerializeField] float timeBetweenEachLetter;
    [SerializeField] float timeToSkipDialogue;
    private NetworkVariable<int> amountOfPlayersFinishedWithDialogue = new();
    private NetworkVariable<int> amountOfPlayersVotedToSkipDialogue = new();

    [Header("Input Action")]
    [SerializeField] InputAction skipAction;
    float skipTimer = 0f;
    bool isHoldingSkip = false;
    bool hasVoted = false;
    Coroutine skipCoroutine;

    [Header("Sounds")]
    [SerializeField] EventReference melSoundPerLetter;
    [SerializeField] EventReference maevisSoundPerLetter;
    [SerializeField] EventReference blackBeardSoundPerLetter;

    #region Initialize
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else { Destroy(this); }
    }

    #endregion

    #region SkipDialogueRegion
    void EnableSkipAction() {
        hasVoted = false;

        skipAction.Enable();
        skipAction.performed += ctx => StartSkipCheck();
        skipAction.canceled += ctx => CancelSkipCheck();

        amountOfPlayersVotedToSkipDialogue.OnValueChanged += ChangeSkipText;
    }

    void DisableSkipAction() {
        skipAction.Disable();
        skipAction.performed -= ctx => StartSkipCheck();
        skipAction.canceled -= ctx => CancelSkipCheck();

        amountOfPlayersVotedToSkipDialogue.OnValueChanged -= ChangeSkipText;
    }

    void StartSkipCheck() {
        if (hasVoted) return;

        isHoldingSkip = true;

        skipCoroutine ??= StartCoroutine(SkipVoteRoutine());

    }

    void CancelSkipCheck() {
        isHoldingSkip = false;
    }

    IEnumerator SkipVoteRoutine() {
        while (!hasVoted) {
            if (isHoldingSkip) {
                skipTimer += Time.unscaledDeltaTime;
                skipTimer = Mathf.Min(skipTimer, timeToSkipDialogue);
            }
            else {
                skipTimer -= Time.unscaledDeltaTime;
                skipTimer = Mathf.Max(skipTimer, 0f);
            }

            SkipImage.fillAmount = skipTimer / timeToSkipDialogue;

            if (skipTimer >= timeToSkipDialogue) {
                hasVoted = true;
                IncreasePlayerVotedAmountRpc();
                break;
            }

            yield return null;
        }

        yield return null;

        skipCoroutine = null;
    }

    void ChangeSkipText(int oldInt, int newInt) {
        skipBox.text = $"Skip {newInt} / 2";
        if (newInt == NetworkManager.Singleton.ConnectedClientsList.Count) EndDialogueRpc();
    }

    [Rpc(SendTo.Server)]
    void IncreasePlayerVotedAmountRpc() {
        amountOfPlayersVotedToSkipDialogue.Value++;
    }
    #endregion

    #region Dialogue
    public void TurnCanvasOn(int dialogueID) {
        if (!IsServer) return;

        amountOfPlayersFinishedWithDialogue.Value = 0;
        amountOfPlayersVotedToSkipDialogue.Value = 0;

        StartDialogueRpc(dialogueID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void StartDialogueRpc(int dialogueID) {
        StopTime(); // Zerando o time scale

        ClearTexts(); // Limpando Caixas de texto

        EnableSkipAction(); // Habilitando funções para pular o diálogo

        HandleDialogue(dialogueID); // Fazendo conversões e iniciando a corrotina de dialogo

    }

    void StopTime() {
        LocalWhiteBoard.Instance.AnimationOn = true;
        Time.timeScale = 0f;
    }

    void ClearTexts() {
        skipBox.text = "Skip 0 / 2";
        finishedBox.text = "Players finished: 0 / 2";
        dialogueBox.text = "";
        SkipImage.fillAmount = 0 / timeToSkipDialogue;
    }

    void HandleDialogue(int dialogueId) {

        amountOfPlayersFinishedWithDialogue.OnValueChanged += ChangeFinishedText;

        DialogueTrigger dialogueHit = IntToDialogue(dialogueId);

        currentDialogue = dialogueHit;

        DialogueSO dialogue = dialogueHit.Dialogue;

        dialogueCanvas.gameObject.SetActive(true);

        StartCoroutine(DialogueRoutine(dialogue));
    }

    void ChangeFinishedText(int oldInt, int newInt) {
        finishedBox.text = $"Players finished: {newInt} / 2";
        if (amountOfPlayersFinishedWithDialogue.Value >= NetworkManager.Singleton.ConnectedClientsList.Count) EndDialogueRpc(); // verificando se os dois jogadores terminaram o dialogo
    }

    IEnumerator DialogueRoutine(DialogueSO dialogue) {
        bool isSkippingTyping = false;

        while (amountOfPlayersFinishedWithDialogue.Value < NetworkManager.Singleton.ConnectedClientsList.Count) {
            for (int i = 0; i < dialogue.ListOfDialogues.Count; i++) {

                // Entre cada fala

                dialogueBox.text = ""; // Limpando a caixa de texto

                characterImage.sprite = dialogue.ListOfDialogues[i].Character switch { // trocando a imagem do personagem 
                    DialogueCharacter.MEL => melSprite,
                    DialogueCharacter.MAEVIS => maevisSprite,
                    DialogueCharacter.BLACKBEARD => blackBeardSprite,
                    _ => null
                };


                //EventReference soundPerLetter = dialogue.ListOfDialogues[i].Character switch {
                //    DialogueCharacter.MEL => melSoundPerLetter,
                //    DialogueCharacter.MAEVIS => maevisSoundPerLetter,
                //    DialogueCharacter.BLACKBEARD => blackBeardSoundPerLetter,
                //    _ => melSoundPerLetter
                //};


                characterName.text = dialogue.ListOfDialogues[i].Character.ToString(); // trocando o nome do personagem

                for (int j = 0; j < dialogue.ListOfDialogues[i].Text.Length; j++) {
                    if (isSkippingTyping) { // Caso o jogador pule a digitação do texto
                        dialogueBox.text = dialogue.ListOfDialogues[i].Text;
                        break;
                    }

                    dialogueBox.text += dialogue.ListOfDialogues[i].Text[j];
                    //RuntimeManager.PlayOneShot(soundPerLetter);
                    yield return new WaitForSecondsRealtime(timeBetweenEachLetter);
                }

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

                isSkippingTyping = false;
            }

            PlayerFinishedDialogueRpc(); // Avisando o servidor que um player terminou o dialogo
        }
    }

    [Rpc(SendTo.Server)]
    void PlayerFinishedDialogueRpc() {
        amountOfPlayersFinishedWithDialogue.Value++;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void EndDialogueRpc() {
        DisableSkipAction();

        amountOfPlayersFinishedWithDialogue.OnValueChanged -= ChangeFinishedText;

        if (currentDialogue is DialogueHitBox) currentDialogue.gameObject.SetActive(false);

        dialogueCanvas.gameObject.SetActive(false);

        Time.timeScale = 1f;

        LocalWhiteBoard.Instance.AnimationOn = false;
    }
    #endregion

    #region Getters

    public int DialogueToInt(DialogueTrigger dialogue) => listOfDialogues.IndexOf(dialogue);

    DialogueTrigger IntToDialogue(int dialogueId) => listOfDialogues[dialogueId];

    #endregion
}
