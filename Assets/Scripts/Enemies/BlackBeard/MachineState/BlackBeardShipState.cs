using System.Collections;
using UnityEngine;

public class BlackBeardShipState : BlackBeardStates {

    #region Variaveis
    bool _firstTime = true;
    int _timesAttacked = 0;
    BlackBeardCannon _info;

    #endregion

    #region StartRegion
    public override void StartState(BlackBeardMachineState parent) {
        base.StartState(parent);

        _info = _parent.ListOfAttacks[0] as BlackBeardCannon;

        _timesAttacked = 0;

        if (_firstTime) _parent.StartCoroutine(InitialCooldown());
        else Attack();
    }

    IEnumerator InitialCooldown() {
        _firstTime = false;
        yield return new WaitForSeconds(_info.InitialTime);

        Attack();
    }

    #endregion

    #region Update Region

    void Attack() {
        if (_timesAttacked >= _info.MaxAmountOfAttacksToBomb) BombAttack();
        else {

            int rng = Random.Range(0, 3);

            _timesAttacked++;

            switch (rng) {
                case 0: ShootUpAttack(); break;
                case 1: StraightShootAttack(); break;
                case 2: BombAttack(); break;
            }
        }
    }
    void ShootUpAttack() {
        Debug.Log("Shoot Up Attack");
    }


    void StraightShootAttack() {
        Debug.Log("Straight Shoot Attack");
    }

    void BombAttack() {
        Debug.Log("Bomb Attack");
    }
    #endregion
}
