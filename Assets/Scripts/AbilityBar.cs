using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class AbilityBar : MonoBehaviour
{
    [SerializeField] List<Ability> abilities = new List<Ability>();
    [SerializeField] RectTransform container;
    [SerializeField] RectTransform buttonPrefab;

    [Header("Button Layout")]
    [SerializeField] int buttonWidth = 50;
    [SerializeField] int buttonHeight = 50;
    [SerializeField] int spacing = 4;

    List<AbilityButton> buttons = new List<AbilityButton>();

    void Start()
    {
        if (container == null)
        {
            Debug.LogError("AbilityBar: container not assigned.", this);
            return;
        }

        // Ensure we have 6 abilities
        while (abilities.Count < 6)
            // abilities.Add(new Ability($"Ability {abilities.Count + 1}", null, "", 0f));
            abilities.Add(ScriptableObject.CreateInstance<Ability>());

        // Create buttons
        for (int i = 0; i < 6; i++)
        {
            CreateAbilityButton(i, abilities[i]);
        }
    }

    void CreateAbilityButton(int index, Ability ability)
    {

        RectTransform rt = Instantiate(buttonPrefab, container);
        
        // Position horizontally at bottom
        rt.anchoredPosition = new Vector2((buttonWidth + spacing) * index, 0f);

        // Add AbilityButton component
        var ab = rt.GetComponent<AbilityButton>();
        if (ab == null) ab = rt.gameObject.AddComponent<AbilityButton>();
        ab.Initialize(index, ability, OnAbilityCast);

        buttons.Add(ab);
    }

    void Update()
    {
        // Check keyboard input 1-6
        if (Keyboard.current != null)
        {
            for (int i = 0; i < 6; i++)
            {
                Key key = i switch
                {
                    0 => Key.Digit1,
                    1 => Key.Digit2,
                    2 => Key.Digit3,
                    3 => Key.Digit4,
                    4 => Key.Digit5,
                    5 => Key.Digit6,
                    _ => Key.None
                };

                if (key != Key.None && Keyboard.current[key].wasPressedThisFrame)
                {
                    buttons[i].Cast();
                }
            }
        }
    }

    void OnAbilityCast(int slotIndex)
    {
        var ab = buttons[slotIndex].GetAbility();
        if (ab == null) return;
        
        Debug.Log($"Ability cast: {ab.abilityName}");
        // TODO: Implement actual ability casting logic here
    }

    public Ability GetAbility(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < buttons.Count)
            return buttons[slotIndex].GetAbility();
        return null;
    }

    public void SetAbility(int slotIndex, Ability ability)
    {
        if (slotIndex >= 0 && slotIndex < buttons.Count && buttons[slotIndex] != null)
        {
            buttons[slotIndex].Initialize(slotIndex, ability, OnAbilityCast);
        }
    }
}
