using System.Collections;
using UnityEngine;

public class Lula : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;

    [SerializeField] private Rigidbody2D lula_Rb;


    [SerializeField] private float lula_Speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lula_Rb.linearVelocityX = lula_Speed;

    }

    private void OnCollisionEnter2D(Collision2D col)
    {

        Teleports teleport = col.gameObject.GetComponent<Teleports>();

        if (teleport != null)
        {
            int index = teleport.targetIndexTeleports;

            if (index >= 0 && index < GameManager.portaisSaida.Length)
            {
                transform.position = GameManager.portaisSaida[index].position;
            }

        }
       

        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(IEfuiPego());
        }

        
    }

    private IEnumerator IEfuiPego()
    {
        lula_Speed = 0f;
        print("fui pego");
        lula_Rb.linearVelocityX = lula_Speed;
        yield return new WaitForSeconds(2);
        lula_Speed = -3f;
    }
}
