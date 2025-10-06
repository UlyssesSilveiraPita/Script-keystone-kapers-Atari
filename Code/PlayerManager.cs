using System.Collections;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    [Header("Script`s")]
    [SerializeField] private GameManager GameManager;

    [Header("Components")]
    [SerializeField] private Rigidbody2D player_Rb;
    [SerializeField] private CapsuleCollider2D player_capsule_Collider;
    [SerializeField] private Animator player_Animator;
    [SerializeField] private SpriteRenderer player_SpriteRenderer;  


    [Header("Variaveis")]
    [SerializeField] private float player_Speed; // velocidade do jogador
    [SerializeField] private float player_JumpForce; // forca do pulo
    [SerializeField] private bool isWalking; // esta andando
    [SerializeField] private bool isLookLeft; // olhando para esqueda
    [SerializeField] private bool isJump; // pulo
    [SerializeField] private bool isCrouch; /// abaixado
    [SerializeField] private bool isdentroDoElevador;
    [SerializeField] private bool ispossoEntrarElevador;
    [SerializeField] private bool isPossoUsarEscadaRolante;
    [SerializeField] private bool isEstouUsarEscadaRolante;

    private Transform degrauAtual;
    private Vector3 startPosition;

    void Start()
    {
        GameManager = FindAnyObjectByType<GameManager>();
        startPosition = transform.position; // salva posição inicial

    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
        animationsManager();

    }


    void InputManager()
    {
       

        if (!isdentroDoElevador && !isEstouUsarEscadaRolante)
        {
            //Movimentacao do Personagem//

            float horizontal = Input.GetAxisRaw("Horizontal");
            isWalking = horizontal != 0; // variavel bolena e true quando valor horizontal e difernte de 0
            player_Rb.linearVelocity = new Vector2(horizontal * player_Speed, player_Rb.linearVelocity.y);

            //Pulo do personagem//

            if (Input.GetKeyDown(KeyCode.Space) && !isJump)
            {
                playerJump();
            }

            if (Input.GetKeyDown(KeyCode.S) && !isWalking)
            {
                player_Rb.linearVelocity = Vector2.zero;
                player_Speed = 0;
                isCrouch = true;
            }

            if (Input.GetKeyDown(KeyCode.W) 
                && ispossoEntrarElevador == true 
                && GameManager.elevadorAtual != null 
                && GameManager.elevadorAtual.isOpen == true)
                {
                    StartCoroutine(IEdentroElevador());
                }

            else if(Input.GetKeyDown(KeyCode.W) && isPossoUsarEscadaRolante)
            {
                isEstouUsarEscadaRolante = true;
                StartCoroutine(IEUsandoEscadaRolante(GameManager.degrauEscadaRolanteAtual));

            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                isCrouch = false;
                player_Speed = 4;
            }

            // flip do personagem
            if (horizontal > 0 && isLookLeft == true)
            {
                flip();
            }
            else if (horizontal < 0 && isLookLeft == false)
            {
                flip();
            }

        }


        if (GameManager.elevadorAtual == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S) && isdentroDoElevador == true && GameManager.elevadorAtual.isOpen == true)
        {
            isdentroDoElevador = false;

            transform.position = GameManager.elevadorAtual.point_ForaElevador.position;
            player_Rb.gravityScale = 1;
            player_SpriteRenderer.sortingOrder = 2;
            player_capsule_Collider.enabled = true;

            GameManager.elevadorAtual = null;   
        }

    }

    void playerJump()
    {
        isJump = true;
        player_Rb.AddForce(Vector2.up * player_JumpForce, ForceMode2D.Impulse);
        GameManager.audioSource.PlayOneShot(GameManager.jumpFX);
    }

    void animationsManager()
    {
        player_Animator.SetBool("isWalking", isWalking);
        player_Animator.SetBool("isJump", isJump);
        player_Animator.SetBool("isCrouch", isCrouch);
    }

    void flip() // metodo de flip
    {
        isLookLeft = !isLookLeft; // boleano recebe valor invertido
        Vector3 scale = transform.localScale; // variavel armazena o valor do localScale
        scale.x *= -1; // variavel recebe multiplicacao -1 para trocar o sinal do eixo x
        transform.localScale = scale; // devolvendo o valor trocado para o localScale
    }

    private void OnCollisionEnter2D(Collision2D col) 
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isJump = false;
        }
        if (col.gameObject.CompareTag("Enemy")) // inimigo colidiu
        {
            for (int i = GameManager.lifes.Length -1 ; i >=0; --i)
            {

                if (GameManager.lifes[i].activeSelf)
                {
                    // desativa UMA vida e sai do loop
                    GameManager.lifes[i].SetActive(false); 
                    break;
                }

            }

            GameManager.enemyBola.transform.position = GameManager.enemyBola.StartPosition;
            GameManager.enemyAviao.transform.position = GameManager.enemyAviao.StartPosition;
            transform.position = startPosition; // volta para origem
            player_Rb.linearVelocity = Vector2.zero; // zera velocidade
            GameManager.audioSource.PlayOneShot(GameManager.deadFX);

            // verifica se acabou todas as vidas
            bool acabouAsVidas = true;

            foreach (GameObject vida in GameManager.lifes)
            {
                if (vida.activeSelf) acabouAsVidas = false;
            }

            if (acabouAsVidas)
            {
                GameManager.Score = 0; // zera pontuação
                GameManager.AddScore(0);
                
                // Reativa todas as vidas depois de um pequeno tempo
                StartCoroutine(ReativarVidas());
            }

        }
        if (col.gameObject.CompareTag("Lula"))
        {

            StartCoroutine(IEPegouLadrao());
        }

       
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case ("Itens"):
                col.gameObject.SetActive(false);    // desativa ou destrói o item
                GameManager.itens.Remove(col.gameObject);   // remove da lista
                GameManager.AddScore(100);
                GameManager.audioSource.PlayOneShot(GameManager.itemFX);
                break;

            case ("EntradaElevador"):
                ispossoEntrarElevador = true;
                GameManager.elevadorAtual = GameManager.elevadores[0];
                break;

            case ("EntradaElevador2"):
                ispossoEntrarElevador = true;
                GameManager.elevadorAtual = GameManager.elevadores[1];
                break;

            case ("EntradaElevador3"):
                ispossoEntrarElevador = true;
                GameManager.elevadorAtual = GameManager.elevadores[2];
                break;

            case ("PossoUsarEscadaRolante"):
                isPossoUsarEscadaRolante = true;
                break;

            case ("DegrauPlataforma"):
                degrauAtual = col.transform; // guarda referência do degrau
                break;

            case ("SaidaEscadaRolante"):
                isEstouUsarEscadaRolante = false;
                isEstouUsarEscadaRolante = false; // isso vai parar a coroutine
                isPossoUsarEscadaRolante = false;
                GameManager.degrauEscadaRolanteAtual = null;
                degrauAtual = null; // solta degrau
                break;
        }
        


    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("EntradaElevador") ||
        col.CompareTag("EntradaElevador2") ||
        col.CompareTag("EntradaElevador3"))
        {
            ispossoEntrarElevador = false;
            GameManager.elevadorAtual = null;
        }
        if(col.CompareTag("PossoUsarEscadaRolante"))
        {
            isPossoUsarEscadaRolante = false;

        }
        if(col.CompareTag("SaidaEscadaRolante"))
        {
            isEstouUsarEscadaRolante = false; // isso vai parar a coroutine
            isPossoUsarEscadaRolante = false;
            GameManager.degrauEscadaRolanteAtual = null;
        }
        if (col.CompareTag("DegrauPlataforma"))
        {
            GameManager.degrauEscadaRolanteAtual = null; // saiu do degrau atual
        }

    }

    IEnumerator IEdentroElevador()
    {
        isdentroDoElevador = true;

        transform.position = GameManager.elevadorAtual.point_DentroElevador.transform.position;
        player_Rb.gravityScale = 0;
        player_SpriteRenderer.sortingOrder = -1;
        player_capsule_Collider.enabled = false;

        // espera a porta fechar
        yield return new WaitUntil(() => GameManager.elevador.isOpen == false);


        // sorteia outro elevador
        Elevador novo = GameManager.elevadoresAction();

        // teleporta para dentro do novo
        transform.position = novo.point_DentroElevador.position;

        // espera a porta abrir
        yield return new WaitUntil(() => GameManager.elevador.isOpen == true);

       // yield return null;  
    }
    IEnumerator IEUsandoEscadaRolante(Transform degrau)
    {
        isEstouUsarEscadaRolante = true;
        player_Rb.gravityScale = 0;
        player_capsule_Collider.enabled = false;
        player_SpriteRenderer.sortingOrder = -1;

        float tempoMaximo = 4.5f; // tempo que o jogador ficará sendo carregado
        float tempoAtual = 0f;

        while (tempoAtual < tempoMaximo && degrauAtual != null)
        {
            // acompanha o degrau
            transform.position = new Vector3(
            degrauAtual.position.x,
            degrauAtual.position.y,
            transform.position.z
        );

            tempoAtual += Time.deltaTime;
            yield return null; // espera próximo frame

        }

        // quando sai da escada
        player_Rb.gravityScale = 1;
        player_capsule_Collider.enabled = true;
        player_SpriteRenderer.sortingOrder = 2; // volta pro normal
        isEstouUsarEscadaRolante = false;


    }
    private IEnumerator ReativarVidas()
    {
        yield return new WaitForSeconds(2f); // espera 2 segundos antes de reviver

        foreach (GameObject vida in GameManager.lifes)
        {
            vida.SetActive(true); // liga novamente todos os ícones de vida
        }

    }

    private IEnumerator IEPegouLadrao()
    {
        GameManager.audioSource.PlayOneShot(GameManager.itemFX);
        player_Speed = 0;


        yield return new WaitForSeconds(1f);

        isdentroDoElevador = true;
        transform.position = startPosition; // volta para origem
        GameManager.AddScore(1000);

        yield return new WaitForSeconds(2f);

        float horizontal = Input.GetAxisRaw("Horizontal");
        isWalking = horizontal == 0; // variavel bolena e true quando valor horizontal e difernte de 0
        player_Rb.linearVelocity = new Vector2(horizontal * player_Speed, player_Rb.linearVelocity.y);

        isdentroDoElevador = false;
        isdentroDoElevador = false;
        player_Speed = 4;
    }


    IEnumerator IRrun()
    {
        GameManager.audioSource.PlayOneShot(GameManager.runFX);
        yield return null;
    }
}
