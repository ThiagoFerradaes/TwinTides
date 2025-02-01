using UnityEngine;

public class SkillContext {
    public Transform PlayerTransform;

    public SkillContext() {
    }

    public SkillContext(Transform playerTransform) {
        PlayerTransform = playerTransform;
    }
}
