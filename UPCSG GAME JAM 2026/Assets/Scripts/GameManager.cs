using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool hasGameEnded = false;
    public GameOverScreen GameOverScreen;

    // pass variable stating what route was chosen, into here and into the function below v
    public void GameOver()
    {
        if(hasGameEnded == false)
        {
            hasGameEnded = true;
            Debug.Log("Game Over.");
            GameOverScreen.Setup(/*variable*/);
        }
    }
}
