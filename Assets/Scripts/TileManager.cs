using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    public Dictionary<Vector2Int, TileTraits> Tiles;
    public int tileCount;
    public Tilemap Map;
    public List<GameObject> Makers;//List of currently running makers (so that when they're all gone, the generation ends)
    public int endgame;
    [SerializeField] private HallwayFunctions Functions;
    void Start()
    {
        Tiles = new Dictionary<Vector2Int, TileTraits>();
        endgame = 0;
    }

    void Update()
    {
        //When the endgame is reached, it generates walls
        if(endgame == 1) {
            if(Makers.Count == 0) {
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
        }
        else {
            //If there is no key, creates a new tile
            Tile myTile;
            if(type == TileType.Wall) {
                myTile = Functions.WallTile;
            }
            else {
                myTile = Functions.FloorTiles[Random.Range(0, Functions.FloorTiles.Count)];
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
}
