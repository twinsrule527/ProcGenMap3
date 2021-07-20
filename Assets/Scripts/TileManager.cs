using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//General Manager of Objects, but mainly Tiles

public enum GameState {//Different game states which the game can have, used for determining what camera/player movement happens
    Generating,
    Playing
}
public class TileManager : Singleton<TileManager>
{
    public Dictionary<Vector2Int, TileTraits> Tiles;
    public int tileCount;
    public Tilemap Map;
    public List<GameObject> Makers;//List of currently running makers (so that when they're all gone, the generation ends)
    private Stack<GameObject> MakerStack;
    public int endgame;
    [SerializeField] private HallwayFunctions Functions;
    [SerializeField] private int seed;
    public PlayerCharacter Player;
    private GameState _curGameState;//The global gamestate, can only be modified here, but can be viewed anywhere
    public GameState CurGameState {
        get {
            return _curGameState;
        }
    }
    private const int END_TILE_TRIES = 50;//How many times it tries to create an end tile
    private const float END_TILE_REPLACE_CHANCE = 0.2f;//The chance that the end tile will be replaced w/ one further away if it happens on a try
    void Awake()
    {
        Tiles = new Dictionary<Vector2Int, TileTraits>();
        Makers = new List<GameObject>();
        MakerStack = new Stack<GameObject>();
        endgame = 0;
        if(seed == 0) {
            seed = Random.Range(1, 2147483647);
        }
        Debug.Log(seed);
        Random.InitState(seed);
        _curGameState = GameState.Generating;
    }

    void Update()
    {
        //When the endgame is reached, it generates walls
        if(endgame == 1) {
            Debug.Log(1);//Game crashes before this
            if(Makers.Count == 0) {
                Player.gameObject.SetActive(true);
                _curGameState = GameState.Playing;//Currently automatically sets to Playing when game ends
                        //(Eventually, won't occur until player presses a button)
                endgame = 2;
                List<TileTraits> TileList = new List<TileTraits>();
                //Creates a list of all tile traits, then creates 
                foreach(var item in Tiles) {
                    TileList.Add(item.Value);
                }
                for(int i = 0; i < TileList.Count; i++) {
                    for(int j = -1; j < 2; j++) {
                        for(int k = -1; k < 2; k++) {
                            Vector2Int pos = new Vector2Int(TileList[i].x + j, TileList[i].y+k);
                            PlaceTile(pos, TileType.Wall);
                        }
                    }
                }
                for(int i = 0; i < TileList.Count; i++) {
                    MakeTileDoor(TileList[i]);
                }
                //Creates an exit, by using a list of all Tiles greater than a certain value
                /*
                for(int i = TileList.Count - 1; i >= 0; i--) {
                    if(TileList[i].number < Mathf.FloorToInt(tileCount / 2)) {
                        TileList.RemoveAt(i);
                    }
                }*/
                //Using this new list, picks a random spot until either its picked a certain number, or its picked one far enough from start
                TileTraits endTile = Tiles[Vector2Int.zero];
                int endDist = 0;
                int tries = 0;

                //If the end tile must be in a room, creates a new list of those tiles
                if(Functions.endTileIsRoom) {
                    List<TileTraits> newTileList = new List<TileTraits>();
                    foreach(TileTraits tile in TileList) {
                        if(tile.type == TileType.Room) {
                            newTileList.Add(tile);
                        }
                    }
                    //Only uses the new room list if there are tiles in rooms to use
                    if(newTileList.Count > 0) {
                        TileList = new List<TileTraits>(newTileList);
                    }
                }

                while(tries < END_TILE_TRIES) {//tries many times to get an optimal placement of an end tile
                    int val = Random.Range(0, TileList.Count);
                    TileTraits checkTile = TileList[val];
                    int checkDist = Mathf.Abs(checkTile.x) + Mathf.Abs(checkTile.y);
                    //If the new tile is further away from the start, it might become the actual end tile
                    if(endDist < checkDist) {
                        if(endDist == 0) {
                            endTile = checkTile;
                            endDist = checkDist;
                        }
                        else {
                            float rnd = Random.Range(0f, 1f);
                            if(rnd < END_TILE_REPLACE_CHANCE) {
                                endTile = checkTile;
                                endDist = checkDist;
                            }
                        }
                    }
                    tries++;
                }
                //EndTile becomes an EndTile
                PlaceTile(new Vector2Int(endTile.x, endTile.y), TileType.End);

            }
        }   
    }

