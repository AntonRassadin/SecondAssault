using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;


    public void SetVolue(float volume)
    {
       audioMixer.SetFloat("volume", volume);
    }

    public void MainMenuOpen()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OptionsOpen()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
