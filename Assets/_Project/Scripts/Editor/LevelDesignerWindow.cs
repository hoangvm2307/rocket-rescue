 
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class LevelDesignerWindow : EditorWindow
{
    // A nested class for constants to avoid magic strings
    private static class Constants
    {
        // Menu Items
        public const string MenuPath = "Tools/Rocket Rescue/Level Designer";
        public const string WindowTitle = "Level Designer";
        public const string CreatePlatformMenuPath = "Assets/Create/Rocket Rescue/Basic Platform Prefab";

        // UI Labels & Messages
        public const string EnableToolLabel = "Enable Tool";
        public const string ToolModeLabel = "Tool Mode";
        public const string ObjectPaletteLabel = "Object Palette";
        public const string GridSettingsLabel = "Grid Settings";
        public const string EnableGridSnappingLabel = "Enable Grid Snapping";
        public const string GridSizeLabel = "Grid Size";
        public const string NoPrefabsFoundMessage = "No matching prefabs found.";
        public const string BuildModeSceneHelpMessage = "Select a prefab from the\nLevel Designer window to build.";
        public static string MoveModeEditorHelpMessage(string toolName) => $"In {toolName} mode.\nClick on objects in the Scene View to interact.";


        // Tool Names
        public static readonly string[] ToolNames = { "Build", "Move" };

        // Asset & Scene Object Names
        public const string PlatformNameIdentifier = "Platform";
        public const string EnemiesParentName = "[Enemies]";
        public const string PlatformsParentName = "[Platforms]";
        public const string PlatformPrefabName = "Platform.prefab";

        // Prefab & Asset Paths
        public static readonly string[] PrefabSearchPaths = { "Assets/_Project/Prefabs/Character", "Assets/_Project/Prefabs/Environment" };
        public const string GameObjectAssetQuery = "t:GameObject";
        public const string DefaultPlatformPrefabDir = "Assets/_Project/Prefabs/Environment";

        // GUI Styles
        public const string ToolbarSearchTextFieldStyle = "ToolbarSearchTextField";
        public const string ToolbarSearchCancelButtonStyle = "ToolbarSearchCancelButton";

        // Undo Messages
        public const string UndoMove = "Move ";
        public const string UndoPlace = "Place ";
        public const string UndoCreatePlatform = "Create Platform";
        public static string UndoCreateParent(string name) => $"Create {name} Parent";

        // Logging
        public static string PlatformExistsWarning(string path) => $"Platform prefab already exists at: {path}";
        public static string PlatformCreatedLog(string path) => $"Created basic platform prefab at: {path}";
    }

    [MenuItem(Constants.MenuPath)]
    public static void ShowWindow()
    {
        GetWindow<LevelDesignerWindow>(Constants.WindowTitle);
    }

    // --- State variables ---
    private List<GameObject> availablePrefabs;
    private Vector2 scrollPosition;
    private GameObject selectedPrefab;
    private string searchQuery = "";
    private ToolMode currentTool = ToolMode.Build;
    private bool isGridEnabled = true;
    private float gridSize = 1.0f;
    private bool isDrawing = false;
    private Vector3 drawStartPosition;
    private GameObject objectBeingMoved = null;
    private bool isToolEnabled = true;

    // --- Enums ---
    private enum ToolMode { Build, Move }

    // --- Unity Methods ---

    private void OnEnable()
    {
        RefreshPrefabList();
        // Listen for events in the Scene window
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Stop listening to avoid errors
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnFocus()
    {
        RefreshPrefabList();
    }
    
    void OnGUI()
    {
        DrawToolbar();

        EditorGUI.BeginDisabledGroup(!isToolEnabled);

        if (currentTool == ToolMode.Build)
        {
            DrawBuildModeUI();
        }
        else
        {
            DrawMoveModeUI();
        }

        EditorGUI.EndDisabledGroup();
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isToolEnabled)
        {
            sceneView.Repaint(); // Repaint to ensure grid etc. are hidden when disabled
            return;
        }

        // Draw the grid first
        if (isGridEnabled)
        {
            DrawGrid(sceneView);
        }

        // Ensure that mouse events are correctly captured by the SceneView
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;

        switch (currentTool)
        {
            case ToolMode.Build:
                HandleBuildMode(e, sceneView);
                break;
            case ToolMode.Move:
                HandleMoveMode(e);
                break;
        }
    }

    // --- UI Drawing Methods ---

    private void DrawToolbar()
    {
        isToolEnabled = EditorGUILayout.Toggle(Constants.EnableToolLabel, isToolEnabled);
        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(!isToolEnabled))
        {
            GUILayout.Label(Constants.ToolModeLabel, EditorStyles.boldLabel);
            currentTool = (ToolMode)GUILayout.Toolbar((int)currentTool, Constants.ToolNames);
            EditorGUILayout.Space();
        }
    }

    private void DrawBuildModeUI()
    {
        GUILayout.Label(Constants.ObjectPaletteLabel, EditorStyles.boldLabel);
        DrawGridSettings();
        EditorGUILayout.Space();
        DrawSearchBar();
        DrawPrefabPalette();
    }
    
    private void DrawMoveModeUI()
    {
        EditorGUILayout.HelpBox(Constants.MoveModeEditorHelpMessage(Constants.ToolNames[(int)currentTool]), MessageType.Info);
    }

    private void DrawGridSettings()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label(Constants.GridSettingsLabel, EditorStyles.miniBoldLabel);
        isGridEnabled = EditorGUILayout.Toggle(Constants.EnableGridSnappingLabel, isGridEnabled);
        gridSize = EditorGUILayout.FloatField(Constants.GridSizeLabel, gridSize);
        if (gridSize <= 0) gridSize = 0.1f;
        GUILayout.EndVertical();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        searchQuery = GUILayout.TextField(searchQuery, GUI.skin.FindStyle(Constants.ToolbarSearchTextFieldStyle));
        if (GUILayout.Button("", GUI.skin.FindStyle(Constants.ToolbarSearchCancelButtonStyle)))
        {
            // Remove focus when the clear button is pressed
            searchQuery = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPrefabPalette()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Filter prefabs based on the search query
        var prefabsToShow = string.IsNullOrWhiteSpace(searchQuery)
            ? availablePrefabs
            : availablePrefabs.Where(p => p.name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant())).ToList();

        if (prefabsToShow == null || prefabsToShow.Count == 0)
        {
            EditorGUILayout.HelpBox(Constants.NoPrefabsFoundMessage, MessageType.Info);
        }
        else
        {
            DrawPrefabGrid(prefabsToShow);
        }
        
        // Reset background color to default
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndScrollView();
    }

    private void DrawPrefabGrid(List<GameObject> prefabsToShow)
    {
        // Grid Layout for Prefab Palette
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / 90));

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            imagePosition = ImagePosition.ImageAbove,
            fixedHeight = 85,
            fixedWidth = 85,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fontSize = 9,
        };

        int count = 0;
        EditorGUILayout.BeginHorizontal();
        foreach (var prefab in prefabsToShow)
        {
            if (count > 0 && count % itemsPerRow == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            if (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()))
            {
                Repaint();
            }

            GUIContent content = new GUIContent(prefab.name, preview);
            GUI.backgroundColor = (selectedPrefab == prefab) ? Color.cyan : Color.white;

            if (GUILayout.Button(content, buttonStyle))
            {
                selectedPrefab = prefab;
            }
            count++;
        }
        EditorGUILayout.EndHorizontal();
    }

    // --- Scene GUI Handlers ---
    
    private void RefreshPrefabList()
    {
        if (availablePrefabs == null)
        {
            availablePrefabs = new List<GameObject>();
        }
        availablePrefabs.Clear();

        string[] prefabGuids = AssetDatabase.FindAssets(Constants.GameObjectAssetQuery, Constants.PrefabSearchPaths);

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                availablePrefabs.Add(prefab);
            }
        }
        
        // Sort list alphabetically for easier searching
        availablePrefabs = availablePrefabs.OrderBy(p => p.name).ToList();
    }

    private void HandleBuildMode(Event e, SceneView sceneView)
    {
        if (selectedPrefab == null)
        {
            if (e.type == EventType.Repaint)
            {
                Handles.BeginGUI();
                GUI.backgroundColor = new Color(0, 0, 0, 0.7f);
                GUI.Box(new Rect(10, sceneView.position.height - 70, 220, 40), Constants.BuildModeSceneHelpMessage, EditorStyles.helpBox);
                Handles.EndGUI();
            }
            return;
        }

        Plane gridPlane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        
        if (gridPlane.Raycast(ray, out float enter))
        {
            Vector3 mousePosition = ray.GetPoint(enter);
            Vector3 snappedPosition = SnapToGrid(mousePosition);

            if (IsPlatform(selectedPrefab))
            {
                HandlePlatformDrawing(e, snappedPosition);
            }
            else
            {
                HandleObjectPlacement(e, snappedPosition);
            }
        }
    }

    private void HandleMoveMode(Event e)
    {
        // On mouse down, try to pick an object to start moving.
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (HandleUtility.GUIPointToWorldRay(e.mousePosition).origin == Vector3.zero) return;

            GameObject pickedObject = HandleUtility.PickGameObject(e.mousePosition, false);
            if (pickedObject != null)
            {
                objectBeingMoved = pickedObject;
                // Record its state before moving for a single undo action.
                Undo.RegisterCompleteObjectUndo(objectBeingMoved.transform, Constants.UndoMove + objectBeingMoved.name);
                e.Use();
            }
        }
        // While dragging, update the object's position with snapping.
        else if (e.type == EventType.MouseDrag && e.button == 0 && objectBeingMoved != null)
        {
            Plane gridPlane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            if (gridPlane.Raycast(ray, out float enter))
            {
                Vector3 mousePosition = ray.GetPoint(enter);
                objectBeingMoved.transform.position = SnapToGridCenter(mousePosition);
            }
            e.Use();
        }
        // On mouse up, release the object.
        else if (e.type == EventType.MouseUp && e.button == 0 && objectBeingMoved != null)
        {
            objectBeingMoved = null;
            e.Use();
        }
    }

    private void HandleObjectPlacement(Event e, Vector3 position)
    {
        Handles.color = Color.green;
        Vector3 previewSize = selectedPrefab.transform.localScale;
        // Use a better preview size for sprites
        var spriteRenderer = selectedPrefab.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null && spriteRenderer.sprite != null)
        {
            previewSize = spriteRenderer.sprite.bounds.size;
        }
        Handles.DrawWireCube(position, previewSize);

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            newObject.transform.position = position;
            Undo.RegisterCreatedObjectUndo(newObject, Constants.UndoPlace + selectedPrefab.name);

            // New parenting logic
            if (selectedPrefab.GetComponent<BaseEnemy>() != null)
            {
                Transform enemyParent = GetOrCreateParent(Constants.EnemiesParentName);
                newObject.transform.SetParent(enemyParent, true);
            }

            e.Use();
        }
    }

    private void HandlePlatformDrawing(Event e, Vector3 snappedPosition)
    {
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isDrawing = true;
            drawStartPosition = snappedPosition;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            if (isDrawing)
            {
                isDrawing = false;
                CreatePlatform(drawStartPosition, snappedPosition);
                e.Use();
            }
        }

        if (isDrawing)
        {
            Bounds bounds = new Bounds();
            bounds.SetMinMax(
                Vector3.Min(drawStartPosition, snappedPosition),
                Vector3.Max(drawStartPosition, snappedPosition)
            );
            // Adjust center and size to fill the tiles correctly
            bounds.center += new Vector3(gridSize/2, gridSize/2, 0);
            bounds.size += new Vector3(gridSize, gridSize, 0);

            Handles.color = Color.cyan;
            Handles.DrawWireCube(bounds.center, bounds.size);
        }
    }

    private void CreatePlatform(Vector3 start, Vector3 end)
    {
        GameObject platform = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
        
        Bounds bounds = new Bounds();
        bounds.SetMinMax(Vector3.Min(start, end), Vector3.Max(start, end));
        
        Vector3 size = new Vector3(
            Mathf.Max(bounds.size.x, 0) + gridSize, 
            Mathf.Max(bounds.size.y, 0) + gridSize,
            selectedPrefab.transform.localScale.z
        );
        
        platform.transform.position = bounds.center + new Vector3(gridSize / 2f, gridSize / 2f, 0);
        platform.transform.localScale = size;
        
        // New parenting logic
        Transform platformParent = GetOrCreateParent(Constants.PlatformsParentName);
        platform.transform.SetParent(platformParent, true);

        Undo.RegisterCreatedObjectUndo(platform, Constants.UndoCreatePlatform);
    }

    // --- Utility Methods ---

    private Transform GetOrCreateParent(string name)
    {
        GameObject parent = GameObject.Find(name);
        if (parent == null)
        {
            parent = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(parent, Constants.UndoCreateParent(name));
        }
        return parent.transform;
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        if (!isGridEnabled) return position;

        return new Vector3(
            Mathf.Floor(position.x / gridSize) * gridSize,
            Mathf.Floor(position.y / gridSize) * gridSize,
            0
        );
    }

    private Vector3 SnapToGridCenter(Vector3 position)
    {
        if (!isGridEnabled) return position;

        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            0 // Keep Z at 0 for 2D context
        );
    }

    private bool IsPlatform(GameObject obj)
    {
        if (obj == null) return false;
        // This is a very simple assumption. A better method would be to use tags or components.
        return obj.name.Contains(Constants.PlatformNameIdentifier);
    }

    private void DrawGrid(SceneView sceneView)
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        
        Camera cam = sceneView.camera;
        
        if(cam == null) return;

        var height = cam.orthographicSize * 2;
        var width = height * cam.aspect;
        
        Vector3 camPos = cam.transform.position;
        
        float startX = Mathf.Floor((camPos.x - width/2) / gridSize) * gridSize;
        float endX = Mathf.Ceil((camPos.x + width/2) / gridSize) * gridSize;
        float startY = Mathf.Floor((camPos.y - height/2) / gridSize) * gridSize;
        float endY = Mathf.Ceil((camPos.y + height/2) / gridSize) * gridSize;

        for (float x = startX; x <= endX; x += gridSize)
        {
            Handles.DrawLine(new Vector3(x, startY, 0), new Vector3(x, endY, 0));
        }

        for (float y = startY; y <= endY; y += gridSize)
        {
            Handles.DrawLine(new Vector3(startX, y, 0), new Vector3(endX, y, 0));
        }

        Handles.color = Color.white;
    }
    
    // --- Menu Item Methods ---

    [MenuItem(Constants.CreatePlatformMenuPath)]
    private static void CreateBasicPlatformPrefab()
    {
        string prefabDir = Constants.DefaultPlatformPrefabDir;
        if (!Directory.Exists(prefabDir))
        {
            Directory.CreateDirectory(prefabDir);
        }

        string prefabPath = Path.Combine(prefabDir, Constants.PlatformPrefabName);
        if (File.Exists(prefabPath))
        {
            Debug.LogWarning(Constants.PlatformExistsWarning(prefabPath));
            return;
        }

        GameObject platformGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platformGO.name = Constants.PlatformNameIdentifier;

        PrefabUtility.SaveAsPrefabAsset(platformGO, prefabPath);
        DestroyImmediate(platformGO);

        Debug.Log(Constants.PlatformCreatedLog(prefabPath));
        AssetDatabase.Refresh();
    }
}
