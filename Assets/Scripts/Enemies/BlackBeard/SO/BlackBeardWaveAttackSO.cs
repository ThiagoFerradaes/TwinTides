using UnityEngine;

[CreateAssetMenu(menuName = "BlackBeardAttack/ Jump Attack")]
public class BlackBeardWaveAttackSO : BlackBeardSO
{
    [Header("Attack Over 50%")]
    public int AmountOfWaves;
    public float TimeBetweenEachWave;
    public float WaveSpeed;
    public float WaveDamage;
    public float WaveMaxRadius;
    public float WaveInitialRadius;
    public float Cooldown;

    [Header("Attack Under 50%")]
    public int AmountOfWavesStronger;
    public float TimeBetweenEachWaveStronger;
    public float WaveSpeedStronger;
    public float AmountOfShieldGainPerWaveHit;
    public float ShieldDuration;
}
