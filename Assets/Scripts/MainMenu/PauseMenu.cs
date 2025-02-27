using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
