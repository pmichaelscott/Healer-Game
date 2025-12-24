using UnityEngine;


[System.Serializable]
public class Ability
{
    public string abilityName;
    public Sprite icon;
    [TextArea] public string description;
    public float coolDown = 0f;

    public Ability(string name, Sprite ico, string desc, float cd)
    {
        abilityName = name;
        icon = ico;
        description = desc;
        coolDown = cd;
    }
}
