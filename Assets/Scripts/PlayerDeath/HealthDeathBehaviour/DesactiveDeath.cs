using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "DeathBehaviour/Desactive")]
public class DesactiveDeath : DeathBehaviour
{
    
    public override void Death(GameObject deadObject) {
        deadObject.GetComponent<HealthManager>().StartCoroutine(WaitToDie(deadObject));
    }
    IEnumerator WaitToDie(GameObject deadObject) {
        Animator anim = deadObject.GetComponentInChildren<Animator>();
        anim.SetTrigger("IsDead");

        // Espera a transição para o estado começar (evita pegar o estado anterior)
        yield return new WaitForSeconds(0.1f); // ou: yield return null;

        // Espera até que a animação realmente esteja no estado "IsDead"
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.IsName("Dead") == false) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Espera a animação terminar
        while (stateInfo.normalizedTime < 1f) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        deadObject.SetActive(false);
    }

}
