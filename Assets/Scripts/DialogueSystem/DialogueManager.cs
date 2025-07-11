using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : NetworkBehaviour {

    public static DialogueManager Instance;

    [Header("List of dialogues")]
    [SerializeField] List<DialogueSO> listOfDialogues = new();
    DialogueSO currentDialogue;

    [Header("Dialogue Components")]
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] Image characterImage;
    [SerializeField] Image SkipImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI dialogueBox;
    [SerializeField] Image[] arrayOfChecks;
    bool isSkippingTyping = false;

    [Header("Sprites")]
    [SerializeField] Sprite melSprite;
    [SerializeField] Sprite maevisSprite;
    [SerializeField] Sprite blackBeardSprite;
    [SerializeField] Sprite zombieSprite;
    [SerializeField] Sprite crewSprite;
    [SerializeField] Texture2D normalMouseSprite;
    [SerializeField] Texture2D aimMouseSprite;

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
    Coroutine detectInputCoroutine;
    Coroutine dialogueRoutine;

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

        skipTimer = 0;
        skipAction.Enable();
        skipAction.performed += ctx => StartSkipCheck();
        skipAction.canceled += ctx => CancelSkipCheck();

        amountOfPlayersVotedToSkipDialogue.OnValueChanged -= ChangeSkipText;
        amountOfPlayersVotedToSkipDialogue.OnValueChanged += ChangeSkipText;
    }

    void DisableSkipAction() {
        skipAction.Disable();
        skipAction.performed -= ctx => StartSkipCheck();
        skipAction.canceled -= ctx => CancelSkipCheck();

        amountOfPlayersVotedToSkipDialogue.OnValueChanged -= ChangeSkipText;

        SkipImage.fillAmount = 0f;
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
        if (newInt == 1) arrayOfChecks[0].gameObject.SetActive(true);
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

        EnableSkipAction(); // Habilitando fun��es para pular o di�logo

        HandleDialogue(dialogueID); // Fazendo convers�es e iniciando a corrotina de dialogo

        if (LocalWhiteBoard.Instance.IsAiming) Cursor.SetCursor(normalMouseSprite, Vector3.zero, CursorMode.Auto);
    }

    void StopTime() {
        LocalWhiteBoard.Instance.AnimationOn = true;
        Time.timeScale = 0f;
    }

    void ClearTexts() {
        foreach (var check in arrayOfChecks) check.gameObject.SetActive(false);
        dialogueBox.text = "";
        SkipImage.fillAmount = 0 / timeToSkipDialogue;

        isSkippingTyping = false;
        hasVoted = false;
        isHoldingSkip = false;

        if (dialogueRoutine != null) {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        if (detectInputCoroutine != null) {
            StopCoroutine(detectInputCoroutine);
            detectInputCoroutine = null;
        }
    }

    void HandleDialogue(int dialogueId) {
        amountOfPlayersFinishedWithDialogue.OnValueChanged -= ChangeFinishedText;
        amountOfPlayersFinishedWithDialogue.OnValueChanged += ChangeFinishedText;

        DialogueSO dialogue = IntToDialogue(dialogueId);

        currentDialogue = dialogue;

        dialogueCanvas.SetActive(true);

        dialogueRoutine = StartCoroutine(DialogueRoutine(dialogue));
    }

    void ChangeFinishedText(int oldInt, int newInt) {
        if (newInt == 1) arrayOfChecks[2].gameObject.SetActive(true);
        if (amountOfPlayersFinishedWithDialogue.Value >= NetworkManager.Singleton.ConnectedClientsList.Count) EndDialogueRpc(); // verificando se os dois jogadores terminaram o dialogo
    }

    IEnumerator DialogueRoutine(DialogueSO dialogue) {
        isSkippingTyping = false;

        while (amountOfPlayersFinishedWithDialogue.Value < NetworkManager.Singleton.ConnectedClientsList.Count) {
            for (int i = 0; i < dialogue.ListOfDialogues.Count; i++) {

                // Entre cada fala

                dialogueBox.text = ""; // Limpando a caixa de texto

                characterImage.sprite = dialogue.ListOfDialogues[i].Character switch { // trocando a imagem do personagem 
                    DialogueCharacter.MEL => melSprite,
                    DialogueCharacter.MAEVIS => maevisSprite,
                    DialogueCharacter.BLACKBEARD => blackBeardSprite,
                    DialogueCharacter.CREW => crewSprite,
                    _ => zombieSprite
                };

                characterName.text = dialogue.ListOfDialogues[i].Character.ToString(); // trocando o nome do personagem

                if (!dialogue.ListOfDialogues[i].InitialDialogueSound.IsNull) RuntimeManager.PlayOneShot(dialogue.ListOfDialogues[i].InitialDialogueSound);

                yield return null;

                detectInputCoroutine ??= StartCoroutine(DetectMouseInput());

                for (int j = 0; j < dialogue.ListOfDialogues[i].Text.Length; j++) {
                    if (isSkippingTyping) { // Caso o jogador pule a digita��o do texto
                        dialogueBox.text = dialogue.ListOfDialogues[i].Text;
                        isSkippingTyping = false;
                        yield return null;
                        break;
                    }

                    dialogueBox.text += dialogue.ListOfDialogues[i].Text[j];
                    //RuntimeManager.PlayOneShot(soundPerLetter);
                    yield return new WaitForSecondsRealtime(timeBetweenEachLetter);
                }

                if (detectInputCoroutine != null) StopCoroutine(detectInputCoroutine);
                detectInputCoroutine = null;
                isSkippingTyping = false;

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                

            }

            PlayerFinishedDialogueRpc(); // Avisando o servidor que um player terminou o dialogo
        }
    }

    IEnumerator DetectMouseInput() {
        while (!isSkippingTyping) {
            if (Input.GetMouseButtonDown(0)) {
                isSkippingTyping = true;
                yield break;
            }

            yield return null;
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

        if (dialogueRoutine != null) {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        if (detectInputCoroutine != null) {
            StopCoroutine(detectInputCoroutine);
            detectInputCoroutine = null;
        }

        dialogueCanvas.SetActive(false);

        Time.timeScale = 1f;

        LocalWhiteBoard.Instance.AnimationOn = false;

        if (LocalWhiteBoard.Instance.IsAiming) Cursor.SetCursor(aimMouseSprite, new Vector2(32, 32), CursorMode.Auto);
    }
    #endregion

    #region Getters

    public int DialogueToInt(DialogueSO dialogue) => listOfDialogues.IndexOf(dialogue);

    DialogueSO IntToDialogue(int dialogueId) => listOfDialogues[dialogueId];

    #endregion
}
