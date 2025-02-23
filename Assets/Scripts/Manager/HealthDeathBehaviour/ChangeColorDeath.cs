using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "DeathBehaviour/ChangeColor")]
public class ChangeColorDeath : DeathBehaviour {

    [SerializeField] float timeToRevive;
    [SerializeField] Material deadMaterial;
    public override void Death(GameObject deadObject) {
        deadObject.GetComponent<MeshRenderer>().material = deadMaterial;

        deadObject.GetComponent<HealthManager>().StartCoroutine(WaitToRevive(deadObject));
    }

    IEnumerator WaitToRevive(GameObject deadObject) {
        yield return new WaitForSeconds(timeToRevive);

        deadObject.GetComponent<HealthManager>().ReviveHandler(100);
    }
}
