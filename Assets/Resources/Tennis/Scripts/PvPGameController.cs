using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPGameController : MonoBehaviour
{

    public GameObject clickToPlay;
    public GameObject settingsCanvas;
    public GameObject canvas;
    public GameObject serveMessage;
    public GameObject DisplayScore;
    public GameObject GameOver;
    public GameObject ServeWarning;

    GameObject pauseCanvas;
    private bool gameStarted;
    private PvpPlayerController player1C;
    private PvpPlayerController player2C;
    public BallPhysics ball;

    public UnityEngine.UI.Text pS, pG, pP1, pP2;
    public UnityEngine.UI.Text eS, eG, eP1, eP2;
    private int playerScore;
    private int enemyScore;
    private int playerGameCount;
    private int enemyGameCount;
    private int playerSetCount;
    private int enemySetCount;

    private string[] score;
    private string[] score2;
    private bool serve; // true == p1; false == p2
    public UnityEngine.UI.Text displayScore;
    public UnityEngine.UI.Text scoreMessage;
    public UnityEngine.UI.Text results;

    // Start is called before the first frame update
    void Start()
    {
        clickToPlay.SetActive(true);
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);
        canvas.SetActive(false);
        GameOver.SetActive(false);
        gameStarted = false;
        serveMessage.SetActive(false);
        Time.timeScale = 0;
        player1C = GameObject.Find("player1").GetComponent<PvpPlayerController>();
        player2C = GameObject.Find("player2").GetComponent<PvpPlayerController>();
        ball = GameObject.Find("ballpvp").GetComponent<BallPhysics>();
        score = new string[] { "0", "1", "3", "4", "A", "0" };
        score2 = new string[] { "0", "5", "0", "0", "D", "0" };

        playerScore = 0;
        enemyScore = 0;
        playerGameCount = 0;
        enemyGameCount = 0;
        playerSetCount = 0;
        enemySetCount = 0;
        
        pS.text = playerSetCount + "";
        pG.text = playerGameCount + "";
        pP1.text = score[playerScore] + "";
        pP2.text = score2[playerScore] + "";
        eS.text = enemySetCount + "";
        eG.text = enemyGameCount + "";
        eP1.text = score[enemyScore] + "";
        eP2.text = score2[enemyScore] + "";

        serve = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {

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
        }

    }

    // Usada pelo ClickToPlayCanvas > Background > ClickToPlay.OnClick()
    // É o que o jogo faz começar
    public void Ready()
    {
        clickToPlay.SetActive(false);
        canvas.SetActive(true);
        DisplayScore.SetActive(false);
        ServeWarning.SetActive(false);
        gameStarted = true;
        Time.timeScale = 1;
    }

    // Usada para pausar e despausar o jogo
    public void PauseGame()
    {

        Time.timeScale = 1 - Time.timeScale;
        if (Time.timeScale == 0)
        {
            pauseCanvas.SetActive(true);
            canvas.SetActive(false);

        }
        else if (Time.timeScale == 1)
        {
            pauseCanvas.SetActive(false);
            settingsCanvas.SetActive(false);
            canvas.SetActive(true);
        }

    }

    // Usada para registrar um ponto. Se ps for true, é do player 1, senão é do player 2
    public void Scored(bool ps)
    {
        gameStarted = false;
        player1C.timeOut = true;
        player2C.timeOut = true;
        Debug.Log("Resetar jogo em alguns segundos");
        // Considerar mover para funções diferentes os casos em que o jogador pontua (PlayerScored) e quando o adversário pontua (EnemyScored)
        if (ps)
        {
            playerScore++;
            if (playerScore >= 4)
            {
                if (enemyScore < 3)
                {
                    scoreMessage.text = "Game Player 1";
                    playerScore = 0;
                    enemyScore = 0;
                    playerGameCount++;
                    ShowGSScore(playerGameCount, enemyGameCount);
                    SwitchCourt();
                }
                else if (enemyScore == 3)
                {
                    if (playerScore == 5)
                    {
                        scoreMessage.text = "Game Player 1";
                        playerScore = 0;
                        enemyScore = 0;
                        playerGameCount++;
                        ShowGSScore(playerGameCount, enemyGameCount);
                        SwitchCourt();
                    }
                    else
                    {
                        scoreMessage.text = "Vantagem Player 1";
                        ShowPoints();
                    }
                }
                else
                if (enemyScore == 4)
                {
                    // Deuce
                    playerScore = 3;
                    enemyScore = 3;
                    ShowPoints();
                }
            }
            else
            {
                scoreMessage.text = "Ponto Player 1";
                ShowPoints();
            }
            if (playerGameCount == 2)
            {
                scoreMessage.text = "Set Player 1";
                playerSetCount++;
                ShowGSScore(playerSetCount, enemySetCount);
                playerGameCount = 0;
                enemyGameCount = 0;
            }
        }
        else
        {
            enemyScore++;
            if (enemyScore >= 4)
            {
                if (playerScore < 3)
                {
                    scoreMessage.text = "Game Player 2";
                    playerScore = 0;
                    enemyScore = 0;
                    enemyGameCount++;
                    ShowGSScore(playerGameCount, enemyGameCount);
                    SwitchCourt();
                }
                else if (playerScore == 3)
                {
                    scoreMessage.text = "Vantagem Player 2";
                    
                    if (enemyScore == 5)
                    {
                        scoreMessage.text = "Game Player 2";
                        playerScore = 0;
                        enemyScore = 0;
                        enemyGameCount++;
                        ShowGSScore(playerGameCount, enemyGameCount);
                        SwitchCourt();
                    }
                    else
                    {
                        ShowPoints();
                    }
                }
                else if (playerScore == 4)
                {
                    // Deuce
                    playerScore = 3;
                    enemyScore = 3;
                    ShowPoints();
                }
            }
            else
            {
                scoreMessage.text = "Ponto Player 2";
                ShowPoints();
            }
            if (enemyGameCount == 2)
            {
                scoreMessage.text = "Set Player 2";
                enemySetCount++;
                ShowGSScore(playerSetCount, enemySetCount);
                playerGameCount = 0;
                enemyGameCount = 0;
            }
        }
        if (playerSetCount == 2)
        {
            EndGame("Player 1", "Player 2");
        }
        else
        if (enemySetCount == 2)
        {
            EndGame("Player 2", "Player 1");
        }
        else
        {
            Invoke(nameof(NewPoint), 5.0f);
            DisplayScore.SetActive(true);
            pS.text = playerSetCount + "";
            pG.text = playerGameCount + "";
            pP1.text = score[playerScore] + "";
            pP2.text = score2[playerScore] + "";
            eS.text = enemySetCount + "";
            eG.text = enemyGameCount + "";
            eP1.text = score[enemyScore] + "";
            eP2.text = score2[enemyScore] + "";
        }
    }

    public void ShowServeWarning()
    {
        ServeWarning.SetActive(true);
    }

    // Parte de Scored
    // Chama funções de outros scripts para começar um novo ponto
    private void NewPoint()
    {
        player1C.NewPoint(serve);
        player2C.NewPoint(serve);
        ball.NewPoint();
        DisplayScore.SetActive(false);
        ServeWarning.SetActive(false);
        gameStarted = true;
    }

    // Parte de Scored
    // Mostra a pontuação atualizada
    private void ShowPoints()
    {
        if (serve)
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

    private void ShowGSScore(int p1Count, int p2Count)
    {
        displayScore.text = p1Count + " - " + p2Count;
    }

    // Parte de Scored
    // Finaliza o jogo e mostra a situação final
    private void EndGame(string winner, string loser)
    {
        GameOver.SetActive(true);
        //results.text = "RESULTADO\n1-"+winner+"\n2-"+loser+ "\nVocê ganhou "+points+" moedas";

        results.text = "RESULTADO\n1-" + winner + "\n2-" + loser;
    }

    // Parte de Scored
    // Troca de quadras ao marcar um Game
    // Na verdade não há troca de quadra entre avatares
    // Apenas a bolinha é teleportada pro outro lado da quadra
    // Sendo sacada pelo outro jogador
    public void SwitchCourt()
    {
        serve = !serve;
        ball.NewGame();
        player1C.isServer = !player1C.isServer;
        player2C.isServer = !player2C.isServer;
    }
}
