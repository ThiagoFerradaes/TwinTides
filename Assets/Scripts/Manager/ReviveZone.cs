using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ReviveZone : NetworkBehaviour
{
    HealthManager _parent;
    [SerializeField] Canvas canvas;
    [SerializeField] Image TimerImage;
    [SerializeField] float timeToRevive;
    [SerializeField] float amountOfReviveHealthPercent;
    float timer;
    Coroutine timerRoutine;
    bool isRevived;
    public override void OnNetworkSpawn() {
        _parent = transform.parent.GetComponent<HealthManager>();
        _parent.OnDeath += PlayerDeath;

        gameObject.SetActive(false);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        _parent.OnDeath -= PlayerDeath;
    }
    private void PlayerDeath() {

        gameObject.SetActive(true);

        isRevived = false;

        TimerImage.fillAmount = timer / timeToRevive;

        if (IsOwner) {
            canvas.worldCamera = Camera.main;
            TimerImage.transform.rotation = canvas.worldCamera.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == _parent.gameObject) return;

        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (timerRoutine != null) {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject == _parent.gameObject) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        timer += Time.deltaTime;

        TimerImage.fillAmount = timer / timeToRevive;

        if (timer >= timeToRevive) {
            Revive();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == _parent.gameObject) return;

        if (!other.CompareTag("Maevis") && !other.CompareTag("Mel")) return;

        timerRoutine = StartCoroutine(TimerDown());
    }

    IEnumerator TimerDown() {
        while (timer > 0) {
            timer -= Time.deltaTime;
            TimerImage.fillAmount = timer / timeToRevive;
            yield return null;
        }

        timerRoutine = null;
    }

    void Revive() {
        if (isRevived) return;

        isRevived = true;

        _parent.ReviveHandler(amountOfReviveHealthPercent);
        _parent.GetComponent<MovementManager>().UnStun();

        gameObject.SetActive(false);
    }
}
