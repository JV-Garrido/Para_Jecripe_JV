using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallPhysics : MonoBehaviour
{
    private static Rigidbody b;
    private bool serveFlag;
    private bool autoServe;
    private bool p1Flag;
    private bool p2Flag;
    private bool pointScored;
    private bool firstPing;

    private int checkPing;
    private int bateRebateMult;
    private float speedDiv;
    private float heightOffset;
    private float firstLaunchMult;
    private float firstLaunchYMult;
    private float rebateYMult;
    private float gravity;
    private float cinematicGravity;
    private float gameplayGravity;
    private float spX, spY, spZ;
    private float shortWait;
    private float shorterWait;
    private float horiOffset;
    private float horiMult;

    private List<string> allRegions;
    private List<string> exceptPlayer;
    private List<string> exceptEnemy;
    private List<string> exceptWall;

    private UnityEngine.Vector3 startingPoint;
    private UnityEngine.Vector3 ballTargetDis;
    private UnityEngine.Vector3 adjustPos;
    private PvpPlayerController player1C;
    private PvpPlayerController player2C;
    private PvPGameController gameControl;
    private TennisSounds TennisSounds;

    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<Rigidbody>();
        serveFlag = true;       // Indica se está na fase de servir. Caso positivo, a gravidade é desativada para simular o avatar segurando a bolinha
        autoServe = true;       // Usado na fase de servir. Ao iniciar o serveAnim de um Player, se torna false e impede chamadas duplicatas do LaunchBallServe
        p1Flag = false;
        p2Flag = false;
        pointScored = false;
        firstPing = false;
        checkPing = 0;
        speedDiv = 1.8f;
        firstLaunchMult = 1.2f;
        firstLaunchYMult = 0.7f;
        rebateYMult = 0.6f;
        bateRebateMult = 60;
        heightOffset = 4f;
        spX = 2.534f;
        spY = 1.749f;
        spZ = -12.393f;
        shortWait = 0.9f;
        shorterWait = 0.7f;
        cinematicGravity = 270f;
        gameplayGravity = 135f;
        horiOffset = 0f;
        horiMult = 40f;
        gravity = cinematicGravity;

        allRegions = new List<string> { "PlayerSide", "EnemySide", "Out", "Wall", "Outerworld" };
        exceptPlayer = new List<string> { "EnemySide", "Out", "Wall", "Outerworld" };
        exceptEnemy = new List<string> { "PlayerSide", "Out", "Wall", "Outerworld" };
        exceptWall = new List<string> { "PlayerSide", "EnemySide", "Out" };

        startingPoint = new UnityEngine.Vector3(spX, spY, spZ);

        player1C = GameObject.Find("player1").GetComponent<PvpPlayerController>();
        player2C = GameObject.Find("player2").GetComponent<PvpPlayerController>();
        gameControl = GameObject.Find("Game Controller").GetComponent<PvPGameController>();
        TennisSounds = GameObject.Find("Sounds").GetComponent<TennisSounds>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!serveFlag)
            BallGravity();
        else
            StartServe();
    }

    void RebatendoFis(bool playerP, float hori)
    {
        if (!firstPing && checkPing == 0)
        {
            Pontuar(!playerP);
            gameControl.ShowServeWarning();
        }
        RebatidaFisica(playerP, hori);
    }

    void CheckPingZero(Collider c, string curSide, List<string> curExcept, bool playerP)
    {
        if (c.gameObject.CompareTag(curSide))
        {
            checkPing += 1;
            firstPing = true;
        }
        else if (curExcept.Contains(c.gameObject.tag))
        {
            Pontuar(playerP);
        }
    }

    void CheckPingOne(Collider c, bool playerP)
    {
        if (exceptWall.Contains(c.gameObject.tag))
        {
            checkPing += 1;
        }
        else if (c.gameObject.CompareTag("Wall") || c.gameObject.CompareTag("Outerworld"))
        {
            Pontuar(!playerP);
        }
    }

    void CheckPingTwo(Collider c, bool playerP)
    {
        if (allRegions.Contains(c.gameObject.tag))
        {
            checkPing += 1;
            Pontuar(!playerP);
        }
    }

    void PingRebateControl( Collider c,
                            string hitTag,
                            PvpPlayerController curControl,
                            bool isP1, 
                            string curSide,
                            List<string> curExcept,
                            float horiDirection )
    {
        if (c.gameObject.CompareTag(hitTag) && (curControl.rebatendo || curControl.serveHeld))
        {
            curControl.RebatePrep();
            RebatendoFis(isP1, horiDirection);
        }
        else if (checkPing == 0)
        {
            CheckPingZero(c, curSide, curExcept, isP1);
        }
        else if (checkPing == 1)
        {
            CheckPingOne(c, isP1);
        }
        else if (checkPing == 2)
        {
            CheckPingTwo(c, isP1);
        }
    }

    // Ativada ao colidir com algo
    // Um jogador ter o turno significa que esse um jogador deve rebater a bolinha para continuar o ponto
    // O turno é indicado pelos p1Flag e p2Flag
    void OnTriggerEnter(Collider c)
    {
        if (!autoServe && !pointScored)
        {
            // Direction from the incoming object's center toward this hitbox center
            UnityEngine.Vector3 direction = transform.position - c.transform.position;
            Ray ray = new Ray(c.transform.position, direction.normalized);

            horiOffset = 0;
            // Raycast against this specific hitbox collider to find the intersection point
            if (GetComponent<Collider>().Raycast(ray, out RaycastHit hit, direction.magnitude * 2f))
            {
                UnityEngine.Vector3 hitPoint = hit.point;
                UnityEngine.Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);

                string directionName;
                if (localHitPoint.x > 0){
                    directionName = "Direita";
                }else
                {
                    directionName = "Esquerda";
                }
                Debug.Log($"Trigger hit point: {localHitPoint}");
                Debug.Log($"Bola vai mais pra {directionName}");
                horiOffset = localHitPoint.x * -1;
            }
            // Turno P1
            if (p1Flag)
            {
                PingRebateControl(c, "hitP1", player1C, true, "PlayerSide", exceptPlayer, horiOffset);
            }
            // Turno P2
            if (p2Flag)
            {
                PingRebateControl(c, "hitP2", player2C, false, "EnemySide", exceptEnemy, horiOffset);
            }
        }
    }

    // Gravidade da bola
    void BallGravity()
    {
        b.AddForce(-transform.up * gravity * Time.deltaTime, ForceMode.Acceleration);
    }

    // Serve inicial
    // Fase 1
    void StartServe()
    {
        if (player1C.isServer)
        {
            if (player1C.serveAnim && autoServe)
            {
                Invoke(nameof(LaunchBallServe), shorterWait);
                autoServe = false;
            }
        }
        else if (player2C.isServer && player2C.serveAnim && autoServe)
        {
            Invoke(nameof(LaunchBallServe), shorterWait);
            autoServe = false;
        }
        // else if (player2C.isServer)
        // {
        //     if (player2C.serveAnim && autoServe)
        //     {
        //         Invoke(nameof(LaunchBallServe), 0.7f);
        //         autoServe = false;
        //     }
        // }
    }

    // Serve Inicial
    // Fase 2
    void LaunchBallServe()
    {
        b.velocity = UnityEngine.Vector3.zero;
        b.angularVelocity = UnityEngine.Vector3.zero;
        b.velocity = new UnityEngine.Vector3(0f, 3.336f, 0f);
        // Ponto de física
        serveFlag = false;
        Invoke(nameof(FirstLaunchBall), shortWait);
    }

    // Serve Inicial
    // Fase 3
    // Fase Final
    void FirstLaunchBall()
    {
        // Saque Rebate
        ballTargetDis = new UnityEngine.Vector3(
            (-b.transform.position.x) * firstLaunchMult / speedDiv,
            (heightOffset - b.transform.position.y) * firstLaunchYMult,
            (-b.transform.position.z) * firstLaunchMult / speedDiv    );
        if (player1C.isServer)
        {
            ballTargetDis.x *= player1C.saquePower;
            ballTargetDis.z *= player1C.saquePower; 
            player1C.saquePower = 0f;
        }
        else
        {
            ballTargetDis.x *= player2C.saquePower;
            ballTargetDis.z *= player2C.saquePower; 
            player2C.saquePower = 0f;
        }
        b.velocity = bateRebateMult * Time.deltaTime * ballTargetDis;
        // Ponto de física
        gravity = gameplayGravity;
        ChangeFlag();
    }

    // Física da rebatida
    // jogador true é P1
    public void RebatidaFisica(bool jogador, float hori)
    {
        // Bola reposição
        float sideZ = 11.345f;
        if (jogador) {sideZ *= -1;}
        adjustPos = new UnityEngine.Vector3(transform.position.x, 1.75f, sideZ);
        b.MovePosition(adjustPos);
        // Saque Rebate
        ballTargetDis = new UnityEngine.Vector3(
            // (-b.transform.position.x) / speedDiv,
            hori*horiMult / speedDiv,
            (heightOffset - b.transform.position.y) * rebateYMult,
            (-b.transform.position.z) / speedDiv);
        Debug.Log("Original ballTargetDis: " + ballTargetDis);
        if (jogador)
        {
            ballTargetDis.x *= player1C.lastPower;
            ballTargetDis.z *= player1C.lastPower;  
        }
        else
        {
            ballTargetDis.x *= player2C.lastPower;
            ballTargetDis.z *= player2C.lastPower;  
        }
        b.velocity = bateRebateMult * Time.deltaTime * ballTargetDis;
        Debug.Log("Velocidade: " + b.velocity + ". ballTargetDis: " + ballTargetDis);
        // Ponto de física
        ChangeFlag();
        checkPing = 0;
    }

    public void RebatidaFisicaProto(bool jogador, float hori)
    {
        // Mudar posição da bolinha, para altura fixa e distância de eixo Z do avatar fixa (pra ambos podemos definir constantes)
        // Definir X com base no hori
        // Com base no lastPower, definimos Z e Y, e os seguintes:
        //  lastPower < 0,9f = Fraco -> Z baixo, Y médio e gravidade abaixo do padrão
        //  lastPower < 1,2f = Médio -> Z regular, Y elevado e gravidade normal
        //  else = Forte -> Z alto, Y superior e gravidade pesada
        
        // Ponto de física
        ChangeFlag();
        checkPing = 0;
    }

    // Atribui ações de pontuação
    // jogador true é ponto do P1
    public void Pontuar(bool jogador)
    {
        if (!pointScored) {PontuarTrue(jogador);}
    }

    public void PontuarTrue(bool jogador)
    {
        TennisSounds.PlaySound(TennisSounds.applause);
        pointScored = true;
        gameControl.Scored(jogador); 
    }

    // Muda turno
    // Usada quando um jogador rebate ou faz um saque
    public void ChangeFlag()
    {
        player1C.turnFlag = !player1C.turnFlag;
        player2C.turnFlag = !player2C.turnFlag;
        if (p1Flag == p2Flag)
        {
            p1Flag = player1C.turnFlag;
            p2Flag = player2C.turnFlag;
        }
        else
        {
            p1Flag = !p1Flag;
            p2Flag = !p2Flag;
        }
        player1C.serveAnim = true;
        player2C.serveAnim = true;
        TennisSounds.PlaySound(TennisSounds.ball);
        TennisSounds.PlaySound(TennisSounds.playerMoan);
        TennisSounds.PlaySound(TennisSounds.racket);
        Debug.Log("Changed Flag");
    }

    // Parte de NewPoint do PvPGameController
    // Acionada apenas pelo script PvPGameController
    public void NewPoint()
    {
        Debug.Log("Bola voltando pra posição");
        if (startingPoint.x > 0) { startingPoint.x -= 4f; }
        else { startingPoint.x += 4f; }
        b.MovePosition(startingPoint);
        b.isKinematic = true;
        b.angularVelocity = UnityEngine.Vector3.zero;
        serveFlag = true;
        p1Flag = false;
        p2Flag = false;
        autoServe = true;
        checkPing = 0;
        pointScored = false;
        gravity = cinematicGravity;
        b.isKinematic = false;
        firstPing = false;
    }

    // Parte de SwitchCourt do PvPGameController
    // Acionada apenas pelo script PvPGameController
    // Muda o lado que a bolinha nasce ao ter um game marcado
    public void NewGame()
    {
        startingPoint.x *= -1f;
        startingPoint.z *= -1f;
    }
}