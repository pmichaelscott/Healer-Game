using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] string characterName;
    [SerializeField] string description;
    [SerializeField] Sprite portrait;

    public string GetCharacterName() => characterName;
    public string GetDescription() => description;
    public Sprite GetPortrait() => portrait;
}
