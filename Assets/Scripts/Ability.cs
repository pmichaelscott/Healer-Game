using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "Ability", menuName = "Game/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    [TextArea] public string description;
    public float coolDown = 0f;
    public float castTime = 0f;

    public Ability(string name, Sprite ico, string desc, float cd, float ct)
    {
        abilityName = name;
        icon = ico;
        description = desc;
        coolDown = cd;
        castTime = ct;
    }
}
