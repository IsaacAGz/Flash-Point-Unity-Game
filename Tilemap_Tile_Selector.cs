using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Tilemap_Tile_Selector : MonoBehaviour
{

    public Camera cam;
    public Grid grid;
    public Tilemap[] tilemaps;
    public Tilemap highlightMap;
    public Tile highlightTile;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //constantly checking if mouse button 0 was clicked
        if (!Input.GetMouseButtonDown(0)) return;

        
        if (Input.GetMouseButtonDown(0))
        {
            // Get world position from mouse position
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            // Convert world position to cell position
            Vector3Int cellPos = grid.WorldToCell(worldPos);

            //Selects tilemap at clicked tile
            Tilemap pickedMap = PickTileMap(cellPos);
            if (pickedMap == null) return;
            Debug.Log($"Clicked {pickedMap.name} at {cellPos}");
            
            //adds highlight border to clicked on tile
            if (highlightMap && highlightTile)
            {
                highlightMap.ClearAllTiles();
                highlightMap.SetTile(cellPos, highlightTile);
            }
        }
    }

    //Selects first tilemap with that cell
    Tilemap PickTileMap(Vector3Int cell)
    {
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.HasTile(cell))
            {
                return tilemap;
            }
        }
        return null;
    }
}
