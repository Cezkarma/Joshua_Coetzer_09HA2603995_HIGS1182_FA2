using UnityEngine;

public static class SaveManager
{
    private const string HighScoreKey = "HighScore";

    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public static void SaveHighScore(int highScore)
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();
        Debug.Log($"New High Score Saved: {highScore}");
    }

    public static bool TrySubmitScore(int score)
    {
        int highScore = LoadHighScore();

        if (score <= highScore)
        {
            Debug.Log($"Score {score} did not beat the high score of {highScore}.");
            return false;
        }

        SaveHighScore(score);
        return true;
    }
}
