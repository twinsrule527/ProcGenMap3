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
    public float chanceTurnOnNewRoom;//Probability that a hallway will turn when it creates a new room
    public float jogChance;//The chance that at any given moment, the hallway will jog to the side by  some amount
    public int maxJogAmount;//When jogging, how much the hallway can jog by
    public List<Tile> FloorTiles;//List of types of tiles for the floor (might eventually be split up into floor types)
    public Tile WallTile;
    public Tile EndTile;
    public Tile DoorTile;
    public float DoorAtRoomEndChance;
    public SecondaryHallwayMaker SecHallPrefab;//Prefab of the main kind of secondary hallway maker
    public int maxSecHallLength;//The longest and shortest distances a secondary hallway can be
    public int minSecHallLength;
    public float HallDeathRoomChance;//Chance of a room spawning when a secondary hallwaymaker destroys itself
    public RoomMaker RoomMakerPrefab;//Prefab of the room maker
    public int minRoomLength;//Possible dimensions of a room
    public int maxRoomLength;
    public bool endTileIsRoom;//Whether or not the end Tile must be in a room

    //Gets all tiles adjacent to a given tile
    public TileTraits[] Adjacency(Vector2Int pos) {
        TileTraits[] newAdjacency = new TileTraits[8];
        newAdjacency[0] = TileManager.Instance.Tiles[pos + Vector2Int.up];
        newAdjacency[1] = TileManager.Instance.Tiles[pos + Vector2Int.up + Vector2Int.right];
        newAdjacency[2] = TileManager.Instance.Tiles[pos + Vector2Int.right];
        newAdjacency[3] = TileManager.Instance.Tiles[pos + Vector2Int.down + Vector2Int.right];
        newAdjacency[4] = TileManager.Instance.Tiles[pos + Vector2Int.down];
        newAdjacency[5] = TileManager.Instance.Tiles[pos + Vector2Int.down + Vector2Int.left];
        newAdjacency[6] = TileManager.Instance.Tiles[pos + Vector2Int.left];
        newAdjacency[7] = TileManager.Instance.Tiles[pos + Vector2Int.left + Vector2Int.up];
        return newAdjacency;
    }
    //Same as above, but only in cardinal directions
    public TileTraits[] CardinalAdjacency(Vector2Int pos) {
        TileTraits[] newAdjacency = new TileTraits[4];
        newAdjacency[0] = TileManager.Instance.Tiles[pos + Vector2Int.up];
        newAdjacency[1] = TileManager.Instance.Tiles[pos + Vector2Int.right];
        newAdjacency[2] = TileManager.Instance.Tiles[pos + Vector2Int.down];
        newAdjacency[3] = TileManager.Instance.Tiles[pos + Vector2Int.left];
        return newAdjacency;
    }

}