using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlTwoThreePuddle : EnemyAttackPrefab {
    GirlTwoThree _info;
    PuddleType type;

    HashSet<HealthManager> _listOfPlayers = new();
    enum PuddleType { Block, Damage, Slow }
    public override void StartAttack(int enemyId, int skillId) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as GirlTwoThree;

        parentContext.Blackboard.IsAttacking = true;

        ChooseTypeOfPuddle();

        SetPosition();

        EndOfAttack();
    }

    void ChooseTypeOfPuddle() {
        int rng;
        switch (parentContext.Blackboard.CurrentComboIndex) {
            case 1:
                rng = Random.Range(0, 2);
                if (rng == 0) {
                    type = PuddleType.Damage;
                    parentContext.Blackboard.CurrentComboIndex = 2;
                }
                else {
                    type = PuddleType.Slow;
                    parentContext.Blackboard.CurrentComboIndex = 3;
                }
                break;
            case 2:
                rng = Random.Range(0, 2);
                if (rng == 0) {
                    type = PuddleType.Slow;
                    parentContext.Blackboard.CurrentComboIndex = 3;
                }
                else {
                    type = PuddleType.Block;
                    parentContext.Blackboard.CurrentComboIndex = 1;
                }
                break;
            case 3:
                rng = Random.Range(0, 2);
                if (rng == 0) {
                    type = PuddleType.Block;
                    parentContext.Blackboard.CurrentComboIndex = 1;
                }
                else {
                    type = PuddleType.Damage;
                    parentContext.Blackboard.CurrentComboIndex = 2;
                }
                break;
        }

        Material puddleMaterial = type switch {
            PuddleType.Block => _info.blockPuddleMaterial,
            PuddleType.Damage => _info.highDamagePuddleMaterial,
            _ => _info.slowPuddleMaterial
        };

        gameObject.GetComponent<MeshRenderer>().material = puddleMaterial;
    }

    void SetPosition() {
        float size = type switch {
            PuddleType.Block => _info.blockPuddleSize,
            PuddleType.Damage => _info.highDamagePuddleSize,
            _ => _info.slowPuddleSize
        };

        transform.localScale = new(size, 0.1f, size);

        Vector3 pos = GetRandomPointNearPlayer();

        pos.y = GetFloorHeight(pos);

        transform.position = pos;

        gameObject.SetActive(true);

        StartCoroutine(Duration());
    }
    Vector3 GetRandomPointNearPlayer() {
        Vector2 randomCircle = Random.insideUnitCircle * _info.offSett;
        Vector3 offset = new(randomCircle.x, 0f, randomCircle.y);
        return parentContext.Blackboard.Target.position + offset;
    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    IEnumerator Duration() {
        float duration = type switch {
            PuddleType.Block => _info.blockPuddleDuration,
            PuddleType.Damage => _info.highDamagePuddleDuration,
            _ => _info.slowPuddleDuration
        };

        StartCoroutine(DamageRoutine());

        yield return new WaitForSeconds(duration);

        ApplyEndEffect();

        End();
    }

    public override void End() {
        foreach (var player in _listOfPlayers) {
            if (type == PuddleType.Slow) {
                player.GetComponent<MovementManager>().IncreaseMoveSpeed(_info.slowPuddleSlowPercent);
            }
        }

        _listOfPlayers.Clear();

        base.End();
    }

    void ApplyEndEffect() {
        foreach(var player in _listOfPlayers) {
            if (type == PuddleType.Slow) {
                player.GetComponent<MovementManager>().StunWithTime(_info.slowPuddleStunDuration);
            }
            else if (type == PuddleType.Block) player.AddDebuffToList(_info.blockPuddleNoDebuffDebuff);
        }
    }

    IEnumerator DamageRoutine() {
        float damage = type switch {
            PuddleType.Block => _info.blockPuddleDamage,
            PuddleType.Damage => _info.highDamagePuddleDamageToHealth,
            _ => _info.slowPuddleDamage
        };

        while (true) {
            if (type == PuddleType.Damage) {
                foreach (var player in _listOfPlayers) {
                    if (player.ReturnShieldStatus()) player.DealDamage(_info.highDamagePuddleDamageToShield, true, true);
                    else player.DealDamage(_info.highDamagePuddleDamageToHealth, true, true);
                }
            }
            else {
                foreach (var player in _listOfPlayers) {
                    player.DealDamage(damage, true, true);
                }
            }

            yield return new WaitForSeconds(_info.timeBetweenDamage);
        }
    }

    void EndOfAttack() {
        parentContext.Blackboard.IsAttacking = false;
        parentContext.Blackboard.CanAttack = false;
        parentContext.Blackboard.Cooldowns[_info.ListOfAttacksNames[0]] = _info.cooldown;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Add(health);

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.DecreaseMoveSpeed(_info.slowPuddleSlowPercent);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        _listOfPlayers.Remove(health);

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        movement.IncreaseMoveSpeed(_info.slowPuddleSlowPercent);
    }
}