    public void PlaceTile(Vector2Int pos, TileType type) {
        //Checks to see if there currently exists the key in the dictionary
        if(Tiles.ContainsKey(pos)) {
            //TODO: currently doesn't place anything (in future, will compare options)

            //For now: Room tiles override hall tiles
            TileTraits curTile = Tiles[pos];
            if(type == TileType.Room && curTile.type == TileType.Hallway) {
                Tile myTile = Functions.FloorTiles[0];
                Map.SetTile((Vector3Int)pos, myTile);
                curTile.type = type;
                curTile.sprite = myTile.sprite;
                Tiles[pos] = curTile;
            }
            else if(type == TileType.End) {
                Map.SetTile((Vector3Int)pos, Functions.EndTile);
                curTile.type = type;
                curTile.sprite = Functions.EndTile.sprite;
                Tiles[pos] = curTile;
            }
        }
        else {
            //If there is no key, creates a new tile
            Tile myTile;
            if(type == TileType.Wall) {
                myTile = Functions.WallTile;
            }
            else if(type == TileType.Room){
                myTile = Functions.FloorTiles[0];
            }
            else {
                myTile = Functions.FloorTiles[3];
            }
            Map.SetTile(new Vector3Int(pos.x, pos.y, 0), myTile);
            TileTraits newTile;
            newTile.x = pos.x;
            newTile.y = pos.y;
            //Sprite assignment should depend on floor type
            newTile.sprite = myTile.sprite;
            newTile.type = type;
            newTile.number = tileCount;
            Tiles.Add(pos, newTile);
            tileCount++;
        }
    }
    //These next 2 functions Can be called by anything when a new object is instantiated to make it the only active gameobject
    public void AddToMakers(GameObject obj) {
        if(MakerStack.Count > 0) {
            GameObject curRunning = MakerStack.Peek();
            curRunning.SetActive(false);
        }
        MakerStack.Push(obj);
    }
    public void RemoveFromMakers() {
        MakerStack.Pop();
        if(MakerStack.Count > 0) {
            GameObject newRunning = MakerStack.Peek();
            newRunning.SetActive(true);
            if(newRunning.GetComponent<BaseHallwayMaker>() != null) {
                newRunning.GetComponent<BaseHallwayMaker>().OnReEnable();
            }
        }
    }

    //Checks a tile, and sees if it should turn into a door
    void MakeTileDoor(TileTraits tile) {
        //Only applies to hallway tiles
        if(tile.type == TileType.Hallway) {
            //Checks to see if its adjacent to a room tile
            TileTraits[] tileAdjacency = Functions.CardinalAdjacency(new Vector2Int(tile.x, tile.y));
            int adjacentRoomNum = -1;
            for(int i = 0; i < tileAdjacency.Length; i++) {
                if(tileAdjacency[i].type == TileType.Room) {
                    adjacentRoomNum = i;
                    break;
                }
            }
            //If a room is found this way, you continue
            if(adjacentRoomNum > -1) {
                //Checks if the opposite tile is a hallway and the others are wall (condition needed for door)
                if(tileAdjacency[(adjacentRoomNum + 2)%tileAdjacency.Length].type == TileType.Hallway || tileAdjacency[(adjacentRoomNum + 2)%tileAdjacency.Length].type == TileType.Room) {
                    if(tileAdjacency[(adjacentRoomNum + 1)%tileAdjacency.Length].type == TileType.Wall && tileAdjacency[(adjacentRoomNum + 3)%tileAdjacency.Length].type == TileType.Wall) {
                        //Chance to Become a door
                        float rnd = Random.Range(0f, 1f);
                        if(rnd < Functions.DoorAtRoomEndChance) {
                            tile.type = TileType.Door;
                            Map.SetTile(new Vector3Int(tile.x, tile.y, 0), Functions.DoorTile);
                            tile.sprite = Functions.DoorTile.sprite;
                            Tiles[new Vector2Int(tile.x, tile.y)] = tile;
                        }
                    }
                }
            }
        }
    }
}
