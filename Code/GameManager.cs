using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Script`s")]
    public EnemyBola enemyBola;
    public EnemyAviao enemyAviao;
    public Elevador elevador;
    public Lula lula;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip deadFX;
    public AudioClip jumpFX;
    public AudioClip runFX;
    public AudioClip itemFX;

    [Header("Componentes")]
    [SerializeField] private GameObject[] enemyBalls;
    [SerializeField] private GameObject[] enemyAviaos;
    [SerializeField] private TextMeshProUGUI score_txt;
    public Transform[] teleports;
    public Transform[] portaisSaida;
    public GameObject[] lifes;
    public Elevador[] elevadores;
    public GameObject[] degrauEscadarolante;
    
    //public List<GameObject> lifes = new List<GameObject>();
    public List<GameObject> itens = new List<GameObject>();

    [Header("Variaveis")]
    public int Score;
    private bool enemyActive = false;
    public Elevador elevadorAtual;
    public Transform degrauEscadaRolanteAtual;

    void Start()
    {
        StartCoroutine(EnemyLoop()); // inicia o ciclo dos inimigos
        Score = 0;
        UpdateScore();
    }


    IEnumerator EnemyLoop()
    {
        while (true)
        {
            if (!enemyActive) // só chama quando não tem inimigo ativo
            {
                enemyBallAction();
                enemyAviaoAction();
                enemyActive = true;
            }

            yield return null;
        }
    }
 
    void enemyBallAction()
    {
        // sorteia índice entre 0 e o tamanho do array
        int randomIndex = Random.Range(0, enemyBalls.Length);

        GameObject chosenEnemy = enemyBalls[randomIndex];
        chosenEnemy.SetActive(true);

        EnemyBola enemy = chosenEnemy.GetComponent<EnemyBola>();
        enemy.StartEnemy();

        // inscreve evento de reset quando ele sumir
        enemy.OnEnemyHidden += EnemyRespawned;

    }
    void enemyAviaoAction()
    {
        // sorteia índice entre 0 e o tamanho do array
        int randomIndex = Random.Range(0, enemyAviaos.Length);

        GameObject chosenEnemy = enemyAviaos[randomIndex];
        chosenEnemy.SetActive(true);

        EnemyAviao enemy = chosenEnemy.GetComponent<EnemyAviao>();
        enemy.StartEnemy();

        // inscreve evento de reset quando ele sumir
        enemyAviao.OnEnemyHiddenAviao += EnemyAviaoRespawned;

    }

    void EnemyRespawned(EnemyBola enemy)
    {
        enemy.OnEnemyHidden -= EnemyRespawned; // remove a inscrição
        enemyActive = false; // libera pra sortear outro
    }
    void EnemyAviaoRespawned(EnemyAviao enemyAviao)
    {
        enemyAviao.OnEnemyHiddenAviao -= EnemyAviaoRespawned; // remove a inscrição
        enemyActive = false; // libera pra sortear outro
    }

    public Elevador elevadoresAction()
    {
        int randomIndex;

        // garante que não sorteia o mesmo elevador
        do
        {
            randomIndex = Random.Range(0, elevadores.Length);
        }
        while (elevadores[randomIndex] == elevadorAtual);

        elevadorAtual = elevadores[randomIndex];

        return elevadorAtual;
    }

   public void AddScore(int points)
    {
        Score += points;
        UpdateScore();
    }
    
    void UpdateScore()
    {
        score_txt.text = Score.ToString();
    }
   
}
