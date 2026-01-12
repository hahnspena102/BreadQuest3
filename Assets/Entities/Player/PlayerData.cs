using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : EntityData
{
    [SerializeField] private float gold;
    [SerializeField] private float experience;
    [SerializeField] private float critRate;
    [SerializeField] private float critBonus;

    public global::System.Single Gold { get => gold; set => gold = value; }
    public global::System.Single Experience { get => experience; set => experience = value; }
    public global::System.Single CritRate { get => critRate; set => critRate = value; }
    public global::System.Single CritBonus { get => critBonus; set => critBonus = value; }
}
