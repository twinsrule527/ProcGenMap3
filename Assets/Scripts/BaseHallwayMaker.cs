using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//Main HallwayCreator (might be different script than the secondary one)
public class BaseHallwayMaker : MonoBehaviour
{
    [SerializeField] protected HallwayFunctions Functions;
    protected int curStraightLength;
    void Start()
    {
        TileManager.Instance.tileCount = 0;
        curStraightLength = 0;
        TileManager.Instance.Makers.Add(gameObject);
    }

    public virtual void Update()
    {
        //Goes straight as long a its straight length is less than the minimum
        if(curStraightLength < Functions.minStraightLength) {
            curStraightLength++;
            HallForward();
        }
        //Otherwise, might turn
        else {
            float rnd = Random.Range(0f, 1f);
            //If random percent is greater than the base percent, it will make a turn
                //TODO:
                    //1) Chance of turning increases the longer the straight
                    //2) Chance to turn is also a chance to create a room or a new hallway
                    //3) Direction of chance to turn is dependent on how many times it turned in one direction
            if(rnd < Functions.baseTurnChance) {
                //Within the turn chance, it has a chance to create a secondary hallway, and a chance to create a room - w/ either of those, it might not turn

                float rnd2 = Random.Range(0f, 1f);
                if(rnd2 < Functions.newHallChanceOnTurn) {//Create a second hallway
                    CreateNewHallway();
                }
                else if(rnd2 < Functions.newHallChanceOnTurn + Functions.newRoomChance) {//Create a new Room
                    CreateRoom();
                }
                else {//Just turn
                    HallTurn();
                }
                curStraightLength = 1;
            }
            HallForward();
        }
        //If the maximum length has been reached, it sacrifices itself, and leads to the game end
        if(TileManager.Instance.tileCount >= Functions.maxTileCount) {
            TileManager.Instance.Makers.Remove(gameObject);
            TileManager.Instance.endgame = 1;
            Destroy(gameObject);
        }
    }

    //moves forward, then creates a new hall tile
    protected virtual void HallForward() {
        transform.position += transform.up;
        Vector2Int intPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        TileManager.Instance.PlaceTile(intPos, TileType.Hallway);
    }
    //When the hallwaymaker needs to turn
    protected void HallTurn() {
        float rnd = Random.Range(0f, 1f);
        Vector3 rot = new Vector3(0f, 0f, 90f);
        if(rnd > 0.5f) {
        //Rotate right
            transform.rotation *= Quaternion.Euler(rot);
        }
        else {
            //Rotate Left
            transform.rotation *= Quaternion.Euler(-rot);
        }
    }

    //Creates a room in front of the hall
    protected void CreateRoom() {
        int width = Random.Range(Functions.minRoomLength, Functions.maxRoomLength);
        int height = Random.Range(Functions.minRoomLength, Functions.maxRoomLength);
        RoomMaker newRoom = Instantiate(Functions.RoomMakerPrefab, transform.position, transform.rotation);
        newRoom.transform.position += transform.up;
        newRoom.width = width;
        newRoom.height = height;
        int offset = Random.Range(0, width);
        newRoom.transform.position -= offset * transform.right;
        TileManager.Instance.Makers.Add(newRoom.gameObject);
    }

    //Creates a new Hallway
    private void CreateNewHallway() {
        //Instantiates a new hallway_generator
        SecondaryHallwayMaker newHall = Instantiate(Functions.SecHallPrefab, transform.position, transform.rotation);
        //base hallway and new hallway both turn as needed
        float rnd = Random.Range(0f, 1f);
        Vector3 rot = new Vector3(0f, 0f, 90f);
        //if/else statement determines the different directions the hallways might go (one or both turn)
        if(rnd < 1f/6f) {
                newHall.transform.rotation *= Quaternion.Euler(rot);
        }
        else if(rnd < 1f/3f) {
                newHall.transform.rotation *= Quaternion.Euler(-rot);
        }
        else if(rnd < 1f/2f) {
                transform.rotation *= Quaternion.Euler(rot);
        }
        else if(rnd < 2f/3f) {
                transform.rotation *= Quaternion.Euler(-rot);
        }
        else if(rnd < 5f/6f) {
                newHall.transform.rotation *= Quaternion.Euler(rot);
                transform.rotation *= Quaternion.Euler(-rot);
        }
        else if(rnd < 1f) {
                transform.rotation *= Quaternion.Euler(rot);
                newHall.transform.rotation *= Quaternion.Euler(-rot);
        }
        //Hallway's length is within a certain range of values
        newHall.maxHallLength = Random.Range(Functions.minSecHallLength, Functions.maxSecHallLength);
        TileManager.Instance.Makers.Add(newHall.gameObject);
    }

}
