using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private List<TilesDatas> tileDatas;

    private Dictionary<TileBase,TilesDatas> dataFromTiles;


    // Start is called before the first frame update
    void Awake()
    {
        dataFromTiles = new Dictionary<TileBase,TilesDatas>();
        foreach (var tileData in tileDatas )
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public AudioClip GetCurrentFloorClip(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);

        if (tile == null || !dataFromTiles.ContainsKey(tile))
        {
            Debug.LogWarning("Không tìm thấy Tile hoặc Tile không có dữ liệu trong Dictionary.");
            return null; 
        }

        if (dataFromTiles[tile].clip.Length == 0)
        {
            Debug.LogWarning("Tile không có âm thanh tương ứng.");
            return null;
        }
        int index = Random.Range(0, dataFromTiles[tile].clip.Length);
        AudioClip currentFloorClip = dataFromTiles[tile].clip[index];

        Debug.Log("Grid Position: " + gridPosition);
        Debug.Log("Tile: " + tile);
        Debug.Log("Clip: " + currentFloorClip);

        return currentFloorClip;
    }
}
