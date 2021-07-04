using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A struct that keeps track of all the traits of a tile
public struct TileTraits {
    public int x;
    public int y;
    public Sprite sprite;
    public TileType type;
}

public enum TileType {
    Hallway,
    Room,
    Wall
}
