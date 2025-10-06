using System.Collections;
using UnityEngine;

public class DegrauEscadaRolante : MonoBehaviour
{
    [SerializeField] private Transform pontoInicio;
    [SerializeField] private Transform pontoFinal;
    [SerializeField] private Transform pontoNascimento;

    [SerializeField] private float speedDegrauEscada;

    public bool isChegueinoFinal;

    void Start()
    {
        // começa no ponto inicial
        transform.position = pontoNascimento.position;
        StartCoroutine(LoopDegrauEscadaRolante());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoopDegrauEscadaRolante()
    {
        while (true)
        {
            // vai do início até o final
            yield return StartCoroutine(moverDegrauEscadaRolante(pontoFinal.position));
            isChegueinoFinal = true;

            // teleporta de volta para o início
            transform.position = pontoInicio.position;
            isChegueinoFinal = false;

            // espera 1 frame pra não dar conflito
            yield return null;

        }
    }

    IEnumerator moverDegrauEscadaRolante(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, speedDegrauEscada * Time.deltaTime);
            yield return null;
        }
    }
}
