using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameOverScreen GameOverScreen;

    // variables
    bool hasGameEnded = false;
    int animationLength;
    string routeTaken; // implement this


    public void PlayerHasDied()
    {
        // play death animation
        // i think its like playerAnimator.setTrigger("NameOfAnimation");
        // calling a method to determine routeTaken?
        StartCoroutine(GameOverSequence(routeTaken)); // again route taken
    }

     

    //string (variable)
    // pass variable stating what route was chosen, into here and into the function below v
    IEnumerator GameOverSequence(string routeTaken)
    {
        if(hasGameEnded == false) // makes sure this game over runs once
        {
            yield return new WaitForSeconds(animationLength); // waits for animationLength seconds
            hasGameEnded = true; 
            Debug.Log("Game Over.");
            GameOverScreen.Setup(routeTaken); // routeTaken still needs to be implemented
        }
    }
}
