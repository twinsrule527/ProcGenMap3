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

    [SerializeField] private Vector3 offsetFromTiles;//how much the player should be offset from an integer value to fit onto the tiles and appear on the camera
    void Start()
    {
        
    }

    void Update()
    {   //Only does anything if the game state is the Playing state
        if(TileManager.Instance.CurGameState == GameState.Playing) {
            //does movement code
            if(Input.GetKeyDown(LeftMove)) {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                Move(GeneralFunctions.Vec3ToVec2Int(transform.position), GeneralFunctions.Vec3ToVec2Int(transform.position + transform.up));
            }
            else if(Input.GetKeyDown(RightMove)) {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                Move(GeneralFunctions.Vec3ToVec2Int(transform.position), GeneralFunctions.Vec3ToVec2Int(transform.position + transform.up));
            }
            else if(Input.GetKeyDown(UpMove)) {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                Move(GeneralFunctions.Vec3ToVec2Int(transform.position), GeneralFunctions.Vec3ToVec2Int(transform.position + transform.up));
            }
            else if(Input.GetKeyDown(DownMove)) {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                Move(GeneralFunctions.Vec3ToVec2Int(transform.position), GeneralFunctions.Vec3ToVec2Int(transform.position + transform.up));
            }
        }
    }
    //Given the current position of the player and where they're trying to move to, the game attempts to move them
    private void Move(Vector2Int posFrom, Vector2Int posTo) {
        TileTraits MoveToTile = TileManager.Instance.Tiles[posTo];
        //if they're not trying to move onto a wall, they succeed
        if(MoveToTile.type != TileType.Wall) {
            transform.position = new Vector3(posTo.x, posTo.y, 0) + offsetFromTiles;
            //Then, if they've reached the exit, give debug message
            if(MoveToTile.type == TileType.End) {
                Debug.Log("GameEnd");
            }
        }
    }
}
