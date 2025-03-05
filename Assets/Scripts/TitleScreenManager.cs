using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject titleScreen;  // Reference to the TitleScreen panel

    void Start()
    {
        // Ensure TitleScreen is active initially
        if (titleScreen != null)
        {
            titleScreen.SetActive(true);
        }
    }

    public void StartGame()
    {
        // Hide the TitleScreen panel when the game starts
        if (titleScreen != null)
        {
            titleScreen.SetActive(false);
        }
    }
}
