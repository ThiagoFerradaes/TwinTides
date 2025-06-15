using UnityEngine;

[CreateAssetMenu (menuName = "DeathBehaviour/ PlayerDeath")]
public class PlayersDeath : DeathBehaviour {
    public override void Death(GameObject deadObject) {
        deadObject.GetComponent<MovementManager>().Stun();
    }
}
