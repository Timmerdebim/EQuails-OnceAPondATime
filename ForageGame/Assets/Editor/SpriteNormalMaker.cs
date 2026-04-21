using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Generates a grayscale distance map from a sprite.
/// Each pixel's value (0–1) represents its normalised distance to the nearest
/// transparent pixel (alpha == 0).  Fully-transparent pixels map to 0 and the
/// pixel that is furthest from any transparent pixel maps to 1.
///
/// Algorithm: two-pass linear-time approximation (Meijster / Saito-Toriwaki).
/// For exact results on large textures this is far faster than a naïve O(n²)
/// search while still being dead-simple to read.
/// </summary>
public class DistanceMapGenerator : EditorWindow
{
    [MenuItem("Tools/Normal Map Generator")]
    static void UseTool()
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
            EditorUtility.DisplayProgressBar("Creating Normals", $"Processing {Path.GetFileName(path)}...", (float)processed / selectedPaths.Count);

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

            if (CreateNormalTexture(tex, outputPath))
                processed++;
            else
                failed++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Complete",
            $"Created {processed} normal sprites.\nFailed: {failed}\n\nOutput folder: {folderName}",
            "OK");
    }

    private static bool CreateNormalTexture(Texture2D tex, string savePath)
    {
        try
        {
            int w = tex.width;
            int h = tex.height;
            Color[] pixels = tex.GetPixels();

            // Build boolean transparency mask:
            bool[] isTransparent = GetIsTransparent(tex);

            // Compute squared distances from the nearest transparent pixel
            float[] dist = TwoPassDistances(isTransparent, w, h);
            for (int i = 0; i < dist.Length; i++)
                if (dist[i] > 0) dist[i] = Mathf.Sqrt(dist[i]);

            // Normalise to [0, 1]
            float max = dist.Max();
            for (int i = 0; i < dist.Length; i++)
                dist[i] = dist[i] / max;

            // Spherize
            for (int i = 0; i < dist.Length; i++)
                if (dist[i] > 0) dist[i] = Mathf.Sqrt(1 - (1 - dist[i]) * (1 - dist[i]));

            // Write to pixels
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color(dist[i], dist[i], dist[i], 1f);

            // Create and save the new texture
            Texture2D output = new(w, h, TextureFormat.RGBA32, false);
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

    // =========================================================================
    // Algorithm – Fast 2-pass Euclidean distance transform
    // Based on Meijster et al. "A General Algorithm for Computing Distance
    // Transforms in Linear Time." Produces exact Euclidean squared distances.
    // O(n) time, O(n) space.
    // =========================================================================
    private static float[] TwoPassDistances(bool[] transparent, int w, int h)
    {
        const float INF = 1e9f;

        // ---- Phase 1: vertical 1-D distance pass ----
        float[] g = new float[w * h];

        for (int x = 0; x < w; x++)
        {
            // Forward scan
            g[0 * w + x] = transparent[0 * w + x] ? 0f : INF;
            for (int y = 1; y < h; y++)
            {
                int i = y * w + x;
                g[i] = transparent[i] ? 0f : 1f + g[(y - 1) * w + x];
            }
            // Backward scan
            for (int y = h - 2; y >= 0; y--)
            {
                int i = y * w + x;
                float below = 1f + g[(y + 1) * w + x];
                if (below < g[i]) g[i] = below;
            }
        }

        // ---- Phase 2: horizontal 1-D parabola envelope pass ----
        float[] sqDist = new float[w * h];
        int[] s = new int[w];
        int[] t = new int[w];

        for (int y = 0; y < h; y++)
        {
            int q = 0;
            s[0] = 0;
            t[0] = 0;

            // Build lower envelope of parabolas
            for (int x = 1; x < w; x++)
            {
                float gy = g[y * w + x];
                while (q >= 0 && F(t[q], s[q], g[y * w + s[q]]) > F(t[q], x, gy))
                    q--;
                q++;
                s[q] = x;
                t[q] = (q == 0)
                    ? 0
                    : (int)((x * x - s[q - 1] * s[q - 1]
                           + gy * gy - g[y * w + s[q - 1]] * g[y * w + s[q - 1]])
                           / (2f * (x - s[q - 1]))) + 1;
            }

            // Read off squared distances
            q = 0;
            for (int x = 0; x < w; x++)
            {
                while (q < w - 1 && t[q + 1] <= x) q++;
                float gsi = g[y * w + s[q]];
                float dx = x - s[q];
                sqDist[y * w + x] = dx * dx + gsi * gsi;
            }
        }

        return sqDist;
    }

    // Parabola evaluation helper for 2-pass algorithm
    private static float F(int x, int i, float gi)
    {
        float d = x - i;
        return d * d + gi * gi;
    }


    static bool[] GetIsTransparent(Texture2D src)
    {
        // Find largest transparent area; set all other points to be non-transparent
        Color[] pixels = src.GetPixels();
        int width = src.width;
        int height = src.height;

        // Step 1: Create alpha mask based on alpha < 0.9f
        bool[] isTransparent = new bool[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
            isTransparent[i] = pixels[i].a <= 0.5f;

        // Step 2: Find the largest connected component of transparent pixels
        int[] labels = new int[pixels.Length];
        for (int i = 0; i < labels.Length; i++) labels[i] = -1;

        int currentLabel = 0;
        Dictionary<int, int> componentSizes = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (!isTransparent[index] || labels[index] != -1) continue;

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
                            if (isTransparent[neighborIndex] && labels[neighborIndex] == -1)
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

        // Make everything not part of the largest group solid
        for (int i = 0; i < pixels.Length; i++)
        {
            if (isTransparent[i] && labels[i] != largestLabel)
                isTransparent[i] = false;
        }
        return isTransparent;
    }
}