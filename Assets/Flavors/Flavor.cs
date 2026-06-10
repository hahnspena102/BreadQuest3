using UnityEngine;

[CreateAssetMenu(fileName = "Flavor", menuName = "Scriptable Objects/Flavor")]
public class Flavor : ScriptableObject
{
    [SerializeField] private string flavorName;
    [SerializeField] private string description;
    [SerializeField] private Sprite flavorSprite;
    [SerializeField] private Color flavorColor;
    [SerializeField] private Flavor effectiveAgainst;

    public global::System.String FlavorName { get => flavorName; set => flavorName = value; }
    public global::System.String Description { get => description; set => description = value; }
    public Sprite FlavorIcon { get => flavorSprite; set => flavorSprite = value; }
    public Color FlavorColor { get => flavorColor; set => flavorColor = value; }
    public Flavor EffectiveAgainst { get => effectiveAgainst; set => effectiveAgainst = value; }
    public Sprite FlavorSprite { get => flavorSprite; set => flavorSprite = value; }
}
