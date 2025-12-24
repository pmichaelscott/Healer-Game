using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A single ability button on the action bar.
/// Displays icon, cooldown, and key binding (1-6).
/// </summary>
public class AbilityButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image cooldownOverlay;
    [SerializeField] Text keyBindText;
    [SerializeField] Button button;

    Ability ability;
    int slotIndex;
    float cooldownRemaining = 0f;
    System.Action<int> onAbilityClick;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (button == null) button = gameObject.AddComponent<Button>();
    }

    void Update()
    {
        // Update cooldown display
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
            UpdateCooldownDisplay();
        }
        else if (cooldownOverlay != null && cooldownOverlay.enabled)
        {
            cooldownOverlay.enabled = false;
        }
    }

    public void Initialize(int idx, Ability ab, System.Action<int> onClickCallback)
    {
        slotIndex = idx;
        ability = ab;
        onAbilityClick = onClickCallback;

        // Set icon
        if (iconImage != null && ability?.icon != null)
            iconImage.sprite = ability.icon;

        // Set key bind label (1-6)
        if (keyBindText != null)
            keyBindText.text = (idx + 1).ToString();

        // Setup button click
        if (button != null)
            button.onClick.AddListener(() => Cast());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Cast();
    }

    public void Cast()
    {
        if (cooldownRemaining > 0f) return;
        
        cooldownRemaining = ability?.cooldown ?? 0f;
        UpdateCooldownDisplay();
        onAbilityClick?.Invoke(slotIndex);
    }

    void UpdateCooldownDisplay()
    {
        if (cooldownOverlay == null) return;

        if (cooldownRemaining > 0f)
        {
            cooldownOverlay.enabled = true;
            float percent = cooldownRemaining / (ability?.cooldown ?? 1f);
            percent = Mathf.Clamp01(percent);
            cooldownOverlay.fillAmount = percent;
        }
    }

    public Ability GetAbility() => ability;
    public bool IsOnCooldown() => cooldownRemaining > 0f;
    public float GetCooldownRemaining() => cooldownRemaining;
}
