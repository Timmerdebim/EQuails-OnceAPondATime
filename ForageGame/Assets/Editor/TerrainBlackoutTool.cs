// Place this file in any folder named "Editor" (e.g. Assets/Editor/TerrainBlackoutTool.cs).
// Open via  Tools > Terrain > Blackout Under Objects
//
// What it does
//   For every terrain sample that lies inside one of your target objects it can:
//     - paint the terrain black (adds/uses a black TerrainLayer and paints it at 100% weight)
//     - clear grass / detail meshes
//     - remove trees
//
// Targets can be
//   - scene objects (list and/or layers), minus anything on the Exclude Layers
//   - terrain tree instances (e.g. a "tree" prototype that is really a rock)
//
// How "inside" is decided (Collider source)
//   For each terrain column (x,z) two rays are cast against the target's collider:
//   one from above (finds the top surface) and one from below (finds the bottom surface).
//   Volume mode   : the terrain surface height must lie between bottom and top.
//   Footprint mode: any column the object covers when seen from above counts.
//   This works for box/sphere/capsule AND non-convex mesh colliders, because only front
//   faces are ever hit.
//
// Two different masks are produced from that test
//   - Details / tree removal : the object mask grown by Padding (m).
//   - Black colour           : the strict object mask (no padding, no vertical tolerance),
//                              pulled inwards so the paint - including its fade - never
//                              reaches past the object's edge.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TerrainTools
{
    public class TerrainBlackoutTool : EditorWindow
    {
        public enum GeometrySource { Colliders, RendererBounds }
        public enum Detection { Volume, Footprint }

        [SerializeField] Terrain terrain;
        [SerializeField] List<GameObject> targetObjects = new List<GameObject>();
        [SerializeField] LayerMask extraLayers = 0;
        [SerializeField] LayerMask excludeLayers = 0;
        [SerializeField] GeometrySource source = GeometrySource.Colliders;
        [SerializeField] Detection detection = Detection.Volume;
        [SerializeField] bool ignoreTriggers = true;
        [SerializeField] bool useTerrainTreesAsTargets = false;
        [SerializeField] List<GameObject> treeTargetPrefabs = new List<GameObject>();
        [SerializeField] float padding = 0.25f;
        [SerializeField] float colourInset = 0f;
        [SerializeField] float feather = 0f;
        [SerializeField] bool paintBlack = true;
        [SerializeField] bool removeDetails = true;
        [SerializeField] bool removeTrees = false;
        [SerializeField] string assetFolder = "Assets/TerrainBlackout";

        SerializedObject so;
        Vector2 scroll;
        string lastResult;

        [MenuItem("Tools/Terrain/Blackout Under Objects")]
        static void Open() => GetWindow<TerrainBlackoutTool>("Terrain Blackout");

        void OnEnable()
        {
            so = new SerializedObject(this);
            if (terrain == null) terrain = Terrain.activeTerrain;
        }

        // ------------------------------------------------------------------ GUI

        void OnGUI()
        {
            so.Update();
            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUILayout.LabelField("Terrain", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(terrain)));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(source)));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(detection)));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(targetObjects)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(extraLayers)),
                new GUIContent("Also Include Layers", "Every active object on these layers in the open scene is added too."));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(excludeLayers)),
                new GUIContent("Exclude Layers", "Objects on these layers are never used as targets. Overrides the object list, the include layers and terrain trees."));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(ignoreTriggers)));
            so.ApplyModifiedProperties();

            // Immediate-mode section (edits fields directly), then re-sync the SerializedObject.
            DrawTreeTargets();
            so.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Details / Tree Removal Mask", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(padding)),
                new GUIContent("Padding (m)", "Grows the area used for clearing details and removing trees (and adds vertical tolerance in Volume mode). Does not affect the black paint."));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Black Paint (stays inside the object)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(colourInset)),
                new GUIContent("Colour Inset (m)", "Extra gap between the object's edge and the black paint. The paint is already kept at least one alphamap cell inside the edge."));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(feather)),
                new GUIContent("Colour Feather (m)", "Fade distance measured inwards, starting where the black begins. The fade never extends outside the object. 0 = hard edge."));
            //colourInset = Mathf.Max(0f, colourInset);
            feather = Mathf.Max(0f, feather);

            if (paintBlack && terrain != null && terrain.terrainData != null)
            {
                TerrainData td = terrain.terrainData;
                EditorGUILayout.HelpBox(
                    $"Alphamap cell size: {td.size.x / td.alphamapWidth:0.00} x {td.size.z / td.alphamapHeight:0.00} m. " +
                    "Black paint always stays at least one cell inside the object's edge, so very small objects " +
                    "(only a few cells across) get little or no black. Raise the terrain's Control Texture Resolution for finer results.",
                    MessageType.None);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(paintBlack)));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(removeDetails)));
            EditorGUILayout.PropertyField(so.FindProperty(nameof(removeTrees)));
            if (paintBlack)
                EditorGUILayout.PropertyField(so.FindProperty(nameof(assetFolder)),
                    new GUIContent("Black Layer Folder"));

            so.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Use Active Terrain")) terrain = Terrain.activeTerrain;
            if (GUILayout.Button("Add Selected Objects"))
            {
                foreach (var go in Selection.gameObjects)
                    if (go.GetComponent<Terrain>() == null && !targetObjects.Contains(go))
                        targetObjects.Add(go);
                so.Update();
            }
            EditorGUILayout.EndHorizontal();

            if (source == GeometrySource.RendererBounds)
                EditorGUILayout.HelpBox("Renderer Bounds uses axis-aligned boxes: fast, but coarse for rotated or irregular objects.", MessageType.Info);
            else if (detection == Detection.Volume)
                EditorGUILayout.HelpBox("Volume mode assumes reasonably closed shapes. For open meshes (planes, tents without a floor) use Footprint.", MessageType.None);

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(terrain == null))
            {
                if (GUILayout.Button("Apply", GUILayout.Height(30))) Apply();
            }

            if (!string.IsNullOrEmpty(lastResult))
                EditorGUILayout.HelpBox(lastResult, MessageType.Info);

            EditorGUILayout.EndScrollView();
        }

        void DrawTreeTargets()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Terrain Trees as Targets", EditorStyles.boldLabel);
            useTerrainTreesAsTargets = EditorGUILayout.ToggleLeft(
                new GUIContent("Use terrain tree instances as targets",
                    "Treat placed terrain trees (e.g. rocks) like objects: the terrain under them is blackened / cleared."),
                useTerrainTreesAsTargets);

            if (!useTerrainTreesAsTargets) return;
            if (terrain == null || terrain.terrainData == null) return;

            TreePrototype[] prototypes = terrain.terrainData.treePrototypes;
            if (prototypes.Length == 0)
            {
                EditorGUILayout.HelpBox("This terrain has no tree prototypes.", MessageType.None);
                return;
            }

            EditorGUI.indentLevel++;
            foreach (TreePrototype prototype in prototypes)
            {
                GameObject prefab = prototype.prefab;
                if (prefab == null) continue;

                bool on = treeTargetPrefabs.Contains(prefab);
                bool now = EditorGUILayout.ToggleLeft(prefab.name, on);
                if (now && !on) treeTargetPrefabs.Add(prefab);
                else if (!now && on) treeTargetPrefabs.Remove(prefab);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.HelpBox(
                "Tick the tree prototypes that should act as objects. Ticked prototypes are never removed by 'Remove Trees'. " +
                "Their prefab's colliders are used if it has any, otherwise a temporary mesh collider is built from the LOD0 mesh. " +
                "For rocks sitting on the ground, Footprint mode usually gives the cleanest result.",
                MessageType.None);
        }

        // ---------------------------------------------------------------- Apply

        void Apply()
        {
            if (terrain == null || terrain.terrainData == null) return;
            if (!paintBlack && !removeDetails && !removeTrees)
            {
                lastResult = "Nothing to do: enable at least one action.";
                return;
            }

            TerrainData td = terrain.terrainData;
            Physics.SyncTransforms(); // make sure collider transforms match the scene in edit mode

            List<Target> targets = GatherTargets();
            TreeTargets trees = null;
            int painted = 0, detailCells = 0, treesRemoved = 0;

            try
            {
                if (useTerrainTreesAsTargets)
                {
                    trees = new TreeTargets(terrain, treeTargetPrefabs, source == GeometrySource.Colliders,
                                            excludeLayers, ignoreTriggers);
                    if (trees.PrototypeCount == 0)
                    {
                        trees.Dispose();
                        trees = null;
                    }
                }

                if (targets.Count == 0 && trees == null)
                {
                    EditorUtility.DisplayDialog("Terrain Blackout",
                        "No targets found. Add objects, pick layers, or tick terrain tree prototypes (and check the Exclude Layers).", "OK");
                    return;
                }

                // Build every mask first so cancelling leaves the terrain untouched.
                MaskGrid alphaGrid = null;
                bool[,] treeMask = null;
                if (paintBlack || removeTrees)
                {
                    alphaGrid = BuildGrid(td.alphamapWidth, td.alphamapHeight, targets, trees, "Alphamap");
                    if (alphaGrid == null) return; // cancelled
                    if (removeTrees) treeMask = alphaGrid.PaddedMask(padding);
                }

                bool[,] detailMask = null;
                if (removeDetails && td.detailPrototypes.Length > 0)
                {
                    MaskGrid detailGrid = BuildGrid(td.detailWidth, td.detailHeight, targets, trees, "Details");
                    if (detailGrid == null) return; // cancelled
                    detailMask = detailGrid.PaddedMask(padding);
                }

                Undo.RegisterCompleteObjectUndo(td, "Terrain Blackout");

                if (paintBlack) painted = PaintBlack(td, alphaGrid);
                if (removeTrees) treesRemoved = RemoveTrees(td, treeMask, trees);
                if (detailMask != null) detailCells = RemoveDetails(td, detailMask);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                trees?.Dispose();
            }

            EditorUtility.SetDirty(td);
            AssetDatabase.SaveAssets();
            terrain.Flush();

            lastResult = $"Done. Painted {painted} alphamap cells black, cleared {detailCells} detail cells, removed {treesRemoved} trees.";
            Debug.Log("[TerrainBlackout] " + lastResult);
        }

        // -------------------------------------------------------------- Targets

        sealed class Target
        {
            public readonly string Name;
            public readonly Bounds WorldBounds;
            readonly Collider collider; // null => use the bounds themselves

            public Target(string name, Bounds bounds, Collider collider)
            {
                Name = name;
                WorldBounds = bounds;
                this.collider = collider;
            }

            /// <summary>
            /// Finds the vertical extent of the object at world column (x,z).
            /// Returns false if the object does not cover that column.
            /// </summary>
            public bool TryGetColumn(float x, float z, bool needBottom, out float yMin, out float yMax)
            {
                yMin = WorldBounds.min.y;
                yMax = WorldBounds.max.y;

                if (x < WorldBounds.min.x || x > WorldBounds.max.x ||
                    z < WorldBounds.min.z || z > WorldBounds.max.z)
                    return false;

                if (collider == null) return true;

                float length = WorldBounds.size.y + 2f;

                // Top surface: ray from above, pointing down.
                var down = new Ray(new Vector3(x, WorldBounds.max.y + 1f, z), Vector3.down);
                if (!collider.Raycast(down, out RaycastHit topHit, length)) return false;
                yMax = topHit.point.y;

                // Bottom surface: ray from below, pointing up. If it misses (open-bottomed mesh),
                // assume the object is solid down to its bounds.
                if (needBottom)
                {
                    var up = new Ray(new Vector3(x, WorldBounds.min.y - 1f, z), Vector3.up);
                    if (collider.Raycast(up, out RaycastHit bottomHit, length))
                        yMin = bottomHit.point.y;
                }
                return true;
            }
        }

        List<Target> GatherTargets()
        {
            var result = new List<Target>();
            var seen = new HashSet<Object>();

            bool Excluded(GameObject go) => (excludeLayers.value & (1 << go.layer)) != 0;

            void AddCollider(Collider c)
            {
                if (c == null || c is TerrainCollider) return;
                if (!c.enabled || !c.gameObject.activeInHierarchy) return;
                if (Excluded(c.gameObject)) return;
                if (ignoreTriggers && c.isTrigger) return;
                if (!seen.Add(c)) return;
                result.Add(new Target(c.name, c.bounds, c));
            }

            void AddRenderer(Renderer r)
            {
                if (r == null || !r.enabled || !r.gameObject.activeInHierarchy) return;
                if (Excluded(r.gameObject)) return;
                if (!seen.Add(r)) return;
                result.Add(new Target(r.name, r.bounds, null));
            }

            foreach (var go in targetObjects)
            {
                if (go == null) continue;
                if (source == GeometrySource.Colliders)
                    foreach (var c in go.GetComponentsInChildren<Collider>(false)) AddCollider(c);
                else
                    foreach (var r in go.GetComponentsInChildren<Renderer>(false)) AddRenderer(r);
            }

            if (extraLayers.value != 0)
            {
                if (source == GeometrySource.Colliders)
                {
                    foreach (var c in FindObjectsByType<Collider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                        if ((extraLayers.value & (1 << c.gameObject.layer)) != 0) AddCollider(c);
                }
                else
                {
                    foreach (var r in FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                        if ((extraLayers.value & (1 << r.gameObject.layer)) != 0) AddRenderer(r);
                }
            }

            return result;
        }

        // ---------------------------------------------------- Terrain tree targets

        /// <summary>
        /// Turns terrain tree instances into targets. One hidden temporary copy of each selected
        /// prototype prefab is moved onto every instance in turn (position, Y rotation, scale),
        /// yielding a Target for the current placement. Call Dispose() to clean up.
        /// </summary>
        sealed class TreeTargets : System.IDisposable
        {
            sealed class Proto
            {
                public GameObject Root;
                public Collider[] Colliders;
                public Renderer[] Renderers;
                public Vector3 BaseScale;
                public Quaternion BaseRotation;
            }

            readonly Terrain terrain;
            readonly bool useColliders;
            readonly Dictionary<int, Proto> protos = new Dictionary<int, Proto>();

            public int InstanceCount { get; private set; }
            public int PrototypeCount => protos.Count;
            public bool Contains(int prototypeIndex) => protos.ContainsKey(prototypeIndex);

            public TreeTargets(Terrain terrain, IList<GameObject> selectedPrefabs, bool useColliders,
                               LayerMask exclude, bool ignoreTriggers)
            {
                this.terrain = terrain;
                this.useColliders = useColliders;

                TerrainData td = terrain.terrainData;
                TreePrototype[] prototypes = td.treePrototypes;

                for (int i = 0; i < prototypes.Length; i++)
                {
                    GameObject prefab = prototypes[i].prefab;
                    if (prefab == null || !selectedPrefabs.Contains(prefab)) continue;
                    if ((exclude.value & (1 << prefab.layer)) != 0) continue;

                    Proto proto = BuildProto(prefab, useColliders, ignoreTriggers);
                    if (proto != null) protos[i] = proto;
                }

                foreach (TreeInstance tree in td.treeInstances)
                    if (protos.ContainsKey(tree.prototypeIndex)) InstanceCount++;
            }

            static Proto BuildProto(GameObject prefab, bool useColliders, bool ignoreTriggers)
            {
                GameObject go = Object.Instantiate(prefab);
                go.name = "~TerrainBlackoutTemp_" + prefab.name;
                foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
                    t.gameObject.hideFlags = HideFlags.HideAndDontSave;

                var proto = new Proto
                {
                    Root = go,
                    BaseScale = prefab.transform.localScale,
                    BaseRotation = prefab.transform.rotation,
                    Renderers = GetLod0Renderers(go),
                    Colliders = new Collider[0]
                };

                if (useColliders)
                {
                    var colliders = new List<Collider>();
                    foreach (Collider c in go.GetComponentsInChildren<Collider>())
                        if (c.enabled && !(ignoreTriggers && c.isTrigger)) colliders.Add(c);

                    // No usable collider on the prefab: build temporary mesh colliders from the visible mesh.
                    if (colliders.Count == 0)
                    {
                        foreach (Renderer r in proto.Renderers)
                        {
                            var filter = r.GetComponent<MeshFilter>();
                            if (filter == null || filter.sharedMesh == null) continue;
                            var mc = r.gameObject.AddComponent<MeshCollider>();
                            mc.sharedMesh = filter.sharedMesh;
                            colliders.Add(mc);
                        }
                    }

                    proto.Colliders = colliders.ToArray();
                    if (proto.Colliders.Length == 0)
                    {
                        DestroyImmediate(go);
                        return null;
                    }
                }
                else if (proto.Renderers.Length == 0)
                {
                    DestroyImmediate(go);
                    return null;
                }

                return proto;
            }

            static Renderer[] GetLod0Renderers(GameObject go)
            {
                var lodGroup = go.GetComponentInChildren<LODGroup>();
                if (lodGroup != null)
                {
                    LOD[] lods = lodGroup.GetLODs();
                    if (lods.Length > 0 && lods[0].renderers != null)
                    {
                        var list = new List<Renderer>();
                        foreach (Renderer r in lods[0].renderers)
                            if (r != null) list.Add(r);
                        if (list.Count > 0) return list.ToArray();
                    }
                }
                return go.GetComponentsInChildren<Renderer>();
            }

            /// <summary>Places the temporary prefab copies on each instance and yields their targets.</summary>
            public IEnumerable<Target> Instances()
            {
                TerrainData td = terrain.terrainData;
                Vector3 origin = terrain.transform.position;

                foreach (TreeInstance tree in td.treeInstances)
                {
                    if (!protos.TryGetValue(tree.prototypeIndex, out Proto proto)) continue;

                    Vector3 position = origin + Vector3.Scale(tree.position, td.size);
                    Quaternion yaw = Quaternion.Euler(0f, tree.rotation * Mathf.Rad2Deg, 0f);

                    Transform t = proto.Root.transform;
                    t.SetPositionAndRotation(position, yaw * proto.BaseRotation);
                    t.localScale = new Vector3(proto.BaseScale.x * tree.widthScale,
                                               proto.BaseScale.y * tree.heightScale,
                                               proto.BaseScale.z * tree.widthScale);

                    if (useColliders)
                    {
                        Physics.SyncTransforms();
                        foreach (Collider c in proto.Colliders)
                            yield return new Target(proto.Root.name, c.bounds, c);
                    }
                    else
                    {
                        foreach (Renderer r in proto.Renderers)
                            yield return new Target(proto.Root.name, r.bounds, null);
                    }
                }
            }

            public void Dispose()
            {
                foreach (Proto proto in protos.Values)
                    if (proto.Root != null) DestroyImmediate(proto.Root);
                protos.Clear();
            }
        }

        // ----------------------------------------------------------------- Mask

        /// <summary>
        /// A [z, x] grid laid over the terrain at a given resolution. Holds two masks:
        ///   padded : object mask with vertical tolerance (used for details / tree removal, then dilated by Padding)
        ///   strict : exactly what lies inside the object, no tolerance (used for the black colour)
        /// </summary>
        sealed class MaskGrid
        {
            readonly TerrainData td;
            readonly Vector3 origin, size;
            readonly int resX, resZ;
            readonly float cellX, cellZ;
            readonly bool[,] padded;
            readonly bool[,] strict;

            public float CellX => cellX;
            public float CellZ => cellZ;
            public bool[,] StrictMask => strict;

            public MaskGrid(Terrain terrain, int resX, int resZ)
            {
                td = terrain.terrainData;
                origin = terrain.transform.position;
                size = td.size;
                this.resX = resX;
                this.resZ = resZ;
                cellX = size.x / resX;
                cellZ = size.z / resZ;
                padded = new bool[resZ, resX];
                strict = new bool[resZ, resX];
            }

            public void Stamp(Target target, bool volume, float padding)
            {
                Bounds b = target.WorldBounds;

                // Range of cells overlapped by this target's bounds.
                int x0 = Mathf.FloorToInt((b.min.x - origin.x) / cellX);
                int x1 = Mathf.FloorToInt((b.max.x - origin.x) / cellX);
                int z0 = Mathf.FloorToInt((b.min.z - origin.z) / cellZ);
                int z1 = Mathf.FloorToInt((b.max.z - origin.z) / cellZ);
                if (x1 < 0 || z1 < 0 || x0 >= resX || z0 >= resZ) return;
                x0 = Mathf.Max(x0, 0); z0 = Mathf.Max(z0, 0);
                x1 = Mathf.Min(x1, resX - 1); z1 = Mathf.Min(z1, resZ - 1);

                for (int z = z0; z <= z1; z++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        if (strict[z, x]) continue; // strict implies padded, nothing more to learn

                        float u = (x + 0.5f) / resX;
                        float v = (z + 0.5f) / resZ;
                        float wx = origin.x + u * size.x;
                        float wz = origin.z + v * size.z;

                        if (!target.TryGetColumn(wx, wz, volume, out float yMin, out float yMax)) continue;

                        if (!volume)
                        {
                            padded[z, x] = true;
                            strict[z, x] = true;
                            continue;
                        }

                        float wy = origin.y + td.GetInterpolatedHeight(u, v);
                        if (wy >= yMin && wy <= yMax)
                        {
                            padded[z, x] = true;
                            strict[z, x] = true;
                        }
                        else if (wy >= yMin - padding && wy <= yMax + padding)
                        {
                            padded[z, x] = true;
                        }
                    }
                }
            }

            /// <summary>Mask for details / tree removal: the padded mask grown by Padding metres.</summary>
            public bool[,] PaddedMask(float padding)
            {
                return Dilate(padded, Mathf.CeilToInt(padding / cellX), Mathf.CeilToInt(padding / cellZ));
            }
        }

        /// <summary>Builds the mask grid at the given resolution. Returns null if the user cancelled.</summary>
        MaskGrid BuildGrid(int resX, int resZ, List<Target> targets, TreeTargets trees, string label)
        {
            var grid = new MaskGrid(terrain, resX, resZ);
            bool volume = detection == Detection.Volume;
            int total = targets.Count + (trees != null ? trees.InstanceCount : 0);
            int done = 0;

            foreach (Target target in targets)
            {
                if (Cancelled(label, target.Name, done++, total, 1)) return null;
                grid.Stamp(target, volume, padding);
            }

            if (trees != null)
            {
                foreach (Target target in trees.Instances())
                {
                    if (Cancelled(label, target.Name, done++, total, 16)) return null;
                    grid.Stamp(target, volume, padding);
                }
            }

            return grid;
        }

        static bool Cancelled(string label, string name, int done, int total, int every)
        {
            if (done % every != 0) return false;
            float progress = total > 0 ? Mathf.Clamp01(done / (float)total) : 0f;
            return EditorUtility.DisplayCancelableProgressBar("Terrain Blackout",
                $"{label}: {name} ({done}/{total})", progress);
        }

        /// <summary>Elliptical dilation by (rx, rz) cells. Only edge cells spread, for speed.</summary>
        static bool[,] Dilate(bool[,] src, int rx, int rz)
        {
            if (rx <= 0 && rz <= 0) return src;

            int h = src.GetLength(0), w = src.GetLength(1);
            var dst = (bool[,])src.Clone();

            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (!src[z, x]) continue;

                    bool border = x == 0 || z == 0 || x == w - 1 || z == h - 1
                                  || !src[z, x - 1] || !src[z, x + 1]
                                  || !src[z - 1, x] || !src[z + 1, x];
                    if (!border) continue;

                    for (int dz = -rz; dz <= rz; dz++)
                    {
                        int nz = z + dz;
                        if (nz < 0 || nz >= h) continue;
                        float ez = rz > 0 ? (float)dz / rz : 0f;

                        for (int dx = -rx; dx <= rx; dx++)
                        {
                            int nx = x + dx;
                            if (nx < 0 || nx >= w) continue;
                            float ex = rx > 0 ? (float)dx / rx : 0f;
                            if (ex * ex + ez * ez <= 1f) dst[nz, nx] = true;
                        }
                    }
                }
            }
            return dst;
        }

        // ------------------------------------------------------- Inside-only paint

        /// <summary>
        /// For every mask cell, the distance (in metres) to the nearest cell outside the mask
        /// (two-pass chamfer approximation). Cells outside the mask get 0. The terrain's own
        /// border does not count as an edge, so paint can run right up to the terrain boundary.
        /// </summary>
        static float[,] InwardDistance(bool[,] mask, float cx, float cz)
        {
            const float Inf = 1e30f;
            int h = mask.GetLength(0), w = mask.GetLength(1);
            float cd = Mathf.Sqrt(cx * cx + cz * cz);
            var d = new float[h, w];

            for (int z = 0; z < h; z++)
                for (int x = 0; x < w; x++)
                    d[z, x] = mask[z, x] ? Inf : 0f;

            // Forward pass
            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    float v = d[z, x];
                    if (v == 0f) continue;
                    if (x > 0) v = Mathf.Min(v, d[z, x - 1] + cx);
                    if (z > 0)
                    {
                        v = Mathf.Min(v, d[z - 1, x] + cz);
                        if (x > 0) v = Mathf.Min(v, d[z - 1, x - 1] + cd);
                        if (x < w - 1) v = Mathf.Min(v, d[z - 1, x + 1] + cd);
                    }
                    d[z, x] = v;
                }
            }

            // Backward pass
            for (int z = h - 1; z >= 0; z--)
            {
                for (int x = w - 1; x >= 0; x--)
                {
                    float v = d[z, x];
                    if (v == 0f) continue;
                    if (x < w - 1) v = Mathf.Min(v, d[z, x + 1] + cx);
                    if (z < h - 1)
                    {
                        v = Mathf.Min(v, d[z + 1, x] + cz);
                        if (x < w - 1) v = Mathf.Min(v, d[z + 1, x + 1] + cd);
                        if (x > 0) v = Mathf.Min(v, d[z + 1, x - 1] + cd);
                    }
                    d[z, x] = v;
                }
            }

            return d;
        }

        /// <summary>
        /// Black-paint weights (0..1) that exist only inside the object:
        ///   - nothing within one alphamap cell of the edge (bilinear filtering of the splat map
        ///     would otherwise smear the paint outside the object),
        ///   - nothing within the extra 'inset' distance either,
        ///   - then a fade-in over 'feather' metres, moving inwards.
        /// </summary>
        static float[,] BuildInnerWeights(bool[,] strictMask, float cellX, float cellZ, float inset, float feather)
        {
            int h = strictMask.GetLength(0), w = strictMask.GetLength(1);
            float[,] dist = InwardDistance(strictMask, cellX, cellZ);
            float start = Mathf.Max(cellX, cellZ) + inset;//Mathf.Max(0f, inset);
            var weights = new float[h, w];

            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (!strictMask[z, x]) continue;

                    float d = dist[z, x];
                    if (d <= start + 1e-4f) continue;

                    weights[z, x] = feather > 0f ? Mathf.Clamp01((d - start) / feather) : 1f;
                }
            }
            return weights;
        }

        // -------------------------------------------------------------- Actions

        int PaintBlack(TerrainData td, MaskGrid grid)
        {
            TerrainLayer black = GetOrCreateBlackLayer();

            // Make sure the black layer is part of the terrain.
            var layers = new List<TerrainLayer>(td.terrainLayers);
            int blackIndex = layers.IndexOf(black);
            if (blackIndex < 0)
            {
                layers.Add(black);
                td.terrainLayers = layers.ToArray();
                blackIndex = layers.Count - 1;
            }

            int w = td.alphamapWidth, h = td.alphamapHeight, layerCount = td.alphamapLayers;
            float[,,] maps = td.GetAlphamaps(0, 0, w, h); // [z, x, layer]

            float[,] weights = BuildInnerWeights(grid.StrictMask, grid.CellX, grid.CellZ, colourInset, feather);

            int count = 0;
            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    float k = weights[z, x];
                    if (k <= 0f) continue;

                    for (int l = 0; l < layerCount; l++)
                        maps[z, x, l] = Mathf.Lerp(maps[z, x, l], l == blackIndex ? 1f : 0f, k);

                    count++;
                }
            }

            td.SetAlphamaps(0, 0, maps);
            return count;
        }

        static int RemoveDetails(TerrainData td, bool[,] mask)
        {
            int w = td.detailWidth, h = td.detailHeight;
            int layerCount = td.detailPrototypes.Length;
            int cleared = 0;

            for (int layer = 0; layer < layerCount; layer++)
            {
                int[,] map = td.GetDetailLayer(0, 0, w, h, layer); // [z, x]
                bool changed = false;

                for (int z = 0; z < h; z++)
                    for (int x = 0; x < w; x++)
                        if (mask[z, x] && map[z, x] != 0)
                        {
                            map[z, x] = 0;
                            changed = true;
                            cleared++;
                        }

                if (changed) td.SetDetailLayer(0, 0, layer, map);
            }
            return cleared;
        }

        static int RemoveTrees(TerrainData td, bool[,] mask, TreeTargets trees)
        {
            int w = mask.GetLength(1), h = mask.GetLength(0);
            var kept = new List<TreeInstance>();
            int removed = 0;

            foreach (TreeInstance tree in td.treeInstances)
            {
                // Prototypes that act as targets (e.g. rocks) are never removed.
                if (trees != null && trees.Contains(tree.prototypeIndex))
                {
                    kept.Add(tree);
                    continue;
                }

                // Tree positions are normalised (0..1) across the terrain.
                int x = Mathf.Clamp((int)(tree.position.x * w), 0, w - 1);
                int z = Mathf.Clamp((int)(tree.position.z * h), 0, h - 1);
                if (mask[z, x]) removed++;
                else kept.Add(tree);
            }

            if (removed > 0) td.SetTreeInstances(kept.ToArray(), false);
            return removed;
        }

        // ---------------------------------------------------------- Black layer

        TerrainLayer GetOrCreateBlackLayer()
        {
            EnsureFolder(assetFolder);

            string layerPath = $"{assetFolder}/BlackTerrainLayer.terrainlayer";
            var existing = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
            if (existing != null) return existing;

            string texPath = $"{assetFolder}/Black.png";
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (tex == null)
            {
                var t = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                var pixels = new Color32[16];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 255);
                t.SetPixels32(pixels);

                string absolute = Path.Combine(Path.GetDirectoryName(Application.dataPath), texPath);
                File.WriteAllBytes(absolute, t.EncodeToPNG());
                DestroyImmediate(t);

                AssetDatabase.ImportAsset(texPath);
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            }

            var layer = new TerrainLayer
            {
                diffuseTexture = tex,
                tileSize = new Vector2(8f, 8f),
                metallic = 0f,
                smoothness = 0f,
                specular = Color.black
            };
            AssetDatabase.CreateAsset(layer, layerPath);
            AssetDatabase.SaveAssets();
            return layer;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}