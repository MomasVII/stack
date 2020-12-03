using UnityEngine;
using PnCCasualGameKit;
/// <summary>
/// Responsible for calculating and storing the score and cash
/// </summary>
public class ScoreAndCashManager : LazySingleton<ScoreAndCashManager> {

    public GameOverGhost gog;

    [HideInInspector]
    public int currentScore;
    float currentCash;

    [Tooltip("Score increases by this value")]
    [SerializeField]
    private int scoreUnit = 1;

    [Tooltip("Cash updates after this score")]
    [SerializeField]
    private int cashUpdateFrequency = 5;

    /// <summary>
    /// Registering for events in start
    /// </summary>
    private void Start()
    {
        GameManager.Instance.GameOver += GameOver;
        GameManager.Instance.GameStarted += ResetScore;
    }

    /// <summary>
    /// Updates the score and cash
    /// </summary>
    public void UpdateScore()
    {
        currentScore += scoreUnit;
        if (cashUpdateFrequency!=0 && currentScore % cashUpdateFrequency == 0)
        {
            currentCash++;
        }

        UIManager.Instance.UpdateHudData(currentScore, currentCash);
    }

    /// <summary>
    /// Resets current score to 0 at game start
    /// </summary>
    void ResetScore()
    {
        currentScore = 0;
        currentCash = 0;
    }

    /// <summary>
    /// Save highscore and cash to persistant storage at gamover and send UIManager the gameover data.
    /// </summary>
    void GameOver()
    {
        bool shouldSaveData = false;
        if (currentScore > PlayerData.Instance.highScore)
        {
            shouldSaveData = true;
            PlayerData.Instance.highScore = currentScore;
        }

        if (currentCash > 0)
        {
            shouldSaveData = true;
            PlayerData.Instance.cash += currentCash;
        }

        if (shouldSaveData)
        {
            //Serialization is a heavy operation. Doing it after sometime to avoid lag during gameover.
            Invoke("SaveData", 2);
        }
        UIManager.Instance.UpdateGameOverData(currentScore, PlayerData.Instance.highScore, PlayerData.Instance.cash);
        gog.GamesOver();

    }

    public  void SaveData(){
        PlayerData.Instance.SaveData();
    }

}
