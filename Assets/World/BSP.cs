using UnityEngine;

class BSPNode
{
    public RectInt area;
    public BSPNode left;
    public BSPNode right;
    public Room room;

    public bool IsLeaf => left == null && right == null;

    public BSPNode(RectInt area)
    {
        this.area = area;
    }

    public void GenerateBSP(int minPartitionSize)
    {
        if (area.width < minPartitionSize * 2 &&
            area.height < minPartitionSize * 2)
            return;

        bool splitHorizontally = Random.value > 0.5f;

        if (area.width > area.height && area.width / area.height >= 1.25f)
            splitHorizontally = false;
        else if (area.height > area.width && area.height / area.width >= 1.25f)
            splitHorizontally = true;

        int max = (splitHorizontally ? area.height : area.width) - minPartitionSize;
        if (max <= minPartitionSize)
            return;

        int split = Random.Range(minPartitionSize, max);

        if (splitHorizontally)
        {
            left = new BSPNode(new RectInt(
                area.x,
                area.y,
                area.width,
                split
            ));

            right = new BSPNode(new RectInt(
                area.x,
                area.y + split,
                area.width,
                area.height - split
            ));
        }
        else
        {
            left = new BSPNode(new RectInt(
                area.x,
                area.y,
                split,
                area.height
            ));

            right = new BSPNode(new RectInt(
                area.x + split,
                area.y,
                area.width - split,
                area.height
            ));
        }

        left.GenerateBSP(minPartitionSize);
        right.GenerateBSP(minPartitionSize);
    }

    public RectInt GetRoom()
    {
        if (IsLeaf)
            return area;

        if (left != null)
        {
            RectInt leftRoom = left.GetRoom();
            if (leftRoom.width > 0)
                return leftRoom;
        }

        if (right != null)
        {
            RectInt rightRoom = right.GetRoom();
            if (rightRoom.width > 0)
                return rightRoom;
        }

        return new RectInt();
    }



}

