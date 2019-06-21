using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridMap : MonoBehaviour {
  
  private HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
  private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
  
  private enum Contour { Full, NE, SE, SW, NW }
  private Dictionary<Vector2Int, Contour> wallContours = new Dictionary<Vector2Int, Contour>();
  
  private TileBase floorTile;
  private TileBase[] wallTiles;
  private TileBase[] shadowTiles;
  
  private Tilemap floor;
  private Tilemap walls;
  private Tilemap decor;
  
  private void BrushPos(Vector2 pos, HashSet<Vector2Int> tiles, double width, bool brush) {
    Vector2[] quadTiles = new Vector2[4];
    Vector2Int tile;
    float yLim;
    for (int x = 0; x < width; x++) {
      yLim = Mathf.Sqrt((float)(width * width - x * x));
      for (int y = 0; y < yLim; y++) {
        quadTiles[0] = new Vector2(x, y);
        quadTiles[1] = new Vector2(-x, y);
        quadTiles[2] = new Vector2(x, -y);
        quadTiles[3] = new Vector2(-x, -y);
        foreach (Vector2 quadTile in quadTiles) {
          tile = Vector2Int.RoundToInt(pos + quadTile);
          if (brush) tiles.Add(tile);
          else tiles.Remove(tile);
        }
      }
    }
  }
  
  private HashSet<NavNode> brushedNodes;
  
  // TODO sep into two to get rid of bool and width for normal
  private void BrushGraph(NavNode node, HashSet<Vector2Int> tiles, double width, bool brush) {
    brushedNodes.Clear();
    BrushGraphRec(node, tiles, width, brush);
  }
  
  private void BrushGraphRec(NavNode node, HashSet<Vector2Int> tiles, double width, bool brush) {
    brushedNodes.Add(node);
    BrushPos(node.pos, tiles, width, brush);
    foreach (KeyValuePair<NavNode, float> link in node.links) {
      if (!brushedNodes.Contains(link.Key)) {
        BrushGraphRec(link.Key, tiles, width, brush);
      }
    }
  }
  
  private void ContourPositions(Dictionary<Vector2Int, Contour> contours, HashSet<Vector2Int> tilePositions) {
    
    bool north, east, south, west;
    int adjCount;
    
    foreach (Vector2Int tilePos in tilePositions) {
      
      adjCount = 0;
      north = tilePositions.Contains(new Vector2Int(tilePos.x, tilePos.y + 1));
      east = tilePositions.Contains(new Vector2Int(tilePos.x + 1, tilePos.y));
      south = tilePositions.Contains(new Vector2Int(tilePos.x, tilePos.y - 1));
      west = tilePositions.Contains(new Vector2Int(tilePos.x - 1, tilePos.y));
      
      if (north) ++adjCount;
      if (east) ++adjCount;
      if (south) ++adjCount;
      if (west) ++adjCount;
      
      if (adjCount >= 3) {
        contours.Add(tilePos, Contour.Full);
      } else if (north && east) {
        contours.Add(tilePos, Contour.SW);
      } else if (east && south) {
        contours.Add(tilePos, Contour.NW);
      } else if (south && west) {
        contours.Add(tilePos, Contour.NE);
      } else if (west && north) {
        contours.Add(tilePos, Contour.SE);
      }
    }
  }
  
  private void MapContours(Tilemap tm, Dictionary<Vector2Int, Contour> contours, TileBase[] tiles) {
    foreach (KeyValuePair<Vector2Int, Contour> contour in contours) {
      tm.SetTile((Vector3Int)contour.Key, tiles[(int)contour.Value]);
    }
  }
  
  private void MapPositions(Tilemap tm, HashSet<Vector2Int> positions, TileBase tile) {
    foreach (Vector2Int pos in positions) {
      tm.SetTile((Vector3Int)pos, tile);
    }
    positions.Clear();
  }
  
  private void MapShadows(Tilemap tm, Dictionary<Vector2Int, Contour> contours, TileBase[] tiles) {
    TileBase tile;
    foreach (KeyValuePair<Vector2Int, Contour> contour in contours) {
      tile = null;
      switch (contour.Value) {
        case Contour.SE:
        tile = tiles[0];
        break;
        case Contour.SW:
        tile = tiles[2];
        break;
        case Contour.Full:
        if (!contours.ContainsKey(contour.Key - Vector2Int.up)) {
          tile = tiles[1];
        }
        break;
      }
      if (tile != null) {
        tm.SetTile((Vector3Int)(contour.Key - Vector2Int.up), tile);
      }
    }
  }
  
  private void Reset() {
    wallPositions.Clear();
    floorPositions.Clear();
    wallContours.Clear();
    floor.ClearAllTiles();
    walls.ClearAllTiles();
    decor.ClearAllTiles();
  }
  
  public void Generate(Map map, NavNode start) {
    
    Reset();
    
    BrushGraph(start, wallPositions, map.trunkWidth + map.wallWidth, true);
    BrushGraph(start, floorPositions, map.trunkWidth + map.wallWidth / 2, true);
    BrushGraph(start, wallPositions, map.trunkWidth, false);
    
    ContourPositions(wallContours, wallPositions);
    
    MapContours(walls, wallContours, map.wallTiles);
    MapPositions(floor, floorPositions, map.floorTile);
    MapShadows(decor, wallContours, map.shadowTiles);
  }
  
  private void Awake() {
    brushedNodes = new HashSet<NavNode>();
    walls = transform.GetChild(0).GetComponent<Tilemap>();
    floor = transform.GetChild(1).GetComponent<Tilemap>();
    decor = transform.GetChild(2).GetComponent<Tilemap>();
  }
}
