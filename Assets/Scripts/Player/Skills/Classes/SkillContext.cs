using Unity.Netcode;
using UnityEngine;

public struct SkillContext: INetworkSerializable {
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public int SkillIdInUI;

    public SkillContext(Vector3 playerPosition, Quaternion playerRotation, int skillIdUI) {
        PlayerPosition = playerPosition;
        PlayerRotation = playerRotation;
        SkillIdInUI = skillIdUI;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue( ref PlayerPosition);
        serializer.SerializeValue( ref PlayerRotation);
        serializer.SerializeValue(ref SkillIdInUI);
    }
}
