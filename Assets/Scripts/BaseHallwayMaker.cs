using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//Main HallwayCreator (might be different script than the secondary one)
public class BaseHallwayMaker : MonoBehaviour
{
    [SerializeField] protected HallwayFunctions Functions;
    protected int curStraightLength;
    protected int turnTimes;//How many times the hallway maker has turned in 1 direction: positive is right, negative is left
    void Start()
    {
        TileManager.Instance.tileCount = 0;
        curStraightLength = 0;
        TileManager.Instance.Makers.Add(gameObject);
        TileManager.Instance.AddToMakers(gameObject);
        //points in a random direction
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        //Creates a Tile where it is
        TileManager.Instance.PlaceTile(GeneralFunctions.Vec3ToVec2Int(transform.position), TileType.Hallway);
    }

    public virtual void Update()
    {
        //Goes straight as long a its straight length is less than the minimum
        if(curStraightLength < Functions.minStraightLength) {
            curStraightLength++;
            //when going straight, has a tiny chance to jog first
            Jog();
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
                        //Within this, it has a chance to create another hallway
                            //As well as may turn, etc.
                    //Gets the dimensions of the new room
                    int width = Random.Range(Functions.minRoomLength, Functions.maxRoomLength);
                    int height = Random.Range(Functions.minRoomLength, Functions.maxRoomLength);
                    int offset = Random.Range(0, width);
                    float rnd3 = Random.Range(0f, 1f);//Is another hallway created?
                    Quaternion curRot = transform.rotation;//These 2 rotations are used for placement of new hallway
                    Quaternion futRot = curRot;
                    if(rnd3 < Functions.newHallChanceOnRoom) {
                        SecondaryHallwayMaker newHall = CreateNewHallway();
                        int newHall_h_offset = Random.Range(0, width);
                        int newHall_v_offset = Random.Range(0, height);
                        futRot = transform.rotation;
                        transform.rotation = curRot;
                        //Offsets the position so they are teleported to a random position in the room
                        newHall.transform.position -= transform.right * offset;
                        newHall.transform.position += transform.right * newHall_h_offset;
                        newHall.transform.position += transform.up * newHall_v_offset;
                    }
                    else {
                        //Otherwise, the old hallway still has a chance to turn
                        float rnd4 = Random.Range(0f, 1f);
                        if(rnd4 < Functions.chanceTurnOnNewRoom) {
                            HallTurn();
                        }
                    }
                    futRot = transform.rotation;
                    transform.rotation = curRot;
                    CreateRoom(width, height, offset);
                    int h_offset = Random.Range(0, width);
                    int v_offset = Random.Range(1, height + 1);
                    transform.position -= transform.right * offset;
                    transform.position += transform.right * h_offset;
                    transform.position += transform.up * v_offset;
                    transform.rotation = futRot;
                }
                else {//Just turn
                    HallTurn();
                    HallForward();//Only moves forward at this time
                }
                curStraightLength = 0;
            }
            else {
                Jog();
                HallForward();
            }
        }
        //If the maximum length has been reached, it sacrifices itself, and leads to the game end
        if(TileManager.Instance.tileCount >= Functions.maxTileCount) {
            //Game break happens before this
            Debug.Log(1);
            TileManager.Instance.Makers.Remove(gameObject);
            TileManager.Instance.RemoveFromMakers();
            TileManager.Instance.endgame = 1;
            Debug.Log(2);
            Destroy(gameObject);
        }
    }

    //moves forward, then creates a new hall tile
    protected virtual void HallForward() {
        transform.position += transform.up;
        Vector2Int intPos = GeneralFunctions.Vec3ToVec2Int(transform.position);
        TileManager.Instance.PlaceTile(intPos, TileType.Hallway);
    }
    //When the hallwaymaker needs to turn
    protected void HallTurn() {
        float rnd = Random.Range(0f, 1f);
        Vector3 rot = new Vector3(0f, 0f, 90f);
        if(rnd > 0.5f + 0.1f * turnTimes) {//If clause needs to include mathematical turn decay
        //Rotate right
            transform.rotation *= Quaternion.Euler(rot);
            if(turnTimes < 0) {
                turnTimes = 0;
            }
            else {
                turnTimes++;
            }
        }
        else {
            //Rotate Left
            transform.rotation *= Quaternion.Euler(-rot);
            if(turnTimes > 0) {
                turnTimes = 0;
            }
            else {
                turnTimes--;
            }
        }
    }
    //Checks to see if the hallway should jog, then jogs one way or another (more likely to jog the same direction as last time)
    protected void Jog() {
        float rnd = Random.Range(0f, 1f);
        if(rnd < Functions.jogChance) {
            float rnd1 = Random.Range(0f, 1f);
            //chooses a direction to jog - eventually will be more likely to be the same direction multiple times in a row
            int direction;
            if(rnd1 > 0.5f) {
                direction = 1;
            }
            else {
                direction = -1;
            }
            //Decides how much it will jog to the side by (base of 1)
            int jogAmount = Random.Range(1, Functions.maxJogAmount+1);
            for(int i = 0; i < jogAmount; i++) {
                transform.position += transform.right * direction;
                Vector2Int intPos = GeneralFunctions.Vec3ToVec2Int(transform.position);
                TileManager.Instance.PlaceTile(intPos, TileType.Hallway);
            }
        }
    }

    //Creates a room in front of the hall
        //Width/Height needs to be predetermined because other things might care about it
    protected RoomMaker CreateRoom(int width, int height, int offset) {
        RoomMaker newRoom = Instantiate(Functions.RoomMakerPrefab, transform.position, transform.rotation);
        newRoom.transform.position += transform.up;
        newRoom.width = width;
        newRoom.height = height;
        newRoom.transform.position -= offset * transform.right;
        TileManager.Instance.Makers.Add(newRoom.gameObject);
        TileManager.Instance.AddToMakers(newRoom.gameObject);
        return newRoom;
    }

    //Creates a new Hallway
    private SecondaryHallwayMaker CreateNewHallway() {
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
        TileManager.Instance.AddToMakers(newHall.gameObject);
        return newHall;
    }

    //Occurs when this maker is reenabled in the maker stack
        //For now, just skips forward until it is not standing on a filled square
    public void OnReEnable() {
        Vector2Int vec2IntPos = GeneralFunctions.Vec3ToVec2Int(transform.position);
        int val = 0;
        while(TileManager.Instance.Tiles.ContainsKey(vec2IntPos) && val < 1000) {
            Debug.Log(TileManager.Instance.Tiles[vec2IntPos].type);
            val++;
            transform.position += transform.up;
            vec2IntPos = GeneralFunctions.Vec3ToVec2Int(transform.position);
        }
        TileManager.Instance.PlaceTile(vec2IntPos, TileType.Hallway);
        
    }
}
