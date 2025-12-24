using System.Collections;
using UnityEngine;

/// <summary>
/// Optional component to let a `Character` periodically attack a target `Enemy`.
/// Attach to the `Character` GameObject and assign `targetEnemy`.
/// </summary>
public class CharacterAttacker : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] Enemy targetEnemy;
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
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(damage);
            }
        }
    }
}
