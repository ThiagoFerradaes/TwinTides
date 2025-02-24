using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class Teste_Dois : NetworkBehaviour {
    Vector3 rightPosition, leftPosition, targetPosition;
    Vector3 patrolOffSet = new(10, 0, 0);
    bool isMovingRight;
    MovementManager mManager;

    private void Start() {
        rightPosition = transform.position + patrolOffSet;
        leftPosition = transform.position - patrolOffSet;
        mManager = GetComponent<MovementManager>();

        if (IsServer) StartCoroutine(MovementCoroutine());
    }

    IEnumerator MovementCoroutine() {
        while (true) {

            targetPosition = isMovingRight ? rightPosition : leftPosition;
            Vector3 direction = targetPosition - transform.position;

            while (Vector3.Distance(transform.position, targetPosition) >= 0.2f) {
                transform.transform.Translate(mManager.ReturnMoveSpeed() * Time.deltaTime * direction.normalized);
                yield return null;
            }

            isMovingRight = !isMovingRight;

        }
    }

}
