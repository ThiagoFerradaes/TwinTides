using UnityEngine;
using UnityEngine.UI;

public class CreditosManager : MonoBehaviour
{
    [SerializeField] Image textImage;
    [SerializeField] Sprite peopleSprite, musicSprite;
    [SerializeField] GameObject creditosObject;
    [SerializeField] Button leftArrow, rightArrow, crossButton;

    private void Start() {
        leftArrow.onClick.AddListener(ChangeImagePeople);
        rightArrow.onClick.AddListener(ChangeImageMusic);
        crossButton.onClick.AddListener(TurnCreditosOff);
    }

    void ChangeImageMusic() {
        textImage.sprite = musicSprite;
        rightArrow.gameObject.SetActive(false);
        leftArrow.gameObject.SetActive(true);
    }
    void ChangeImagePeople() {
        textImage.sprite = peopleSprite;
        rightArrow.gameObject.SetActive(true);
        leftArrow.gameObject.SetActive(false);
    }

    public void TurnCreditosOn() {
        ChangeImagePeople();
        creditosObject.SetActive(true);
    }

    void TurnCreditosOff() { creditosObject.SetActive(false); }
}
