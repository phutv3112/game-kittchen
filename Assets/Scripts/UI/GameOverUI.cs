using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textHightScore;


    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsGameOver()) {
            Show();
            if (DeliveryManager.Instance.GetSuccessfulRecipesAmount() > DataManager.Instance.LoadHighScore())
            {
                textHightScore.gameObject.SetActive(true);
                textScore.gameObject.SetActive(false);
            }
            else
            {
                textHightScore.gameObject.SetActive(false);
                textScore.gameObject.SetActive(true);
            }
            DeliveryManager.Instance.SaveHightScore();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}