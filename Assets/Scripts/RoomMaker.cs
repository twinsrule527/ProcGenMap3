using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Is given a width and height, and creates a room on a hallway
    //Starts in the "lower-left" corner of how it would be oriented
public class RoomMaker : MonoBehaviour
{
    private int x;//current "x" position - may be rotated
    private int y;//current "y" position
    public int width;
    public int height;
    void Start()
    {
        x = 0;
        y = 0;
    }

    void Update()
    {
        //It starts by placing a tile
        Vector2Int intPos = GeneralFunctions.Vec3ToVec2Int(transform.position);
        TileManager.Instance.PlaceTile(intPos, TileType.Room);
        //When it reaches the side of the room, it moves up and back
        if(x >= width) {
            transform.position += transform.up;
            y++;
            transform.position -= transform.right * x;
            x = 0;
            //if its reached the end of the room, it gets destroyed
            if(y >= height) {
                TileManager.Instance.Makers.Remove(gameObject);
                TileManager.Instance.RemoveFromMakers();
                Destroy(gameObject);
            }
        }
        else {
            transform.position+= transform.right;
            x++;
        }
    }
}
