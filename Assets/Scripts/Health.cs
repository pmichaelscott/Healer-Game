using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] bool destroyOnDeath = true;

    [Header("Health Bar")]
    [SerializeField] float barWidth;
    [SerializeField] float barHeight;
    [SerializeField] GameObject barBg;
    [SerializeField] GameObject barFg;
    SpriteRenderer bgSr;
    SpriteRenderer fgSr;

    [SerializeField] UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        if (barBg != null) bgSr = barBg.GetComponent<SpriteRenderer>();
        if (barFg != null) fgSr = barFg.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (fgSr == null || bgSr == null) return;
        float pct = (maxHealth > 0f) ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
        
        // update the foreground bar based on current health percentage
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
        onDeath?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => (maxHealth > 0f) ? currentHealth / maxHealth : 0f;
    public bool IsDead() => currentHealth <= 0f;
}
