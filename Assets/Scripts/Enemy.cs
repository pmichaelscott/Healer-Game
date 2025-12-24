using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] bool destroyOnDeath = true;

    [Header("Attack")]
    [SerializeField] float damage = 10f;
    [SerializeField] float attackInterval = 2f;
    [Tooltip("Target Character to attack")]
    public Character target;

    [Header("Health Bar")]
    // [SerializeField] Vector3 barOffset = new Vector3(0f, 0.5f, 0f);
    // [SerializeField] Color barBgColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    // [SerializeField] Color barFgColor = new Color(0.1f, 0.8f, 0.2f, 1f);
    [SerializeField] float barWidth;
    [SerializeField] float barHeight;


    [SerializeField]GameObject barBg;
    [SerializeField] GameObject barFg;
    SpriteRenderer bgSr;
    SpriteRenderer fgSr;
    Sprite runtimeWhiteSprite;

    Coroutine attackRoutine;

    void Awake()
    {
        currentHealth = maxHealth;
        bgSr= barBg.GetComponent<SpriteRenderer>();
        fgSr= barFg.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // CreateHealthBar();
        UpdateHealthBar();
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


    int GetSortingOrder()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) return sr.sortingOrder;
        return 0;
    }

    void UpdateHealthBar()
    {
        if (fgSr == null || bgSr == null) return;
        float pct = (maxHealth > 0f) ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
        
        // update the forground bar based on current health percentage
        fgSr.size = new Vector2(barWidth * pct, barHeight);
        barFg.transform.localPosition = new Vector3(-barWidth * 0.5f + (barWidth * pct) * 0.5f, 0f, 0f);

    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        UpdateHealthBar();
        if (currentHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthBar();
    }

    void Die()
    {
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        if (destroyOnDeath) Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
