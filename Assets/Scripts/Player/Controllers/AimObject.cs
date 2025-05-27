using System.Collections;
using UnityEngine;

public class AimObject : MonoBehaviour {
    Coroutine aimAlive;
    PlayerController _father;

    void Awake() {
        PlayerSetUp.OnPlayerSpawned += PlayerSetUp_OnPlayerSpawned;
    }

    private void PlayerSetUp_OnPlayerSpawned(GameObject obj) {
        _father = obj.GetComponent<PlayerController>();
        _father.aimObject = this.gameObject.transform;
        _father.OnAim += AimObject_OnAim;
        gameObject.SetActive(false);
    }

    private void AimObject_OnAim(object sender, System.EventArgs e) {

        if (aimAlive != null) {
            StopCoroutine(aimAlive);
            aimAlive = null;
        }
        else {
            transform.position = _father.transform.position;
        }
        aimAlive = StartCoroutine(AimDuration());
    }

    IEnumerator AimDuration() {
        float elapsedTime = 0f;

        while (elapsedTime < 1.2f) {
            if (_father.isRotatingMouse) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _father.FloorLayer)) {
                    Vector3 pos = new(hit.point.x, hit.point.y + 0.5f, hit.point.z);
                    transform.position = pos;
                }
            }
            else {
                Vector3 controlDirection = new Vector3(_father._rotationInput.x, 0, _father._rotationInput.y).normalized;

                if (controlDirection.sqrMagnitude > 0.01f) {
                    transform.position += (10 * Time.deltaTime * controlDirection);
                }
            }
            if (!_father.isAiming) {
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        gameObject.SetActive(false);
        aimAlive = null;
    }

    private void OnDisable() {
        aimAlive = null;
    }

    private void OnDestroy() {
        PlayerSetUp.OnPlayerSpawned -= PlayerSetUp_OnPlayerSpawned;
    }
}
