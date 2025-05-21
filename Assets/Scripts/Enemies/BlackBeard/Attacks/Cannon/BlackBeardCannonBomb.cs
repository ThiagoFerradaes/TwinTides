using System.Collections;
using UnityEngine;

public class BlackBeardCannonBomb : BlackBeardAttackPrefab {
    BlackBeardCannon _info;
    bool collided;
    bool pushed;
    Material originalMaterial;
    MeshRenderer mrenderer;

    public override void StartAttack(int enemyId, int skillId, Vector3 position) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as BlackBeardCannon;

        mrenderer = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();

        originalMaterial = mrenderer.material;

        pushed = false;

        SetPosition(position);
    }

    void SetPosition(Vector3 position) {
        transform.position = position;

        transform.localScale = Vector3.one * _info.BombSize;

        gameObject.SetActive(true);

        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine() {
        while (!collided) {
            transform.position += _info.BombFallSpeed * Time.deltaTime * -transform.up;
            yield return null;
        }

        collided = false;

        StartCoroutine(WaitToExplode());

    }

    IEnumerator WaitToExplode() {

        StartCoroutine(ExplosionWarning());

        yield return new WaitForSeconds(_info.TimeToBombExplode);

        if (!pushed) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 4, parent, transform.position);

            End();
        }
    }

    public override void End() {
        mrenderer.material = originalMaterial;
        base.End();
    }

    IEnumerator ExplosionWarning() {
        while (!pushed) {
            yield return new WaitForSeconds(_info.TimeBetweenWarnings);
            mrenderer.material = _info.WarningMaterial;
            yield return new WaitForSeconds(_info.TimeBetweenWarnings);
            mrenderer.material = originalMaterial;
            yield return new WaitForSeconds(_info.WarningCooldown);
        }
        mrenderer.material = originalMaterial;
    }

    public void TryPush(Vector3 attackerPosition) {
        Vector3 toAttacker = (attackerPosition - transform.position).normalized;
        float dot = Vector3.Dot(toAttacker, -transform.forward); 

        if (dot > 0.5f && !pushed) {
            pushed = true;
            StartCoroutine(PushTowardsShip());
        }
    }

    IEnumerator PushTowardsShip() {
        Transform ship = parent.GetComponent<BlackBeardMachineState>().Ship;
        while (Vector3.Distance(transform.position, ship.position) > 0.5f) {
            Vector3 dir = (ship.position - transform.position).normalized;
            transform.position += _info.BombSpeed * Time.deltaTime * dir;
            yield return null;
        }

        parent.GetComponent<HealthManager>().DealDamage(_info.BombDamageToShip, true, false);

        pushed = false;

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Floor")) {
            collided = true;
        }
            
    }
}
