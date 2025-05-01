using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject focalPoint;
    private Rigidbody rb;
    private Coroutine runingSmashRoutine= null;
    private InputAction moveAction;
    private InputAction smashAction;
    private InputAction breakAction;

    private bool hasPowerUp = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        smashAction = InputSystem.actions.FindAction("Smash");
        breakAction = InputSystem.actions.FindAction("Break");
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = moveAction.ReadValue<Vector2>().y;
        rb.AddForce(verticalInput * speed*  focalPoint.transform.forward);
        if (breakAction.IsPressed())
        {
            rb.linearVelocity = Vector3.zero;
            
        }

        if (smashAction.triggered)
        {
            if (hasPowerUp == true)
            {
               runingSmashRoutine = StartCoroutine(SmashRoutine());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
            hasPowerUp = true;
            StartCoroutine(PowerUpCooldownRoutine(60));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            if (hasPowerUp)
            {
                Vector3 dir = collision.transform.position - transform.position;
                dir.Normalize();
                Rigidbody EnemyRb = collision.gameObject.GetComponent<Rigidbody>();
                EnemyRb.AddForce(dir * 10f, ForceMode.Impulse);
            }
        }
    }

    IEnumerator PowerUpCooldownRoutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        hasPowerUp = false;
        if (runingSmashRoutine != null)
        {
            StopCoroutine(runingSmashRoutine);
        }
    }

    IEnumerator SmashRoutine()
    {
        float chargetime = 0;
        while (smashAction.IsPressed())
        {
            chargetime += Time.deltaTime;
            yield return null;
            if (chargetime >= 2f)
            {
                Debug.Log("Smash activated!!");
                break;
            }
        }

        if (chargetime > 2f)
        {
            Debug.Log("Smash cancled");
            yield break;
        }

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        for (int i = 0; i < enemies.Length; i++)
        {
            Rigidbody enemyRb = enemies[i].GetComponent<Rigidbody>();
            enemyRb.AddExplosionForce(10f, transform.position, 100,0, ForceMode.Impulse);
        }
        
        yield return null;

    }
    
}
