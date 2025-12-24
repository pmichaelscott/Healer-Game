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
            abilities.Add(new Ability($"Ability {abilities.Count + 1}", null, "", 0f));

        // Create buttons
        for (int i = 0; i < 6; i++)
        {
            CreateAbilityButton(i, abilities[i]);
        }
    }

    void CreateAbilityButton(int index, Ability ability)
    {
        RectTransform rt;
        if (buttonPrefab != null)
        {
            rt = Instantiate(buttonPrefab, container);
        }
        else
        {
            var go = new GameObject($"AbilityButton_{index + 1}", typeof(RectTransform));
            go.transform.SetParent(container, false);
            rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(buttonWidth, buttonHeight);

            // Background
            var bgImage = go.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

            // Icon
            var iconGO = new GameObject("Icon", typeof(RectTransform));
            iconGO.transform.SetParent(rt, false);
            var iconRT = iconGO.GetComponent<RectTransform>();
            iconRT.anchorMin = Vector2.zero;
            iconRT.anchorMax = Vector2.one;
            iconRT.offsetMin = Vector2.zero;
            iconRT.offsetMax = Vector2.zero;
            var iconImage = iconGO.AddComponent<Image>();
            iconImage.raycastTarget = false;

            // Cooldown overlay
            var cdGO = new GameObject("Cooldown", typeof(RectTransform));
            cdGO.transform.SetParent(rt, false);
            var cdRT = cdGO.GetComponent<RectTransform>();
            cdRT.anchorMin = Vector2.zero;
            cdRT.anchorMax = Vector2.one;
            cdRT.offsetMin = Vector2.zero;
            cdRT.offsetMax = Vector2.zero;
            var cdImage = cdGO.AddComponent<Image>();
            cdImage.color = new Color(0f, 0f, 0f, 0.6f);
            cdImage.type = Image.Type.Filled;
            cdImage.fillMethod = Image.FillMethod.Vertical;
            cdImage.fillOrigin = (int)Image.OriginVertical.Bottom;
            cdImage.enabled = false;
            cdImage.raycastTarget = false;

            // Key bind text
            var textGO = new GameObject("KeyBind", typeof(RectTransform));
            textGO.transform.SetParent(rt, false);
            var textRT = textGO.GetComponent<RectTransform>();
            textRT.anchoredPosition = new Vector2(2, -2);
            textRT.sizeDelta = new Vector2(20, 20);
            var txt = textGO.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.text = (index + 1).ToString();
            txt.alignment = TextAnchor.UpperRight;
            txt.color = new Color(1f, 1f, 0f, 1f);
            txt.fontSize = 14;
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = false;

            // Button
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bgImage;
        }

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
