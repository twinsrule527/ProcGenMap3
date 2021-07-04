using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//Scriptable Object which contains various functions and variables for HallwayMakers
[CreateAssetMenu(menuName = "ScriptableObjects/HallwayFunctions")]
public class HallwayFunctions : ScriptableObject
{
    public int maxTileCount;
    public int minStraightLength;//how long a hallway has to go before it can turn
    public float baseTurnChance;//once a hallway reaches its min distance, the chance it has to turn
    public float newHallChanceOnTurn;//Probability that it generates a new hallway instead of turning
    public float newRoomChance;//Probability that it generates a new room instead of turning (or plus turning)
    public float newHallChanceOnRoom;//Proability that it generates a new hallway when it generates a new room
    public List<Tile> FloorTiles;//List of types of tiles for the floor (might eventually be split up into floor types)
    public Tile WallTile;
    public SecondaryHallwayMaker SecHallPrefab;//Prefab of the main kind of secondary hallway maker
    public int maxSecHallLength;//The longest and shortest distances a secondary hallway can be
    public int minSecHallLength;
    public float HallDeathRoomChance;//Chance of a room spawning when a secondary hallwaymaker destroys itself
    public RoomMaker RoomMakerPrefab;//Prefab of the room maker
    public int minRoomLength;//Possible dimensions of a room
    public int maxRoomLength;


}
