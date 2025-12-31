using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] string characterName;
    [SerializeField] string description;
    [SerializeField] Sprite portrait;

    Health health;

    public string GetCharacterName() => characterName;
    public string GetDescription() => description;
    public Sprite GetPortrait() => portrait;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
            health = gameObject.AddComponent<Health>();
    }

    // Delegate health methods to Health component
    public void TakeDamage(float amount) => health.TakeDamage(amount);
    public void Heal(float amount) => health.Heal(amount);
    public float GetCurrentHealth() => health.GetCurrentHealth();
    public float GetMaxHealth() => health.GetMaxHealth();
    public float GetHealthPercent() => health.GetHealthPercent();
    public bool IsDead() => health.IsDead();
}
