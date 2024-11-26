using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum FloorType
{
    Grass,
    Stone,
    Dirt,
    Carpet,
    Wood,
    Tiling,
    Snow,
    Gravel

}

[CreateAssetMenu]
public class TilesDatas : ScriptableObject
{
    public TileBase[] tiles;
    public AudioClip[] clip;
    public FloorType floorType;
}
