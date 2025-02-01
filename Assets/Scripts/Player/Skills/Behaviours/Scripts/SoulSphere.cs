using UnityEngine;

[CreateAssetMenu(menuName = "SkillBehaviour/SoulSphere")]
public class SoulSphere : SkillBehaviour {

    [Header("PreFabs")]
    [SerializeField] GameObject spherePreFab;
    public GameObject sphereAreaPreFab;

    [Header("Sphere Atributes")]
    public float SphereDuration;
    public float Speed;
    public float DamagePassingThrowEnemy;
    public HealthBuff invulnerabilityBuff;

    [Header("Explosion Atributes")]
    public float ExplosionDuration;
    public float ExplosionDamage;

    [Header("Sphere Area Atributes")]
    public float AreaDuration;
    public float TimeBetweenInvulnerabilityInsideArea;
    public float AreaScaleLevel4;
    public override void UseSkill(SkillContext context, int skillLevel) {

        if (PlayersSkillPooling.Instance == null) {
            Debug.LogError("PlayersSkillPooling.Instance is null");
        }

        GameObject sphereObject = PlayersSkillPooling.Instance.GetObjectFromQueue(spherePreFab);
        sphereObject.transform.SetPositionAndRotation(context.PlayerTransform.position, context.PlayerTransform.rotation);
        SoulSphereObject soulSphereObject = sphereObject.GetComponent<SoulSphereObject>();

        Debug.Log("Going to activate sphere");
        sphereObject.SetActive(true);
        soulSphereObject.ActivateSkill(this, skillLevel);

    }
}
