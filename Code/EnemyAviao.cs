using System;
using UnityEngine;

public class EnemyAviao : MonoBehaviour
{
    [SerializeField] private Rigidbody2D enemey_Rb;
    [SerializeField] public float enemySpeed;

    public Vector3 StartPosition { get;  set; }
    public event Action<EnemyAviao> OnEnemyHiddenAviao;
    private bool isMoving = false;

    void Start()
    {
        StartPosition = transform.position;
        StopEnemy();

    }
    public void StartEnemy()
    {
        isMoving = true;
        enemey_Rb.linearVelocity = new Vector2(enemySpeed, 0);
    }

    public void StopEnemy()
    {
        isMoving = false;
        enemey_Rb.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // Mantém a velocidade constante, sem perder energia
            enemey_Rb.linearVelocity = enemey_Rb.linearVelocity.normalized * enemySpeed;
        }
    }


    private void OnBecameInvisible()
    {
        // reseta posição
        transform.position = StartPosition;
        StopEnemy();

        gameObject.SetActive(false);

        OnEnemyHiddenAviao?.Invoke(this);
    }
}
