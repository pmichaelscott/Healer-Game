using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float damage = 10f;
    [SerializeField] float attackInterval = 2f;
    [Tooltip("Target Character to attack")]
    public Character target;

    Health health;
    Coroutine attackRoutine;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
            health = gameObject.AddComponent<Health>();
    }

    void Start()
    {
        if (attackInterval > 0f)
            attackRoutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            if (target != null && !target.IsDead())
            {
                target.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
