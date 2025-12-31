using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PartyUIManager : MonoBehaviour
{
    [Header("Party Members")]
    [Tooltip("Assign Character components for party members (exclude player). If empty, will auto-find by tag 'PartyMember'.")]
    public List<Character> partyMembers = new List<Character>();

    [Header("Selection")]
    [Tooltip("Sprite used to show selection above characters (world-space SpriteRenderer)")]
    public Sprite selectionSprite;

    SelectionIndicator[] indicators;
    int selectedIndex = -1;

    void Start()
    {
        // Auto-find if list empty
        if (partyMembers == null || partyMembers.Count == 0)
        {
            var foundByTag = GameObject.FindGameObjectsWithTag("partyMember");
            foreach (var go in foundByTag)
            {
                var c = go.GetComponent<Character>();
                if (c != null) partyMembers.Add(c);
                Debug.Log("PartyUIManager: Auto-found party member " + c.GetCharacterName(), this);
            }

            // Fallback: find all Character components in scene if tagging wasn't used
            if (partyMembers.Count == 0)
            {
                Debug.LogWarning("PartyUIManager: No party members found by tag 'PartyMember'. Using all Character components in scene.", this);    
                var foundCharacter = FindObjectsOfType<Character>();
                foreach (var c in foundCharacter)
                {
                    partyMembers.Add(c);
                }
            }
        }

        // Ensure each party member has a collider for click detection
        for (int i = 0; i < partyMembers.Count; i++)
        {
            var go = partyMembers[i].gameObject;
            if (go.GetComponent<Collider2D>() == null)
            {
                go.AddComponent<CircleCollider2D>();
            }
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

    void Update()
    {
        // Check for mouse click on party members using raycasting
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                // Check if the hit object is a party member
                for (int i = 0; i < partyMembers.Count; i++)
                {
                    if (hit.collider.gameObject == partyMembers[i].gameObject)
                    {
                        SelectIndex(i);
                        return;
                    }
                }
            }
        }
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
