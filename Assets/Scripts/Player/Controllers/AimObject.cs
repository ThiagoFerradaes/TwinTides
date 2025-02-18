using System.Collections;
using UnityEngine;

public class AimObject : MonoBehaviour {
    Coroutine aimAlive;
    PlayerController _father;
    void Start() {
        PlayerSetUp.OnPlayerSpawned += PlayerSetUp_OnPlayerSpawned;
    }

    private void PlayerSetUp_OnPlayerSpawned(GameObject obj) {
        _father = obj.GetComponent<PlayerController>();
        _father.OnAim += AimObject_OnAim;
        _father.aimObject = this.gameObject.transform;
        gameObject.SetActive(false);
    }

    private void AimObject_OnAim(object sender, System.EventArgs e) {

        if (aimAlive != null) {
            StopCoroutine(aimAlive);
            aimAlive = null;
        }
        else {
            transform.position = new Vector3(_father.transform.position.x, transform.position.y, _father.transform.position.z);
        }
            aimAlive = StartCoroutine(AimDuration());
    }

    IEnumerator AimDuration() {
        float elapsedTime = 0f;

        while (elapsedTime < 2) {
            elapsedTime += Time.deltaTime;
            if (_father.isRotatingMouse) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _father.floorLayer)) {
                    transform.position = hit.point;
                }
            }
            else {
                Vector3 controlDirection = new Vector3(_father._rotationInput.x, 0, _father._rotationInput.y).normalized;

                if (controlDirection.sqrMagnitude > 0.01f) {
                    transform.position += (10 * Time.deltaTime * controlDirection);
                }
            }

                yield return null;
        }

        gameObject.SetActive(false);
        aimAlive = null;
    }
}
