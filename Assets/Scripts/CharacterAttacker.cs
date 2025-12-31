using System.Collections;
using UnityEngine;

public class CharacterAttacker : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] Health targetHealth;
    [SerializeField] float damage = 8f;
    [SerializeField] float attackInterval = 2.5f;

    Coroutine atk;

    void OnValidate()
    {
        if (character == null) character = GetComponent<Character>();
    }

    void OnEnable()
    {
        if (attackInterval > 0f)
            atk = StartCoroutine(AttackRoutine());
    }

    void OnDisable()
    {
        if (atk != null) StopCoroutine(atk);
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }
}
