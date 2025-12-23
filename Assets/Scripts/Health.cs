using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] bool destroyOnDeath = false;

    [SerializeField] internal UnityEvent<float> onHealthChanged;  
    [SerializeField] UnityEvent onDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount < 0f) return;
        currentHealth -= amount;
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount < 0f) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth);
    }

    void Die()
    {
        onDeath?.Invoke();
        if (destroyOnDeath)
            Destroy(gameObject);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => currentHealth / maxHealth;
    public bool IsDead() => currentHealth <= 0f;
}
