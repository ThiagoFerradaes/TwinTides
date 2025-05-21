using UnityEngine;
using UnityEngine.UI;

public class BlackBeardHealthUiManager : MonoBehaviour
{
    [SerializeField] Image HealthPlace, HealthBar, HealthEffect;
    [SerializeField] Sprite HealthPlaceSpriteOne, HealthPlaceSpriteTwo;
    [SerializeField] HealthManager BlackBeardHealthManager;

    private void Start() {
        BlackBeardHealthManager.OnHealthUpdate += HealthUpdate;
        BlackBeardHealthManager.GetComponent<BlackBeardMachineState>().OnFinal += BlackBeardHealthUiManager_OnFinal;
    }

    private void HealthUpdate((float maxHealth, float currentHealth, float currentShield) obj) {
        HealthBar.fillAmount = obj.currentHealth / obj.maxHealth;
    }

    private void BlackBeardHealthUiManager_OnFinal() {
        ChangeUI();
    }

    public void TurnUIOn() {
        HealthPlace.sprite = HealthPlaceSpriteOne;
        HealthEffect.gameObject.SetActive(false);

        HealthPlace.gameObject.SetActive(true);
        HealthBar.gameObject.SetActive(true);
    }

    public void TurnUIOff() {
        HealthPlace.gameObject.SetActive(false);
        HealthBar.gameObject.SetActive(false);
        HealthEffect.gameObject.SetActive(false);
    }

    void ChangeUI() {
        HealthEffect.gameObject.SetActive(true);
        HealthPlace.sprite = HealthPlaceSpriteTwo;
    }
}
