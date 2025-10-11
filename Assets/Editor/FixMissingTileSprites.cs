using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FixMissingTileSprites
{
    [MenuItem("Tools/Fix Missing Tile Sprites")]
    public static void FixTiles()
    {
        // <-- adapte ce chemin si nécessaire
        string folderPath = "Assets/Floor_Tiles";
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Dossier introuvable : {folderPath}. Vérifie le chemin (doit commencer par 'Assets/').");
            return;
        }

        string[] assetPaths = Directory.GetFiles(folderPath, "*.asset", SearchOption.AllDirectories);

        int fixedCount = 0;
        int missingCount = 0;

        foreach (string sysPath in assetPaths)
        {
            // Normalisation du chemin pour Unity (forward slashes)
            string assetPath = sysPath.Replace("\\", "/");

            // Convertir en path relatif "Assets/..." si nécessaire
            int assetsIndex = assetPath.IndexOf("Assets/");
            if (assetsIndex >= 0)
                assetPath = assetPath.Substring(assetsIndex);
            else
            {
                Debug.LogWarning($"Ignoré (chemin invalide) : {assetPath}");
                continue;
            }

            // Charge l'asset (n'importe quel type d'asset)
            Object tileAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (tileAsset == null)
            {
                Debug.LogWarning($"Impossible de charger l'asset : {assetPath}");
                continue;
            }

            // Vérifie si il y a déjà un sprite assigné (propriété m_Sprite)
            var so = new SerializedObject(tileAsset);
            var spriteProp = so.FindProperty("m_Sprite");
            if (spriteProp == null)
            {
                Debug.Log($"L'asset n'a pas de propriété 'm_Sprite' (probablement pas une Tile standard) : {assetPath}");
                continue;
            }

            if (spriteProp.objectReferenceValue != null)
            {
                // déjà assigné
                continue;
            }

            // tentative 1 : cherche un PNG du même nom à côté (.asset -> .png)
            string pngPathCandidate = Path.ChangeExtension(assetPath, ".png");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPathCandidate);

            // tentative 2 : si pas trouvé, cherche par nom de fichier dans tout le projet
            if (sprite == null)
            {
                string baseName = Path.GetFileNameWithoutExtension(assetPath);
                // recherche par nom (type:Sprite)
                string[] guids = AssetDatabase.FindAssets($"t:Sprite {baseName}");
                if (guids != null && guids.Length > 0)
                {
                    // prend le premier résultat qui correspond le mieux (tu peux améliorer la logique si besoin)
                    string foundPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(foundPath);
                }
            }

            if (sprite != null)
            {
                spriteProp.objectReferenceValue = sprite;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(tileAsset);
                fixedCount++;
                Debug.Log($"Réassigné : {Path.GetFileName(assetPath)} → {AssetDatabase.GetAssetPath(sprite)}");
            }
            else
            {
                missingCount++;
                Debug.LogWarning($"Sprite introuvable pour : {assetPath} (essayé {pngPathCandidate} et recherche par nom)");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Terminé. Réassignés : {fixedCount}. Introuvables : {missingCount}.");
    }
}
