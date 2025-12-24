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
    float maxCooldown = 0f;
    RectTransform overlayRect;
    System.Action<int> onAbilityClick;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (coolDownOverlay != null)
            overlayRect = coolDownOverlay.GetComponent<RectTransform>();
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
        maxCooldown = ab?.coolDown ?? 0f;

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
        if (coolDownOverlay == null || overlayRect == null) return;

        if (coolDownRemaining > 0f)
        {
            coolDownOverlay.enabled = true;
            float percent = coolDownRemaining / maxCooldown;
            percent = Mathf.Clamp01(percent);
            
            // Scale overlay vertically from 1 (full) to 0 (empty)
            Vector3 scale = overlayRect.localScale;
            scale.y = percent;
            overlayRect.localScale = scale;
        }
        else
        {
            coolDownOverlay.enabled = false;
        }
    }

    public Ability GetAbility() => ability;
    public bool IsOnCooldown() => coolDownRemaining > 0f;
    public float GetCoolDownRemaining() => coolDownRemaining;
}
