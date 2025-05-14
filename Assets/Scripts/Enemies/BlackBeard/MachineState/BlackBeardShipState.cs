using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackBeardShipState : BlackBeardStates {

    #region Variaveis

    // Booleanas
    bool _firstTime = true;

    // ints
    int _timesAttacked = 0;

    // floats
    float healthLimit;

    // SO
    BlackBeardCannon _info;

    // Corrotinas
    Coroutine _cooldownCoroutine;

    // HealthManager do barba negra
    HealthManager _health;

    #endregion

    #region StartRegion
    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);

        _info = _parent.ListOfAttacks[0] as BlackBeardCannon;

        _timesAttacked = 0;

        if (_health == null) _health = _parent.GetComponent<HealthManager>();

        DefineHealthLimit();

        _health.OnGeneralDamage += BlackBeardShipState_OnGeneralDamage;

        if (_firstTime) _parent.StartCoroutine(InitialCooldown());
        else Attack();
    }

    IEnumerator InitialCooldown() {
        _firstTime = false;
        yield return new WaitForSeconds(_info.InitialTime);

        Attack();
    }
    void DefineHealthLimit() {

        int numberOfDivisions = (int)(100 / _info.PercentToChangeState);

        for (int i = 1; i<= numberOfDivisions; i++) {
            float life = ReturnDivision(i, numberOfDivisions);
            if (_health.ReturnCurrentHealth() > life) {
                healthLimit = life;
                break;
            }
        }
    }
    float ReturnDivision(int i, int numberOfDivisions) {
        float health = (_health.ReturnMaxHealth() / numberOfDivisions) * (numberOfDivisions - i);

        return health;
    }

    #endregion

    #region Update Region

    void Attack() {
        //if (_timesAttacked >= _info.MaxAmountOfAttacksToBomb) BombAttack();
        //else {
        //    if (_timesAttacked > 0) {
        //        int rng = Random.Range(0, 3);

        //        switch (rng) {
        //            case 0: _parent.StartCoroutine(ShootUpAttack()); break;
        //            case 1: StraightShootAttack(); break;
        //            case 2: BombAttack(); break;
        //        }
        //    }
        //    else {
        //        int rng = Random.Range(0, 2);

        //        switch (rng) {
        //            case 0: _parent.StartCoroutine(ShootUpAttack()); break;
        //            case 1: StraightShootAttack(); break;
        //        }
        //    }
            _parent.StartCoroutine(ShootUpAttack());
            _timesAttacked++;
        //}
    }
    IEnumerator ShootUpAttack() {
        Queue<Vector3> recentPositions = new(); 
        int maxAttempts = 10;
        int recentCheckCount = 3; 

        for (int i = 0; i < _info.AmountOfShootUpBullets; i++) {
            Vector3 pos = Vector3.zero;
            bool validPos = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++) {
                Vector2 randomPoint = Random.insideUnitCircle * _info.ShootUpRadius;
                pos = _parent.CenterOfArena.position + new Vector3(randomPoint.x, _info.ShootUpBulletHeigh, randomPoint.y);

                validPos = true;

                foreach (Vector3 recentPos in recentPositions) {
                    if (Vector3.Distance(pos, recentPos) < _info.DistanceBetweenEachShootUpBullet) {
                        validPos = false;
                        break;
                    }
                }

                if (validPos) break;
            }

            if (validPos) {
                recentPositions.Enqueue(pos);
                if (recentPositions.Count > recentCheckCount)
                    recentPositions.Dequeue(); // mantém apenas os últimos N
            }
            else {
                pos = _parent.CenterOfArena.position;
            }

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 0, _parent.gameObject, pos);
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, _parent.gameObject, pos);

            yield return new WaitForSeconds(_info.TimeBetweenShootUpBullets);
        }

        _cooldownCoroutine = _parent.StartCoroutine(AttacksCooldown());
    }



    void StraightShootAttack() {
        for (int i = 0; i < _info.AmountOfStraighBullets; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, _parent.gameObject, _parent.CenterOfArena.position);
        }

        _cooldownCoroutine = _parent.StartCoroutine(AttacksCooldown());
    }

    void BombAttack() {
        for (int i = 0; i < _info.AmountOfBombs; i++) {
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, _parent.gameObject, _parent.CenterOfArena.position);
        }

        _timesAttacked = 0;

        _cooldownCoroutine = _parent.StartCoroutine(AttacksCooldown());
    }

    IEnumerator AttacksCooldown() {
        yield return new WaitForSeconds(_info.CooldownBetweenAttacks);

        Attack();
    }

    #endregion

    #region ChangeState

    private void BlackBeardShipState_OnGeneralDamage(object sender, float damage) {
        float newHealth = _health.ReturnCurrentHealth();

        if (newHealth <= healthLimit) ChangeState();
    }

    void ChangeState() { // fim desse estado

        _parent.StopCoroutine(_cooldownCoroutine); // parando corrotina de cooldown

        _parent.GetComponent<HealthManager>().OnGeneralDamage -= BlackBeardShipState_OnGeneralDamage; // Desinscrevendo do evento

        _parent.ChangeState(BlackBeardState.RUNNAWAY); // trocando de estado
    }

    #endregion
}
