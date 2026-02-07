using UnityEngine;

class BSPNode
{
    public RectInt area;
    public BSPNode left;
    public BSPNode right;
    public RectInt room;

    public bool IsLeaf => left == null && right == null;

    public BSPNode(RectInt area)
    {
        this.area = area;
    }
}