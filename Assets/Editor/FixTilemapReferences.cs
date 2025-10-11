using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

public class AutoRepairTilemap
{
    [MenuItem("Tools/Auto Repair Tilemaps")]
    public static void RepairTilemaps()
    {
        // Dossier contenant tes nouveaux Tiles
        string folderPath = "Assets/Floor_Tiles"; // adapte si nécessaire
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Dossier introuvable : {folderPath}");
            return;
        }

        // Récupère tous les Tiles dans ce dossier et crée un dictionnaire nom -> Tile
        string[] assetPaths = Directory.GetFiles(folderPath, "*.asset", SearchOption.AllDirectories);
        Dictionary<string, TileBase> tileLookup = new Dictionary<string, TileBase>();

        foreach (string path in assetPaths)
        {
            TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            if (tile != null)
            {
                string tileName = tile.name;
                if (!tileLookup.ContainsKey(tileName))
                    tileLookup.Add(tileName, tile);
            }
        }

        if (tileLookup.Count == 0)
        {
            Debug.LogWarning("Aucun Tile trouvé dans le dossier. Vérifie que les Sprites sont bien assignés.");
            return;
        }

        // Parcourt toutes les Tilemaps de la scène
        Tilemap[] tilemaps = GameObject.FindObjectsOfType<Tilemap>();
        Debug.Log($"🗺️ Nombre de Tilemaps trouvées dans la scène : {tilemaps.Length}");

        int replacedCount = 0;

        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            int tileCount = 0;

            // Compte toutes les tiles existantes dans cette Tilemap
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(pos);
                    if (tile != null) tileCount++;
                }
            }

            Debug.Log($"📌 Tilemap '{tilemap.name}' contient {tileCount} tiles.");

            // Remplacement automatique
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase oldTile = tilemap.GetTile(pos);

                    if (oldTile == null) continue; // pas de tile, rien à faire

                    // Remplace seulement si le Sprite est manquant (case rose)
                    if ((oldTile is Tile t && t.sprite == null) || oldTile.name == "Tile") // "Tile" générique
                    {
                       
                        if (tileLookup.TryGetValue(oldTile.name, out TileBase newTile))
                        {
                            tilemap.SetTile(pos, newTile);
                            replacedCount++;
                        }
                    }
                }
            }

            EditorUtility.SetDirty(tilemap);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Auto repair terminé : {replacedCount} tiles remplacées.");
    }
}
