using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTreeTwoOil : EnemyAttackPrefab {
    ZombieTreeTwo _info;
    Vector3 position;
    public List<MovementManager> _listOfPlayers = new();

    EventInstance sound;

    public override void StartAttack(int enemyId, int skillId, Vector3 pos) {
        base.StartAttack(enemyId, skillId);

        _info = EnemySkillConverter.Instance.TransformIdInSkill(skillId) as ZombieTreeTwo;

        position = new Vector3(pos.x, GetFloorHeight(pos), pos.z);

        SetPosition();
    }

    void SetPosition() {

        GetComponent<MeshRenderer>().material = _info.oilNormalMaterial;

        transform.localScale = new Vector3(_info.oilSize, 0.1f, _info.oilSize);

        transform.SetPositionAndRotation(position, parent.transform.rotation);

        gameObject.SetActive(true);

        if (!_info.OilSound.IsNull) RuntimeManager.PlayOneShot(_info.OilSound, transform.position);

        StartCoroutine(Duration());

    }

    float GetFloorHeight(Vector3 position) {
        Ray ray = new(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Floor"))) return hit.point.y + 0.1f;
        return position.y;
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.oilDuration);

        End();
    }

    public override void End() {
        foreach (var player in _listOfPlayers) {
            player.IncreaseMoveSpeed(_info.oilSpeedReduction);
        }
        _listOfPlayers.Clear();

        if (sound.isValid()) {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            sound.release();
        }
        base.End();
    }

    public void Burn() {
        StartCoroutine(DamageRoutine());

        if (!_info.OilFireSound.IsNull) {
            sound = RuntimeManager.CreateInstance(_info.OilFireSound);
            RuntimeManager.AttachInstanceToGameObject(sound, this.gameObject);
            sound.start();
        }
    }

    IEnumerator DamageRoutine() {

        gameObject.GetComponent<MeshRenderer>().material = _info.oilBurningMaterial;

        while (true) {
            foreach (var player in _listOfPlayers) {
                player.GetComponent<HealthManager>().DealDamage(_info.oilDamage, true, true);
                yield return new WaitForSeconds(_info.oilDamageInterval);
            }
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        if (!_listOfPlayers.Contains(movement)) _listOfPlayers.Add(movement);

        movement.DecreaseMoveSpeed(_info.oilSpeedReduction);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Mel") && !other.CompareTag("Maevis")) return;

        if (!other.TryGetComponent<MovementManager>(out MovementManager movement)) return;

        if (_listOfPlayers.Contains(movement)) _listOfPlayers.Remove(movement);

        movement.IncreaseMoveSpeed(_info.oilSpeedReduction);
    }
}
