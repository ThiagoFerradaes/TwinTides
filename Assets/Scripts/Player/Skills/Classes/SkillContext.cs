using Unity.Netcode;
using UnityEngine;

public struct SkillContext: INetworkSerializable {
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;

    public SkillContext(Vector3 playerPosition, Quaternion playerRotation) {
        PlayerPosition = playerPosition;
        PlayerRotation = playerRotation;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue( ref PlayerPosition);
        serializer.SerializeValue( ref PlayerRotation);
    }
}
