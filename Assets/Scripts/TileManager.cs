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
}
