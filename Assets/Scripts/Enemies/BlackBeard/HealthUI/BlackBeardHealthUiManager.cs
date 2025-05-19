using UnityEngine;
using UnityEngine.UI;

public class BlackBeardHealthUiManager : MonoBehaviour
{
    [SerializeField] Image HealthPlace, HealthBar;
    [SerializeField] Sprite HealthPlaceSpriteOne, HealthPlaceSpriteTwo, HealthBarSpriteOne, HealthBarSpriteTwo;
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
        HealthPlace.gameObject.SetActive(true);
        HealthBar.gameObject.SetActive(true);
    }

    void TurnUIOff() {
        HealthPlace.gameObject.SetActive(false);
        HealthBar.gameObject.SetActive(false);
    }

    void ChangeUI() {
        HealthBar.sprite = HealthBarSpriteTwo;
        HealthPlace.sprite = HealthPlaceSpriteTwo;
    }
}
