using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BlackBeardShipState : BlackBeardStates {

    #region Variaveis

    // Booleanas
    bool _firstTime = true;
    bool _isStraightShooting = false;

    // ints
    int _timesAttacked = 0;

    // floats
    float healthLimit;

    // SO
    BlackBeardCannon _info;

    // Corrotinas
    Coroutine _cooldownCoroutine;
    Coroutine _attackRoutine;
    Coroutine _cannonRoutine;
    Coroutine _cannonToInitialPositionRoutine;

    // HealthManager do barba negra
    HealthManager _health;

    // Listas
    List<StraightShootPattern> _listOfShootPatterns;
    Vector3[] initialCannonPosition;

    #endregion

    #region StartRegion
    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);
        _info = _parent.ListOfAttacks[0] as BlackBeardCannon;

        _timesAttacked = 0;

        if (_health == null) _health = _parent.GetComponent<HealthManager>();

        if (_listOfShootPatterns == null) CreateListOfPatterns();

        DefineHealthLimit();

        _health.OnHealthUpdate += CheckHealthToChangeState;

        _health.OnDeath += OnDeath;

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

        for (int i = 1; i <= numberOfDivisions; i++) {
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

    void CreateListOfPatterns() {
        _listOfShootPatterns = new() {
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{0, 1, 2, 3}, fireSimultaneously = false},
                    new() { cannonIndexes = new List<int>{3, 2, 1, 0}, fireSimultaneously = false}
                }
            },
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{0, 1, 2, 3}, fireSimultaneously = false},
                    new() { cannonIndexes = new List<int>{0, 1, 2, 3}, fireSimultaneously = false}
                }
            },
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{3, 2, 1, 0}, fireSimultaneously = false},
                    new() { cannonIndexes = new List<int>{0, 1, 2, 3 }, fireSimultaneously = false}
                }
            },
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{3, 2, 1, 0 }, fireSimultaneously = false},
                    new() { cannonIndexes = new List<int>{3, 2, 1, 0}, fireSimultaneously = false}
                }
            },
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{0, 2}, fireSimultaneously = true},
                    new() { cannonIndexes = new List<int>{1, 3}, fireSimultaneously = true}
                }
            },
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{0,1}, fireSimultaneously = true},
                    new() { cannonIndexes = new List<int>{2, 3}, fireSimultaneously = true}
                }
            },
            new StraightShootPattern {
                groups = new List<StraightShootGroup> {
                    new() { cannonIndexes = new List<int>{0, 1, 2, 3}, fireSimultaneously = true},
                    new() { cannonIndexes = new List<int>{0, 1, 2, 3 }, fireSimultaneously = true}
                }
            },
        };
    }

    #endregion

    #region Attack Region

    void Attack() {
        if (_timesAttacked >= _info.MaxAmountOfAttacksToBomb) _parent.StartCoroutine(BombAttack());
        else {
            if (_timesAttacked > 0) {
                int rng = Random.Range(0, 3);

                switch (rng) {
                    case 0: _attackRoutine = _parent.StartCoroutine(ShootUpAttack()); break;
                    case 1: _attackRoutine = _parent.StartCoroutine(StraightShootAttack()); break;
                    case 2: _attackRoutine = _parent.StartCoroutine(BombAttack()); break;
                }
            }
            else {
                int rng = Random.Range(0, 2);

                switch (rng) {
                    case 0: _attackRoutine = _parent.StartCoroutine(ShootUpAttack()); break;
                    case 1: _attackRoutine = _parent.StartCoroutine(StraightShootAttack()); break;
                }
            }

            _timesAttacked++;
        }
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
                    recentPositions.Dequeue();
            }
            else {
                pos = _parent.CenterOfArena.position;
            }

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 0, _parent.gameObject, pos);
            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 1, _parent.gameObject, pos);

            yield return new WaitForSeconds(_info.TimeBetweenShootUpBullets);
        }

        _cooldownCoroutine = _parent.StartCoroutine(AttacksCooldown(_info.CooldownBetweenAttacks));
    }

    IEnumerator StraightShootAttack() {

        _cannonRoutine = _parent.StartCoroutine(MoveCannons());

        for (int i = 0; i < _info.AmountOfStraightBulletsAttacks; i++) {

            var selectedPattern = _listOfShootPatterns[Random.Range(0, _listOfShootPatterns.Count)];

            foreach (var pattern in selectedPattern.groups) {

                if (pattern.fireSimultaneously) {
                    foreach (int bullet in pattern.cannonIndexes) {
                        Vector3 pos = _parent.CannonsPosition[bullet].position;
                        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, _parent.gameObject, pos);
                    }
                }
                else {
                    for (int j = 0; j < pattern.cannonIndexes.Count; j++) {
                        Vector3 pos = _parent.CannonsPosition[pattern.cannonIndexes[j]].position;
                        EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 2, _parent.gameObject, pos);
                        yield return new WaitForSeconds(_info.TimeBetweenStraightBullets);
                    }
                }
                yield return new WaitForSeconds(_info.TimeBetweenStraightBulletsSequence);
            }

            yield return new WaitForSeconds(_info.TimeBetweenStraightBulletsAttacks);

        }

        _isStraightShooting = false;

        _cooldownCoroutine = _parent.StartCoroutine(AttacksCooldown(_info.CooldownBetweenAttacks));
    }

    IEnumerator MoveCannons() {
        _isStraightShooting = true;

        if (initialCannonPosition == null) initialCannonPosition = new Vector3[_parent.CannonsPosition.Length];

        int[] movementDirections = new int[_parent.CannonsPosition.Length];

        for (int i = 0; i < _parent.CannonsPosition.Length; i++) {
            initialCannonPosition[i] = _parent.CannonsPosition[i].position;
            movementDirections[i] = Random.value > 0.5f ? 1 : -1;
        }

        while (_isStraightShooting) {
            for (int i = 0; i < _parent.CannonsPosition.Length; i++) {
                float offset = Mathf.PingPong(Time.time * _info.CannonMovementSpeed + i, _info.CannonMovementRange) - (_info.CannonMovementRange / 2f);
                offset *= movementDirections[i];
                Vector3 basePos = initialCannonPosition[i];
                _parent.CannonsPosition[i].position = basePos + _parent.CannonsPosition[i].right * offset;
            }

            yield return null;
        }

        _cannonRoutine = null;

        _cannonToInitialPositionRoutine = _parent.StartCoroutine(ReturnCannonToPlace());
    }

    IEnumerator ReturnCannonToPlace() {
        bool allReached = false;
        while (!allReached) {
            allReached = true;

            for (int i = 0; i < _parent.CannonsPosition.Length; i++) {
                Transform cannon = _parent.CannonsPosition[i];
                Vector3 target = initialCannonPosition[i];
                cannon.position = Vector3.MoveTowards(cannon.position, target, _info.CannonMovementSpeed * Time.deltaTime);

                if (Vector3.Distance(cannon.position, target) > 0.01f) {
                    allReached = false;
                }
            }
            yield return null;
        }

        _cannonToInitialPositionRoutine = null;
    }

    IEnumerator BombAttack() {
        _timesAttacked = 0;
        Queue<Vector3> recentPositions = new();
        int maxAttempts = 10;
        int recentCheckCount = 3;

        for (int i = 0; i < _info.AmountOfBombs; i++) {
            Vector3 pos = Vector3.zero;
            bool validPos = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++) {
                Vector2 randomPoint = Random.insideUnitCircle * _info.ShootBombRadius;
                pos = _parent.CenterOfArena.position + new Vector3(randomPoint.x, _info.BombHeight, randomPoint.y);

                validPos = true;

                foreach (Vector3 recentPos in recentPositions) {
                    if (Vector3.Distance(pos, recentPos) < _info.DistanceBetweenBombs) {
                        validPos = false;
                        break;
                    }
                }

                if (validPos) break;
            }

            if (validPos) {
                recentPositions.Enqueue(pos);
                if (recentPositions.Count > recentCheckCount)
                    recentPositions.Dequeue();
            }
            else {
                pos = _parent.CenterOfArena.position;
            }

            EnemySkillPooling.Instance.RequestInstantiateAttack(_info, 3, _parent.gameObject, pos);

            yield return new WaitForSeconds(_info.TimeBetweenBombs);
        }

        _cooldownCoroutine = _parent.StartCoroutine(AttacksCooldown(_info.CooldownAfterBomb));
    }

    IEnumerator AttacksCooldown(float cooldown) {
        _attackRoutine = null;

        yield return new WaitForSeconds(cooldown);

        _cooldownCoroutine = null;

        Attack();
    }

    #endregion

    #region ChangeState

    private void CheckHealthToChangeState((float maxHealth, float currentHealth, float currentShield, float maxShield) health) {
        if (_parent.Lifes > 1) {
            float newHealth = health.currentHealth;

            if (newHealth <= healthLimit && newHealth > 0) ChangeState();
        }
    }
    void OnDeath() {
        ChangeState();
    }
    void ChangeState() { // fim desse estado

        CheckCoroutines();

        _parent.GetComponent<HealthManager>().OnHealthUpdate -= CheckHealthToChangeState; // Desinscrevendo do evento
        _parent.GetComponent<HealthManager>().OnDeath -= OnDeath; // Desinscrevendo do evento

        if (_parent.Lifes > 1) _parent.ChangeState(BlackBeardState.RUNNAWAY); // trocando de estado
        else _parent.ChangeState(BlackBeardState.FINAL);
    }

    void CheckCoroutines() {
        if (_cooldownCoroutine != null) _parent.StopCoroutine(_cooldownCoroutine); // parando corrotina de cooldown

        if (_attackRoutine != null) _parent.StopCoroutine(_attackRoutine); // parando corrotina de ataque

        if (_cannonToInitialPositionRoutine != null) _parent.StopCoroutine(_cannonToInitialPositionRoutine); // parando a rotina de volta dos canhoes

        if (_cannonRoutine != null) {
            _parent.StopCoroutine(_cannonRoutine); // parando o movimento dos canhoes
            _cannonToInitialPositionRoutine = _parent.StartCoroutine(ReturnCannonToPlace()); // voltando eles pro lugar caso necessario
        }     
    }

    #endregion
}

public class StraightShootGroup {
    public List<int> cannonIndexes;
    public bool fireSimultaneously;
}

public class StraightShootPattern {
    public List<StraightShootGroup> groups;
}
