using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GameController : MonoBehaviour
{
    public CoinManager coinManager;
    public int primeirolugar;
    public int gameOver;
    public Text coinsTextAtual;
    public Text coinsTextGrant;

    
    public GameObject player;
    private PlayerController pC;
    public GameObject pHitArea;
    public GameObject playerTarget;

    public GameObject enemy;
    private EnemyController eC;
    public GameObject eHitArea;
    public GameObject enemyTarget;

    public GameObject ball;
    private Rigidbody ballRB;

    public GameObject Fade;
    private Renderer fadeRenderer;
    private Color fadeColor;
    private bool fadeIn, fadeOut;
    private float fadeTime = 0f;

    public GameObject settingsCanvas;

    private int bounces;

    private bool inGame;
    private int serve; //1 = player, -1 = enemy;

    private int servingSide = 1; //-1 left, 1 Right

    public Text pS, pG, pP1, pP2;
    public Text eS, eG, eP1, eP2;

    public GameObject DisplayScore;
    public Text displayScore;
    public Text scoreMessage;

    public GameObject enemyServeMessage;

    public GameObject GameOver;
    public Text results;

    private BallController bC;

    public bool playerTurn;

    private string[] score;
    private string[] score2;
    private int playerScore;
    private int enemyScore;
    private int playerGameCount;
    private int enemyGameCount;
    private int playerSetCount;
    private int enemySetCount;

    public int points;
    public Text pointsText;
    private StoreDataContainer sD;

    public GameObject clickToPlay;
    public GameObject canvas;

    GameObject pauseCanvas;
    
    private float logTimer = 0f; // Timer for logging every 10 seconds

    void Start()
    {
        ball.SetActive(false);
        clickToPlay.SetActive(true);
        canvas.SetActive(false);
        GameOver.SetActive(false);
        fadeRenderer = Fade.GetComponent<Renderer>();
        fadeColor = fadeRenderer.material.color;
        fadeColor.a = 0;
        fadeRenderer.material.color = fadeColor;
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);
        Time.timeScale = 1;
        coinsTextAtual.text = coinManager.GetCoins().ToString();
    }

    public void Ready()
    {
        clickToPlay.SetActive(false);
        canvas.SetActive(true);

        ballRB = ball.GetComponent<Rigidbody>();

        DisplayScore.SetActive(false);

        fadeIn = false;
        fadeOut = false;

        pC = player.GetComponent<PlayerController>();
        eC = enemy.GetComponent<EnemyController>();
        bC = ball.GetComponent<BallController>();


        score = new string[] { "0", "1", "3", "4", "A", "0" };
        score2 = new string[] { "0", "5", "0", "0", "D", "0" };

        playerScore = 0;
        enemyScore = 0;
        playerGameCount = 0;
        enemyGameCount = 0;
        playerSetCount = 0;
        enemySetCount = 0;
        serve = 1;
        
        pS.text = playerSetCount + "";
        pG.text = playerGameCount + "";
        pP1.text = score[playerScore] + "";
        pP2.text = score2[playerScore] + "";
        eS.text = enemySetCount + "";
        eG.text = enemyGameCount + "";
        eP1.text = score[enemyScore] + "";
        eP2.text = score2[enemyScore] + "";

        points = 0;
        SetPoints();

        Debug.Log($"Ready. Serve = {serve} {servingSide}");
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        coinsTextAtual.text = coinManager.GetCoins().ToString();

        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
#if MOBILE_INPUT
        if (CrossPlatformInputManager.GetButtonDown("P"))
        {
            PauseGame();
        }
#endif
        if (fadeOut == true)
        {
            fadeTime += Time.deltaTime;
            if (fadeTime > 2 && fadeTime < 2.5)
            {
                fadeColor.a += 0.07f;
                fadeRenderer.material.color = fadeColor;
            }
            else if (fadeTime >= 2.5f)
            {
                DisplayScore.SetActive(false);
                fadeTime = 0;
                fadeOut = false;
                fadeIn = true;
                StartGame();
            }
        }
        if (fadeIn == true)
        {
            fadeTime += Time.deltaTime;
            if (fadeTime > 0.5 && fadeTime < 1)
            {
                fadeColor.a -= 0.075f;
                if (fadeColor.a < 0)
                {
                    fadeColor.a = 0;
                }
                fadeRenderer.material.color = fadeColor;
            }
            else if (fadeTime >= 1)
            {
                fadeTime = 0;
                enemyServeMessage.SetActive(true);
                fadeIn = false;
                if (serve == 1)
                {
                    pC.canServe = true;
                }
                else
                {
                    eC.canServe = true;
                }
            }
        }
        CheckBallBounds();
    }

    /// <summary>
    /// Checks if the ball is out of the designated play area and resets the round if necessary.
    /// </summary>
    private void CheckBallBounds()
    {
        if(bC.isServing == true && (ball.transform.position.y < 1)) {
            Debug.Log("Forcing Ball to serving position");
            SetBallToServingPosition();
        }

        logTimer += Time.deltaTime;
        if (logTimer >= 10f)
        {
            Debug.Log($"ball {ball.transform.position}");
            logTimer = 0f; // Reset the timer
        }
        //if (ball != null && ballRB != null)
        //{
        //    // Check if the ball is below ground level or outside the allowed z-axis range
        //    if (ball.transform.position.y < 0 || ball.transform.position.z < -20 || ball.transform.position.z > 20)
        //    {
        //        Debug.Log("Resetting round. Resetting ball position and scores.");
        //        // Logic to move to the next round
        //        StartGame();
        //    }
        //}
    }

    public void AddBounce()
    {
        bounces++;
        if (bounces == 3)
        {
            if (playerTurn == true)
            {
                SetScore(-1);
            }
            else
            {
                SetScore(1);
            }
        }
    }
    public void ResetBounce()
    {
        bounces = 0;
    }
    private void SetScore(int t)
    {
        if (inGame == true)
        {
            if (t == -1)
            {
                enemyScore++;
                scoreMessage.text = "Ponto Adversário";
                ShowPoints();
            }
            else if (t == 1)
            {
                playerScore++;
                scoreMessage.text = "Ponto Player";
                ShowPoints();
                coinManager.AddCoins(2);
            }
            if (playerScore == 4 && enemyScore < 3)
            {
                scoreMessage.text = "Game Player";
                GameWon();
                if (enemyScore == 0)
                {
                    PerfectGame();
                }
                playerGameCount++;
                ShowGameScore();
                enemyScore = 0;
                playerScore = 0;
                serve *= -1;

            }
            else if (enemyScore == 4 && playerScore < 3)
            {
                scoreMessage.text = "Game Adversário";
                enemyGameCount++;
                ShowGameScore();
                enemyScore = 0;
                playerScore = 0;
                serve *= -1;

            }
            else if (enemyScore == 4 && playerScore == 4)
            {
                scoreMessage.text = "Deuce";
                playerScore = 3;
                enemyScore = 3;
                ShowPoints();

            }
            else if (playerScore == 5)
            {
                scoreMessage.text = "Game Player";
                GameWon();
                playerGameCount++;
                ShowGameScore();
                enemyScore = 0;
                playerScore = 0;
                serve *= -1;

            }
            else if (enemyScore == 5)
            {
                scoreMessage.text = "Game Adversário";
                enemyGameCount++;
                ShowGameScore();
                enemyScore = 0;
                playerScore = 0;
                serve *= -1;

            }
            if (playerGameCount == 2)
            {
                scoreMessage.text = "Set Player";
                if (enemyGameCount == 0)
                {
                    PerfectSet();
                }
                playerSetCount++;
                ShowSetScore();
                playerGameCount = 0;
                enemyGameCount = 0;

            }
            else if (enemyGameCount == 2)
            {
                scoreMessage.text = "Set Adversário";
                enemySetCount++;
                ShowSetScore();
                playerGameCount = 0;
                enemyGameCount = 0;


            }
        }
        if (playerSetCount == 2)
        {
            MatchWon();
            if (enemySetCount == 0)
            {
                PerfectMatch();
            }
            EndGame("Marcos", "Adversário");
        }
        else if (enemySetCount == 2)
        {
            EndGame("Adversário", "Marcos");
        }
        else
        {
            DisplayScore.SetActive(true);
            inGame = false;
            pS.text = playerSetCount + "";
            pG.text = playerGameCount + "";
            pP1.text = score[playerScore] + "";
            pP2.text = score2[playerScore] + "";
            eS.text = enemySetCount + "";
            eG.text = enemyGameCount + "";
            eP1.text = score[enemyScore] + "";
            eP2.text = score2[enemyScore] + "";
            fadeOut = true;
        }
    }

    private void StartGame()
    {
        Debug.Log($"************ Starting new game. Ball and players are reset. Serve = {serve} {servingSide}");
        inGame = true;
        ball.SetActive(true); // Make sure the ball is active

        // Reset ball physics
        ballRB.velocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero; // Reset any rotation
        ballRB.useGravity = false;
        bC.isServing = true;

        // Set player and enemy initial positions and states
        SetInitialPositions();

        // Reset scores or game state as needed here
        
        Debug.Log($"Game Started Serve = {serve} {servingSide}, ball {ball.transform.position}");
    }

    private Vector3 GetBallToServingPosition() {
        return servingSide == 1 ? new Vector3(serve * 2.127f, 1.6f, -serve * 12.01f)
                                                : new Vector3(-serve * 1.87f, 1.6f, -serve * 12.01f);
    }

    private void SetBallToServingPosition() {
        ball.transform.position = GetBallToServingPosition(); 
    }

    private void SetInitialPositions()
    {
        // Set initial positions for player and enemy based on serving side
        Vector3 pStartPosition = new Vector3(2f * servingSide, 0.519f, -12.47f);
        player.transform.position = pStartPosition;
        player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        Vector3 eStartPosition = new Vector3(-servingSide * 2f, 0.519f, 12.47f);
        enemy.transform.position = eStartPosition;
        enemy.transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        Debug.Log($"Changing ball to serve {serve}. ball {ball.transform.position}");
        // Reset ball position based on the serving side
        SetBallToServingPosition();
        
        // Activate serving logic based on who is serving
        if (serve == 1)
        {
            pC.isServing = true;
            pHitArea.SetActive(false);
            playerTarget.transform.position = new Vector3(-servingSide * 2.5f, 0f, 3.5f);
            playerTurn = true;
        }
        else
        {
            eC.isServing = true;
            eHitArea.SetActive(false);
            enemyTarget.transform.position = new Vector3(servingSide * 2.5f, 0f, -3.5f);
            playerTurn = false;
        }

        StartCoroutine(UpdateBallPositionAfterDelay(0.3f));
    }

    
    IEnumerator UpdateBallPositionAfterDelay(float delayInSeconds)
    {
        Debug.Log($"Ball position before delay=  {ball.transform.position}");
        // Wait for the specified delay
        yield return new WaitForSeconds(delayInSeconds);

        // Code to update the ball position after the delay
        SetBallToServingPosition();
        Debug.Log($"Ball position updated after 100ms delay. Ball=  {ball.transform.position}");
        servingSide *=-1;
    }

    public void PlayerSideHit()
    {
        if (playerTurn == false)
        {
            SetScore(-1);
        }
        else
        {
            AddBounce();
        }
    }
    public void EnemySideHit()
    {
        if (playerTurn == true)
        {
            SetScore(1);
        }
        else
        {
            AddBounce();
        }
    }

    public void OutHit()
    {
        if (bounces >= 1)
        {
            AddBounce();
        }
        else
        {
            if (playerTurn == true)
            {
                SetScore(1);
            }
            else
            {
                SetScore(-1);
            }
        }
    }
    public void WallHit()
    {
        if (bounces >= 1)
        {
            if (playerTurn == true)
            {
                SetScore(-1);
            }
            else
            {
                SetScore(1);
            }
        }
    }

    private void ShowPoints()
    {
        if (serve == 1)
        {
            displayScore.text = score[playerScore] + score2[playerScore] + " - " + score[enemyScore] + score2[enemyScore];
        }
        else
        {
            displayScore.text = score[enemyScore] + score2[enemyScore] + " - " + score[playerScore] + score2[playerScore];
        }
        if (playerScore == 3 && enemyScore == 3)
        {
            scoreMessage.text = "Deuce";
        }
    }

    private void ShowGameScore()
    {
        displayScore.text = playerGameCount + " - " + enemyGameCount;
    }

    private void ShowSetScore()
    {
        displayScore.text = playerSetCount + " - " + enemySetCount;
    }

    public void BackToMenu()
    {
        Application.LoadLevel("PlayTennis");
    }

    private void EndGame(string winner, string loser)
    {
        inGame = false;
        GameOver.SetActive(true);
        //results.text = "RESULTADO\n1-"+winner+"\n2-"+loser+ "\nVocê ganhou "+points+" moedas";

        if (enemySetCount == 2)
        {
            results.text = "RESULTADO\n1-" + winner + "\n2-" + loser + "\nVocê ganhou" + gameOver + "moedas";
        }
        if (playerSetCount == 2)
        {
            results.text = "RESULTADO\n1-" + winner + "\n2-" + loser + "\nVocê ganhou" + primeirolugar + "moedas";
        }
    }

    void GameWon()
    {
        coinManager.AddCoins(100);
        /*
        points += 100;
        sD = StoreDataContainer.Load();
        sD.storeObjects[0].coin += 100;
        sD.Save();
        SetPoints();
        */
    }

    void MatchWon()
    {
        coinManager.AddCoins(10000);
        /*
        points += 10000;
        sD = StoreDataContainer.Load();
        sD.storeObjects[0].coin += 10000;
        sD.Save();
        SetPoints();
        */
    }

    void PerfectGame()
    {
        coinManager.AddCoins(1000);
        /*
        points += 1000;
        sD = StoreDataContainer.Load();
        sD.storeObjects[0].coin += 1000;
        sD.Save();
        SetPoints();
        */
    }

    void PerfectSet()
    {
        coinManager.AddCoins(3000);
        /*
        points += 3000;
        sD = StoreDataContainer.Load();
        sD.storeObjects[0].coin += 3000;
        sD.Save();
        SetPoints();
        */
    }

    void PerfectMatch()
    {
        coinManager.AddCoins(5000);
        /*
        points += 5000;
        sD = StoreDataContainer.Load();
        sD.storeObjects[0].coin += 5000;
        sD.Save();
        */
    }

    void SetPoints()
    {
        pointsText.text = "" + points;
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            pauseCanvas.SetActive(true);
            canvas.SetActive(false);

        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseCanvas.SetActive(false);
            settingsCanvas.SetActive(false);
            canvas.SetActive(true);
        }

    }
}