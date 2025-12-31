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
    [SerializeField] Image castOverlay;
    RectTransform castOverlayRect;
    bool isCasting = false;
    float castRemaining = 0f;
    float maxCastTime = 0f;
    System.Action<int> onAbilityClick;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (coolDownOverlay != null)
            overlayRect = coolDownOverlay.GetComponent<RectTransform>();
        if (castOverlay != null)
            castOverlayRect = castOverlay.GetComponent<RectTransform>();
        if (castOverlay != null) castOverlay.enabled = false;
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
        
        // Update casting progress
        if (isCasting)
        {
            castRemaining -= Time.deltaTime;
            float t = 1f - (castRemaining / Mathf.Max(1e-6f, maxCastTime));
            t = Mathf.Clamp01(t);
            if (castOverlayRect != null)
            {
                Vector3 s = castOverlayRect.localScale;
                s.x = t;
                castOverlayRect.localScale = s;
            }

            if (castRemaining <= 0f)
            {
                isCasting = false;
                if (castOverlay != null) castOverlay.enabled = false;
                TriggerCastComplete();
            }
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
            button.onClick.AddListener(() => StartCast());
        maxCastTime = ability?.castTime ?? 0f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCast();
    }

    public void StartCast()
    {
        if (ability == null) return;
        if (coolDownRemaining > 0f) return; // still cooling down
        if (isCasting) return;

        if (ability.castTime <= 0f)
        {
            // instant cast
            TriggerCastComplete();
        }
        else
        {
            isCasting = true;
            castRemaining = ability.castTime;
            maxCastTime = ability.castTime;
            if (castOverlay != null)
            {
                castOverlay.enabled = true;
                if (castOverlayRect != null)
                {
                    Vector3 s = castOverlayRect.localScale;
                    s.x = 0f;
                    castOverlayRect.localScale = s;
                }
            }
        }
    }

    void TriggerCastComplete()
    {
        // perform the ability effect
        onAbilityClick?.Invoke(slotIndex);
        // start cooldown
        coolDownRemaining = ability?.coolDown ?? 0f;
        UpdateCoolDownDisplay();
    }

    void UpdateCoolDownDisplay()
    {
        if (coolDownOverlay == null || overlayRect == null) return;

        if (coolDownRemaining > 0f)
        {
            coolDownOverlay.enabled = true;
            float percent = 0f;
            if (maxCooldown > 0f)
                percent = coolDownRemaining / maxCooldown;
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
