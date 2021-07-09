using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Used to control the player character once the map-building is over
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private KeyCode LeftMove;
    [SerializeField] private KeyCode RightMove;
    [SerializeField] private KeyCode UpMove;
    [SerializeField] private KeyCode DownMove;
    void Start()
    {
        
    }

    void Update()
    {   //Only does anything if the game state is the Playing state
        if(TileManager.Instance.CurGameState == GameState.Playing) {
            //does movement code
            if(Input.GetKeyDown(LeftMove)) {
                TileTraits futTile = TileManager.Instance.Tiles[GeneralFunctions.Vec3ToVec2Int(transform.position + new Vector3(-1, 0, 0))];
                if(futTile.type != TileType.Wall) {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    transform.position += transform.up;
                }
            }
            else if(Input.GetKeyDown(RightMove)) {
                TileTraits futTile = TileManager.Instance.Tiles[GeneralFunctions.Vec3ToVec2Int(transform.position + new Vector3(1, 0, 0))];
                if(futTile.type != TileType.Wall) {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    transform.position += transform.up;
                }
            }
            else if(Input.GetKeyDown(UpMove)) {
                TileTraits futTile = TileManager.Instance.Tiles[GeneralFunctions.Vec3ToVec2Int(transform.position + new Vector3(0, 1, 0))];
                if(futTile.type != TileType.Wall) {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    transform.position += transform.up;
                }
            }
            else if(Input.GetKeyDown(DownMove)) {
                TileTraits futTile = TileManager.Instance.Tiles[GeneralFunctions.Vec3ToVec2Int(transform.position + new Vector3(0, -1, 0))];
                if(futTile.type != TileType.Wall) {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    transform.position += transform.up;
                }
            }
        }
    }
}
