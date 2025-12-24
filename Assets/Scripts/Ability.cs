using UnityEngine;

/// <summary>
/// Stores data for a single ability (icon, name, cooldown, etc).
/// </summary>
[System.Serializable]
public class Ability
{
    public string abilityName;
    public Sprite icon;
    [TextArea] public string description;
    public float cooldown = 0f;

    public Ability(string name, Sprite ico, string desc, float cd)
    {
        abilityName = name;
        icon = ico;
        description = desc;
        cooldown = cd;
    }
}
