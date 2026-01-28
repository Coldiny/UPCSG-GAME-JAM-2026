using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public static void SetPause(bool shouldPause)
    {
        IsGamePaused = shouldPause;

        if (shouldPause)
        {
            Time.timeScale = 0f;
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1f;
            Debug.Log("Game Resumed");
        }
    }
}