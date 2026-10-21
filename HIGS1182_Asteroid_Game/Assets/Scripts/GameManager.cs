using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Over Screen")]
    [Tooltip("Only used by GameOverScene. Leave these empty in the other scenes.")]
    [SerializeField] private TMP_Text outcomeText;
    [SerializeField] private TMP_Text amountCollectedText;

    private static int scrapCollected;
    private static int scrapTotal;

    private static string outcome = "You lost!";

    private void Start()
    {
        if (outcomeText != null)
        {
            outcomeText.text = outcome;
        }

        if (amountCollectedText != null)
        {
            amountCollectedText.text = $"You picked up {scrapCollected}/{scrapTotal} pieces of scrap!";
        }
    }

    public void PlayGame() => LoadScene("MainScene");

    public void ShowHowToPlay() => LoadScene("HowToPlayScene");

    public void ShowMainMenu() => LoadScene("MainMenuScene");

    public void QuitGame() => Application.Quit();

    public static void ReportScrap(int collected, int total)
    {
        scrapCollected = collected;
        scrapTotal = total;
    }

    public static void EndRun(string result)
    {
        outcome = result;
        Debug.Log($"Run ended: {result}");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadScene("GameOverScene");
    }

    private static void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene '{sceneName}'.");
        SceneManager.LoadScene(sceneName);
    }
}
