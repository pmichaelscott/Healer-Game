using UnityEngine;

[DisallowMultipleComponent]
public class SelectionIndicator : MonoBehaviour
{
    SpriteRenderer sr;
    GameObject indicatorObject;

    [SerializeField] Vector3 localOffset = new Vector3(0f, 1.5f, 0f);

    void Awake()
    {

        indicatorObject = transform.Find("_SelectionIndicator")?.gameObject;
        if (indicatorObject == null)
        {
            indicatorObject = new GameObject("_SelectionIndicator");
            indicatorObject.transform.SetParent(transform, false);
            indicatorObject.transform.localPosition = localOffset;
            sr = indicatorObject.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 100; // show on top
            sr.enabled = false;
        }
        else
        {
            sr = indicatorObject.GetComponent<SpriteRenderer>();
            if (sr == null) sr = indicatorObject.AddComponent<SpriteRenderer>();
            sr.enabled = false;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (sr == null) return;
        sr.sprite = sprite;
    }

    public void SetSelected(bool selected)
    {
        if (sr == null) return;
        sr.enabled = selected;
    }
}
