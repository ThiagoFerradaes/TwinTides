using UnityEngine;

[CreateAssetMenu(menuName = "DeathBehaviour/ BlackBeard")]
public class BlackBeardDeath : DeathBehaviour {
    public override void Death(GameObject deadObject) {
        deadObject.GetComponent<BlackBeardMachineState>().Lifes--;
        deadObject.GetComponent<BlackBeardMachineState>().Death();
    }
}
