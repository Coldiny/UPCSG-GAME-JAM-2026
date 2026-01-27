using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{

    public TextMeshProUGUI routeText;

    public void Setup(/*variable*/)
    {
        gameObject.SetActive(true);
        routeText.text = "ROUTE: "; /* + variable*/
    }

    public void TryAgainButton()
    {
        SceneManager.LoadScene("Tutorial Area Template"); // change to (TutorialArea) when renamed later
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
