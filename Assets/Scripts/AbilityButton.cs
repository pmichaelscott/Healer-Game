using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class AbilityButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image coolDownOverlay;
    [SerializeField] Text keyBindText;
    [SerializeField] Button button;

    Ability ability;
    int slotIndex;
    float coolDownRemaining = 0f;
    System.Action<int> onAbilityClick;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        
    }

    void Update()
    {
        // Update cool down display
        if (coolDownRemaining > 0f)
        {
            coolDownRemaining -= Time.deltaTime;
            UpdateCoolDownDisplay();
        }
        else if (coolDownOverlay != null && coolDownOverlay.enabled)
        {
            coolDownOverlay.enabled = false;
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
        if (coolDownRemaining > 0f) return;
        
        coolDownRemaining = ability?.coolDown ?? 0f;
        UpdateCoolDownDisplay();
        onAbilityClick?.Invoke(slotIndex);
    }

    void UpdateCoolDownDisplay()
    {
        if (coolDownOverlay == null) return;

        if (coolDownRemaining > 0f)
        {
            coolDownOverlay.enabled = true;
            float percent = coolDownRemaining / (ability?.coolDown ?? 1f);
            percent = Mathf.Clamp01(percent);
            coolDownOverlay.fillAmount = percent;
        }
    }

    public Ability GetAbility() => ability;
    public bool IsOnCooldown() => coolDownRemaining > 0f;
    public float GetCoolDownRemaining() => coolDownRemaining;
}
