using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A struct that keeps track of all the traits of a tile
public struct TileTraits {
    public int x;
    public int y;
    public Sprite sprite;
    public TileType type;
    public int number;//The number tile placed
}

public enum TileType {
    Hallway,
    Room,
    Door,
    Wall,
    End
}
