using System.Collections;
using UnityEngine;

public class Elevador : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] Transform point_Aberto;
    [SerializeField] Transform point_Fechado;
    public Transform point_DentroElevador;
    public Transform point_ForaElevador;


    [Header("Variaveis")]
    [SerializeField] float speedPorta;
    [SerializeField] float waitTime;   // tempo de espera em cada estado

    public bool isOpen { get; private set; }

    void Start()
    {
        StartCoroutine(LoopElevador());
    }

    IEnumerator LoopElevador()
    {
        while (true) // loop infinito
        {
            // Abre
            yield return StartCoroutine(MoverPorta(point_Aberto.position));
            isOpen = true;
            yield return new WaitForSeconds(waitTime);

            // Fecha
            yield return StartCoroutine(MoverPorta(point_Fechado.position));
            isOpen = false;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator MoverPorta(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, speedPorta * Time.deltaTime);
            yield return null; // espera o próximo frame
        }
    }
}
