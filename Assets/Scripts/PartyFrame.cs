using UnityEngine;
using UnityEngine.UI;

public class PartyFrame : MonoBehaviour
{
    public Image backgroundImage;
    public Image fillImage;
    public Button button;
    public UnityEngine.UI.Text nameText;

    int index;
    System.Action<int> onClick;

    public void Initialize(int idx, System.Action<int> onClickAction)
    {
        index = idx;
        onClick = onClickAction;
            if (button != null)
            {
                button.onClick.AddListener(() => {
                    onClick?.Invoke(index);
                });
            }
    }

    public void SetName(string name)
    {
        if (nameText != null) nameText.text = name;
    }

    public void SetHealthPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);
        if (fillImage == null)
        {
            return;
        }

        // Ensure correct image settings for fill usage
        if (fillImage.type != Image.Type.Filled)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        // Smoothly animate the fill to the target percent
        StopAllCoroutines();
        StartCoroutine(AnimateFill(fillImage.fillAmount, percent, 0.12f));
    }

    System.Collections.IEnumerator AnimateFill(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(from, to, t / duration);
            fillImage.fillAmount = v;
            yield return null;
        }
        fillImage.fillAmount = to;
    }
}
