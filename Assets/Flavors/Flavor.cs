using UnityEngine;

[CreateAssetMenu(fileName = "Flavor", menuName = "Scriptable Objects/Flavor")]
public class Flavor : ScriptableObject
{
    [SerializeField] private string flavorName;
    [SerializeField] private string description;
    [SerializeField] private Sprite flavorIcon;
    [SerializeField] private Color flavorColor;
    [SerializeField] private Flavor effectiveAgainst;

    public global::System.String FlavorName { get => flavorName; set => flavorName = value; }
    public global::System.String Description { get => description; set => description = value; }
    public Sprite FlavorIcon { get => flavorIcon; set => flavorIcon = value; }
    public Color FlavorColor { get => flavorColor; set => flavorColor = value; }
    public Flavor EffectiveAgainst { get => effectiveAgainst; set => effectiveAgainst = value; }
}
