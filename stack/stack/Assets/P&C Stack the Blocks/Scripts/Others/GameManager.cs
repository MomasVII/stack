using UnityEngine;
using UnityEngine.Events;
using PnCCasualGameKit;
/// <summary>
/// Consits of Game state events and other General Game specific functionalities. 
/// </summary>
public class GameManager : LazySingleton<GameManager>
{
    /// <summary>
    ///  Game state events
    /// </summary>
    public System.Action GameInitialized, GameStarted, GameContinued, GameOver;

    /// <summary>
    /// Unity events for the above states
    /// </summary>
    public UnityEvent OnGameIntialized, OnGameStarted, OnGameOver;

    //To avoid unnecessary platform speficic warning
#pragma warning disable
    [SerializeField]
    private string playStoreURL = null;
    [SerializeField]
    private string appStoreURL = null;
#pragma warning restore

    private void Awake()
    {
        // Make the game run as fast as possible. Typically on mobile platforms the default frame rate is 30 frames per second.
        Application.targetFrameRate = 300;
        PlayerData.Create();
        GameInitialized += delegate { OnGameIntialized.Invoke(); };
        GameStarted += delegate { OnGameStarted.Invoke(); };
        GameOver += delegate { OnGameOver.Invoke(); };
    }

    private void OnDestroy()
    {
        GameInitialized -= delegate { OnGameIntialized.Invoke(); };
        GameStarted -= delegate { OnGameStarted.Invoke(); };
        GameOver -= delegate { OnGameOver.Invoke(); };
    }

    /// <summary>
    /// Init Game in Start because other classes register for events in Awake
    /// </summary>
    void Start()
    {
        InitGame();
        if(PlayerData.Instance.adsRemoved){
            UIManager.Instance.removeAdBtn.SetActive(false);
        }
    }

    /// <summary>
    /// Initialise game
    /// </summary>
    public void InitGame()
    {
        //In editor, the game doesn't pause when ad is shown. Workaround : set timescale 0 when showing ad.
#if UNITY_EDITOR
        if (Time.timeScale == 0)
            return;
#endif

        if (GameInitialized != null)
            GameInitialized();
    }

    /// <summary>
    /// Start Game
    /// </summary>
    public void StartGame()
    {
        //In editor, the game doesn't pause when ad is shown. Workaround : set timescale 0 when showing ad.
#if UNITY_EDITOR
        if (Time.timeScale == 0)
            return;
#endif
        if (GameStarted != null)
            GameStarted();
    }

    /// <summary>
    /// Game over
    /// </summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        if (Time.timeScale == 0)
            return;
#endif

        if (GameOver != null)
            GameOver();
    }

    /// <summary>
    /// Opens the store pages
    /// </summary>
    public void RateGame()
    {
#if UNITY_ANDROID
        Application.OpenURL(playStoreURL);
#elif UNITY_IOS
        Application.OpenURL(appStoreURL);
#endif
    }

    /// <summary>
    /// Removes ad permanantly from the game.
    /// </summary>
    public void RemoveAds()
    {
        PlayerData.Instance.adsRemoved = true;
        PlayerData.Instance.SaveData();
        AdsManager.Instance.isAdRemoved = true;
        UIManager.Instance.removeAdBtn.SetActive(false);
    }


    /// <summary>
    ///For testing.  Editor Buttons available in the inspector.
    /// </summary>
#region TESTING

    /// <summary>
    /// Increases player cash
    /// </summary>
    public void IncreasePlayerCash()
    {
        float increaseBy = 100;
        PlayerData.Instance.cash += increaseBy;
        PlayerData.Instance.SaveData();
        Debug.Log("player cash increased by " + increaseBy);
    }

   /// <summary>
   /// Decreases the player cash.
   /// </summary>
    public void DecreasePlayerCash()
    {
        float decreaseBy = 100;
        PlayerData.Instance.cash -= decreaseBy;
        PlayerData.Instance.SaveData();
        Debug.Log("player cash decreased by " + decreaseBy);

    }

    /// <summary>
    /// Deletes and resets player data
    /// </summary>
    public void ClearPlayerData()
    {
        PlayerData.Clear();
        Debug.Log("player data cleared");

    }
#endregion

}
