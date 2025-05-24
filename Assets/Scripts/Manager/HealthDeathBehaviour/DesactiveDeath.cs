using UnityEngine;

[CreateAssetMenu(menuName = "DeathBehaviour/Desactive")]
public class DesactiveDeath : DeathBehaviour
{
    public override void Death(GameObject deadObject) {
        deadObject.SetActive(false);
    }

}
