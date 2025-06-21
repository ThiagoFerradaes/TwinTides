using UnityEngine;
using UnityEngine.UI;

public class EnemieHealth : MonoBehaviour {
    HealthManager enemie;
    Canvas canvas;
    [SerializeField] Image image;
    void Start() {
        // Pegando os componentes
        canvas = transform.parent.GetComponent<Canvas>();
        enemie = canvas.transform.parent.GetComponent<HealthManager>();

        // Setando outras coisas
        canvas.worldCamera = Camera.main;
        image.fillAmount = 1;
        enemie.OnHealthUpdate += UpdateHealth;
        enemie.OnRevive += OnRevive;
    }

    private void OnRevive() {
        UpdateHealth((enemie.ReturnMaxHealth(), enemie.ReturnCurrentHealth(), enemie.ReturnShieldAmount(), enemie.ReturnMaxShieldAmount()));
    }

    private void UpdateHealth((float maxHealth, float currentHealth, float currentShield, float maxShield) obj) {
        image.fillAmount = obj.currentHealth / obj.maxHealth;
    }

    private void LateUpdate() {
        transform.LookAt(transform.position + canvas.worldCamera.transform.forward);
    }

    private void OnDestroy() {
        if (gameObject.activeInHierarchy) {
            enemie.OnHealthUpdate -= UpdateHealth;
            enemie.OnRevive -= OnRevive;
        }
    }

}
