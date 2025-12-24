using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PartyUIManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Container RectTransform under a Canvas where frames will be created")]
    public RectTransform container;
    [Tooltip("Optional UI prefab for a single frame (must contain PartyFrame component)")]
    public RectTransform framePrefab;
    [Tooltip("Sprite used to show selection above characters (world-space SpriteRenderer)")]
    public Sprite selectionSprite;

    [Header("Layout")]
    public int frameWidth = 160;
    public int frameHeight = 28;
    public int spacing = 6;

    [Header("Party Members")]
    [Tooltip("Assign Character components for party members (exclude player). If empty, will auto-find by tag 'PartyMember'.")]
    public List<Character> partyMembers = new List<Character>();

    List<PartyFrame> frames = new List<PartyFrame>();
    SelectionIndicator[] indicators;

    int selectedIndex = -1;
    [Header("Debug")]
    public bool autoCreateEventSystem = true;
    public bool debugInvokeButtons = false;

    void Start()
    {
        // Auto-find if list empty
        if (partyMembers == null || partyMembers.Count == 0)
        {
            var foundByTag = GameObject.FindGameObjectsWithTag("PartyMember");
            foreach (var go in foundByTag)
            {
                var c = go.GetComponent<Character>();
                if (c != null) partyMembers.Add(c);
            }

            // Fallback: find all Health components in scene if tagging wasn't used
            if (partyMembers.Count == 0)
            {
                var foundCharacter = FindObjectsOfType<Character>();
                foreach (var c in foundCharacter)
                {
                    // exclude player by name or tag if needed; for now add all
                    partyMembers.Add(c);
                }
            }
        }


        // Ensure EventSystem and GraphicRaycaster exist for UI interaction
        if (autoCreateEventSystem && UnityEngine.EventSystems.EventSystem.current == null)
        {
            var esGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            
        }

        // Ensure container's Canvas has a GraphicRaycaster
        var parentCanvas = container.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("PartyUIManager: container is not under a Canvas. UI clicks won't work.", this);
        }
        else
        {
            var gr = parentCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (gr == null)
            {
                parentCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                // Debug.Log("PartyUIManager: Added GraphicRaycaster to Canvas.", this);
            }
        }

        // Create frames
        if (container == null)
        {
            Debug.LogError("PartyUIManager: container is not assigned. Create a Canvas and assign a Panel RectTransform.", this);
            return;
        }
        

        for (int i = 0; i < partyMembers.Count; i++)
        {
            CreateFrameForMember(i, partyMembers[i]);
        }



        // Ensure each party member has a SelectionIndicator and assign sprite
        indicators = new SelectionIndicator[partyMembers.Count];
        for (int i = 0; i < partyMembers.Count; i++)
        {
            var si = partyMembers[i].GetComponent<SelectionIndicator>();
            if (si == null)
                si = partyMembers[i].gameObject.AddComponent<SelectionIndicator>();
            si.SetSprite(selectionSprite);
            si.SetSelected(false);
            indicators[i] = si;
        }

        // Select none initially
        SelectIndex(-1);
    }

    void CreateFrameForMember(int index, Character c)
    {
        RectTransform rt;
        PartyFrame pf;
        rt = Instantiate(framePrefab, container);
        pf = rt.GetComponent<PartyFrame>();
        if (pf == null) pf = rt.gameObject.AddComponent<PartyFrame>();

        // Ensure button's targetGraphic is assigned (useful when using prefabs)
        if (pf != null && pf.button != null && pf.backgroundImage != null)
            pf.button.targetGraphic = pf.backgroundImage;

        rt.anchoredPosition = new Vector2((frameWidth + spacing) * frames.Count, 0f);

        // Diagnostic: log button / UI state for troubleshooting
        var buttonState = pf.button != null ? (pf.button.interactable ? "interactable" : "not-interactable") : "no-button";
        var bgRaycast = pf.backgroundImage != null ? pf.backgroundImage.raycastTarget : false;
        var canvas = container.GetComponentInParent<Canvas>();
        var canvasInfo = canvas != null ? (canvas.renderMode.ToString()) : "NoCanvas";


        // Initialize and subscribe to health changes
        pf.Initialize(index, SelectIndex);
        pf.SetName(c.GetComponent<Character>()?.GetCharacterName() ?? c.gameObject.name);
        pf.SetHealthPercent(c.GetHealthPercent());

        // Prevent child graphics from intercepting raycasts so the Button receives clicks
        if (pf.fillImage != null) pf.fillImage.raycastTarget = false;
        if (pf.nameText != null) pf.nameText.raycastTarget = false;
        if (pf.backgroundImage != null) pf.backgroundImage.raycastTarget = true;

        c.GetType();

        // Subscribe to Health change event if present
        c.GetComponent<Character>().onHealthChanged.AddListener((current) =>
        {
            float percent = current / c.GetMaxHealth();
            pf.SetHealthPercent(percent);
        });

        frames.Add(pf);
    }

    

    public void SelectIndex(int idx)
    {

        if (idx >= 0 && idx < indicators.Length)
        {
            // Deselect previous
            if (selectedIndex >= 0 && selectedIndex < indicators.Length)
                indicators[selectedIndex].SetSelected(false);

            selectedIndex = idx;
            indicators[selectedIndex].SetSelected(true);
        }
        else
        {
            if (selectedIndex >= 0 && selectedIndex < indicators.Length)
                indicators[selectedIndex].SetSelected(false);
            selectedIndex = -1;
        }
    }
}
