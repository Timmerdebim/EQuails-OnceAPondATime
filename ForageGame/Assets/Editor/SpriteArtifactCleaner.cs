using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpriteArtifactCleaner : EditorWindow
{
    [MenuItem("Tools/Fix Sprite Artifacts")]
    static void FixSelectedSprites()
    {
        // Get all selected assets
        List<string> selectedPaths = new List<string>();
        foreach (string guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            selectedPaths.Add(path);
        }

        if (selectedPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No sprites selected.", "OK");
            return;
        }

        // Ask for folder name
        string folderName = EditorUtility.SaveFolderPanel("Choose Output Folder", "Assets", "FixedSprites");
        if (string.IsNullOrEmpty(folderName))
            return;

        // Convert to relative path
        if (folderName.StartsWith(Application.dataPath))
        {
            folderName = "Assets" + folderName.Substring(Application.dataPath.Length);
        }

        // Create folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(folderName))
        {
            string[] folders = folderName.Split('/');
            string currentPath = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }

        int processed = 0;
        int failed = 0;

        foreach (string path in selectedPaths)
        {
            EditorUtility.DisplayProgressBar("Fixing Sprites", $"Processing {Path.GetFileName(path)}...", (float)processed / selectedPaths.Count);

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                failed++;
                continue;
            }

            // Ensure texture is readable
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            // Get original filename without extension
            string originalFileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            // Create output path in the new folder
            string outputPath = Path.Combine(folderName, originalFileName + extension);

            if (FixTexture(tex, outputPath))
                processed++;
            else
                failed++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Complete",
            $"Fixed {processed} sprites.\nFailed: {failed}\n\nOutput folder: {folderName}",
            "OK");
    }

    static bool FixTexture(Texture2D src, string savePath)
    {
        try
        {
            Color[] pixels = src.GetPixels();
            int width = src.width;
            int height = src.height;

            // Step 1: Create alpha mask based on alpha < 0.9f
            bool[] isSolid = new bool[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                isSolid[i] = pixels[i].a >= 0.5f;
            }

            // Step 2: Find the largest connected component of solid pixels
            int[] labels = new int[pixels.Length];
            for (int i = 0; i < labels.Length; i++) labels[i] = -1;

            int currentLabel = 0;
            Dictionary<int, int> componentSizes = new Dictionary<int, int>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    if (!isSolid[index] || labels[index] != -1) continue;

                    // Flood fill to find connected component
                    Queue<Vector2Int> queue = new();
                    queue.Enqueue(new Vector2Int(x, y));
                    labels[index] = currentLabel;
                    int size = 1;

                    while (queue.Count > 0)
                    {
                        Vector2Int pos = queue.Dequeue();
                        int currIndex = pos.y * width + pos.x;

                        // Check 4-directional neighbors (up, down, left, right)
                        Vector2Int[] neighbors = new Vector2Int[]
                        {
                            new(pos.x + 1, pos.y),
                            new(pos.x - 1, pos.y),
                            new(pos.x, pos.y + 1),
                            new(pos.x, pos.y - 1)
                        };

                        foreach (Vector2Int neighbor in neighbors)
                        {
                            if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
                            {
                                int neighborIndex = neighbor.y * width + neighbor.x;
                                if (isSolid[neighborIndex] && labels[neighborIndex] == -1)
                                {
                                    labels[neighborIndex] = currentLabel;
                                    size++;
                                    queue.Enqueue(neighbor);
                                }
                            }
                        }
                    }

                    componentSizes[currentLabel] = size;
                    currentLabel++;
                }
            }

            // Find the largest component label
            int largestLabel = -1;
            int largestSize = 0;
            foreach (var kvp in componentSizes)
            {
                if (kvp.Value > largestSize)
                {
                    largestSize = kvp.Value;
                    largestLabel = kvp.Key;
                }
            }

            // If no solid pixels found, just make everything transparent
            if (largestLabel == -1)
            {
                Color[] emptyPixels = new Color[pixels.Length];
                for (int i = 0; i < pixels.Length; i++)
                {
                    emptyPixels[i] = pixels[i];
                    emptyPixels[i].a = 0f;
                }
                pixels = emptyPixels;
            }
            else
            {
                // Step 3: Apply changes - set alpha to 0 for:
                // - Pixels with alpha < 0.9f
                // - Pixels not in the largest connected component
                for (int i = 0; i < pixels.Length; i++)
                {
                    bool shouldKeep = isSolid[i] && labels[i] == largestLabel;
                    if (!shouldKeep)
                    {
                        pixels[i].r = 0f;
                        pixels[i].g = 0f;
                        pixels[i].b = 0f;
                        pixels[i].a = 0f;
                    }
                    else
                        pixels[i].a = 1;
                }
            }

            // Create and save the new texture
            Texture2D output = new Texture2D(width, height, TextureFormat.RGBA32, false);
            output.SetPixels(pixels);
            output.Apply();

            byte[] pngData = output.EncodeToPNG();
            File.WriteAllBytes(savePath, pngData);

            // Clean up
            Object.DestroyImmediate(output);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to fix texture: {e.Message}");
            return false;
        }
    }
}