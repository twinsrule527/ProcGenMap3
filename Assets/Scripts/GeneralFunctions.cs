using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Full of static conversions and functions
public class GeneralFunctions
{
    public static Vector2Int Vec3ToVec2Int(Vector3 vec) {
        Vector2Int newVec = new Vector2Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
        return newVec;
    }
}
