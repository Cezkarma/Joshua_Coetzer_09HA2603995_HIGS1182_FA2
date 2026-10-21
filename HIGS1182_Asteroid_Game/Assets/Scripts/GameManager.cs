using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Over Screen")]
    [Tooltip("Only used by GameOverScene. Leave these empty in the other scenes.")]
    [SerializeField] private TMP_Text outcomeText;
    [SerializeField] private TMP_Text amountCollectedText;

    private static int score;
    private static int wave = 1;
    private static string outcome = "You lost!";

    private void Start()
    {
        if (outcomeText != null)
        {
            outcomeText.text = outcome;
        }

        if (amountCollectedText != null)
        {
            amountCollectedText.text = $"You collected {score} scrap and reached wave {wave}!";
        }
    }

    public void PlayGame() => LoadScene("MainScene");

    public void ShowHowToPlay() => LoadScene("HowToPlayScene");

    public void ShowMainMenu() => LoadScene("MainMenuScene");

    public void QuitGame() => Application.Quit();

    public static void ReportScore(int newScore)
    {
        score = newScore;
    }

    public static void ReportWave(int newWave)
    {
        wave = newWave;
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
