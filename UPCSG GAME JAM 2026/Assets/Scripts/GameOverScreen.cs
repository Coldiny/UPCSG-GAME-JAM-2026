using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI routeText;
    public GameObject gameOverUI;

    public void Setup(string routeTaken) // variable is the route taken
    {
        gameOverUI.SetActive(true);
        routeText.text = "ROUTE: "; /* + variable*/
    }

    public void TryAgainButton() // This restarts entire game, cause game over
    {

        SceneManager.LoadScene("TutorialAreaTemplate"); // change to (TutorialArea) when renamed later
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
