// Designed & Developed by Alexander Ameye
// https://alexander-ameye.gitbook.io/stylized-water/
// Version 1.0.3

#if UNIVERSAL_RENDERER
using UnityEngine;
using UnityEditor;

namespace StylizedWaterForURP
{
    [CustomEditor(typeof(StylizedWaterURP))]
    public class StylizedWaterURPEditor : Editor
    {
        StylizedWaterURP stylizedWater;
        new SerializedObject serializedObject;
        GameObject selected;
        public bool isDesktopVariant;
        public bool isLit;
        public bool useReflections;
        public bool usesColorGradient;
        public bool usesVertexColors;
        public string shaderName;

#region Water Properties
        SerializedProperty shallowColor, deepColor, horizonColor, underwaterColor, underwaterContribution, colorContrast, colorSaturation, colorDepth, horizonDistanceColor;
        SerializedProperty useColorGradient, colorGradient;
        SerializedProperty surfaceFoamTexture, surfaceFoamColor1, surfaceFoamColor2, surfaceFoamOffset, surfaceFoamCutoff, surfaceFoamDistortion, surfaceFoamScale, surfaceFoamMovement;
        SerializedProperty intersectionFoamTexture, intersectionFoamColor1, intersectionFoamColor2, intersectionFoamDepth, intersectionFoamCutoff, intersectionFoamDistortion, useVertexColors, shoreFade, shoreFadeColor, intersectionFoamScale, intersectionFoamMovement;
        SerializedProperty causticsStrength, causticsTexture, causticsDepth, causticsVisuals;
        SerializedProperty refractionDepth, refraction;
        SerializedProperty roughness, normalsTexture, normalsStrength, normalsScale, normalsSpeed, surfaceNormals;
        SerializedProperty reflectionStrength, reflectionLayer, reflectSkybox, reflectionPlaneOffset, renderScale, lighting, enableReflections;
        SerializedProperty hideComponents, hideReflectionCamera;
        SerializedProperty waveVisuals, waveDirections;
#endregion

#region Section Foldouts
        private static bool colorTransparencySettings,
             surfaceFoamSettings,
             intersectionFoamSettings,
             refractionSettings,
             causticsSettings,
             surfaceSettings,
             waveSettings,
             additionalSettings;
        public delegate void DrawSettingsMethod();
#endregion

        class Texts
        {
            public static readonly GUIContent colorTransparencySettings = EditorGUIUtility.TrTextContent("Colors and Transparency");
            public static readonly GUIContent surfaceFoamSettings = EditorGUIUtility.TrTextContent("Surface Foam");
            public static readonly GUIContent intersectionFoamSettings = EditorGUIUtility.TrTextContent("Intersection Foam");
            public static readonly GUIContent refractionSettings = EditorGUIUtility.TrTextContent("Refraction");
            public static readonly GUIContent causticsSettings = EditorGUIUtility.TrTextContent("Caustics");
            public static readonly GUIContent surfaceSettings = EditorGUIUtility.TrTextContent("Surface and Lighting");
            public static readonly GUIContent waveSettings = EditorGUIUtility.TrTextContent("Waves");
            public static readonly GUIContent additionalSettings = EditorGUIUtility.TrTextContent("Additional Settings");
            public static readonly GUIContent contrast = EditorGUIUtility.TrTextContent("Contrast");
            public static readonly GUIContent falloff = EditorGUIUtility.TrTextContent("Falloff");
            public static readonly GUIContent saturation = EditorGUIUtility.TrTextContent("Saturation");
            public static readonly GUIContent reflection = EditorGUIUtility.TrTextContent("Reflection");
            public static readonly GUIContent source = EditorGUIUtility.TrTextContent("Source");
            public static readonly GUIContent hideComponents = EditorGUIUtility.TrTextContent("Hide Components");
            public static readonly GUIContent hideReflectionCamera = EditorGUIUtility.TrTextContent("Hide Reflection Camera");
            public static readonly GUIContent useColorGradient = EditorGUIUtility.TrTextContent("Use Gradient");
            public static readonly GUIContent shallow = EditorGUIUtility.TrTextContent("Shallow");
            public static readonly GUIContent deep = EditorGUIUtility.TrTextContent("Deep");
            public static readonly GUIContent depth = EditorGUIUtility.TrTextContent("Depth");
            public static readonly GUIContent albedo = EditorGUIUtility.TrTextContent("Albedo");
            public static readonly GUIContent distance = EditorGUIUtility.TrTextContent("Distance");
            public static readonly GUIContent strength = EditorGUIUtility.TrTextContent("Strength");
            public static readonly GUIContent surfaceNormals = EditorGUIUtility.TrTextContent("Surface Normals");
            public static readonly GUIContent method = EditorGUIUtility.TrTextContent("Method");
            public static readonly GUIContent texture = EditorGUIUtility.TrTextContent("Texture");
            public static readonly GUIContent scale = EditorGUIUtility.TrTextContent("Scale");
            public static readonly GUIContent depthMask = EditorGUIUtility.TrTextContent("Depth Mask");
            public static readonly GUIContent roughness = EditorGUIUtility.TrTextContent("Roughness");
            public static readonly GUIContent steepness = EditorGUIUtility.TrTextContent("Steepness");
            public static readonly GUIContent wavelength = EditorGUIUtility.TrTextContent("Wavelength");
            public static readonly GUIContent fade = EditorGUIUtility.TrTextContent("Fade");
            public static readonly GUIContent color = EditorGUIUtility.TrTextContent("Color");
            public static readonly GUIContent direction = EditorGUIUtility.TrTextContent("Direction");
            public static readonly GUIContent foamOffset = EditorGUIUtility.TrTextContent("Offset");
            public static readonly GUIContent hardness = EditorGUIUtility.TrTextContent("Hardness");
            public static readonly GUIContent speed = EditorGUIUtility.TrTextContent("Speed");
            public static readonly GUIContent resolution = EditorGUIUtility.TrTextContent("Resolution");
            public static readonly GUIContent layerMask = EditorGUIUtility.TrTextContent("Layer Mask");
            public static readonly GUIContent reflectSkybox = EditorGUIUtility.TrTextContent("Reflect Skybox");
            public static readonly GUIContent offset = EditorGUIUtility.TrTextContent("Offset");
            public static readonly GUIContent renderScale = EditorGUIUtility.TrTextContent("Render Scale");
            public static readonly GUIContent useVertexColors = EditorGUIUtility.TrTextContent("Use Vertex Colors");
            public static readonly GUIContent lighting = EditorGUIUtility.TrTextContent("Lighting");
            public static readonly GUIContent cutoff = EditorGUIUtility.TrTextContent("Cutoff");
            public static readonly GUIContent distortion = EditorGUIUtility.TrTextContent("Distortion");
            public static readonly GUIContent enableReflections = EditorGUIUtility.TrTextContent("Reflections");
        }

        [MenuItem("GameObject/3D Object/Stylized Water/Square", priority = 7)]
        static void CreateSquareWater() => InstantiateWater("Square Water");

        [MenuItem("GameObject/3D Object/Stylized Water/Hexagonal", priority = 7)]
        static void CreateHexagonalWater() => InstantiateWater("Hexagonal Water");

        [MenuItem("GameObject/3D Object/Stylized Water/Circular", priority = 7)]
        static void CreateCircularWater() => InstantiateWater("Circular Water");

        static void InstantiateWater(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Prefab " + name);
            if (guids.Length == 0)
            {
                Debug.Log("Error: water prefab not found");
                return;
            }

            else
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                Undo.RegisterCreatedObjectUndo(instance, "Create Water Object");
                Selection.activeObject = instance;
                SceneView.FrameLastActiveSceneView();
            }
        }

        public void OnEnable()
        {
            selected = Selection.activeGameObject;

            if (!selected) return;
            if (!stylizedWater) stylizedWater = selected.GetComponent<StylizedWaterURP>();
            if (stylizedWater)
            {
                serializedObject = new SerializedObject(stylizedWater);
                GetWaterProperties();
            }

            Undo.undoRedoPerformed += ApplyChanges;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            stylizedWater.ReadMaterialProperties();
            EditorGUI.BeginChangeCheck();
            DrawSections();
            if (EditorGUI.EndChangeCheck()) ApplyChanges();
        }

        void ApplyChanges()
        {
            if (serializedObject.targetObject) serializedObject.ApplyModifiedProperties();
            stylizedWater.WriteMaterialProperties();
            GetWaterProperties();
            stylizedWater.ReadMaterialProperties();
        }

        void DrawSections()
        {
            if (!stylizedWater.meshRenderer || !stylizedWater.meshRenderer.sharedMaterial) return;
            string shaderName = stylizedWater.meshRenderer.sharedMaterial.shader.name;
            if (!(shaderName == "Shader Graphs/Stylized Water For URP Desktop" || shaderName == "Shader Graphs/Stylized Water For URP Mobile"))
            {
                EditorGUILayout.HelpBox("Material incompatible.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            if (shaderName.EndsWith("Desktop")) EditorGUILayout.LabelField("  " + stylizedWater.material.name + " - Desktop shader");
            else if (shaderName.EndsWith("Mobile")) EditorGUILayout.LabelField("  " + stylizedWater.material.name + " - Mobile shader");
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            colorTransparencySettings = DrawSection(colorTransparencySettings, Texts.colorTransparencySettings, DrawColorSettings);
            surfaceFoamSettings = DrawSection(surfaceFoamSettings, Texts.surfaceFoamSettings, DrawSurfaceFoamSettings);
            intersectionFoamSettings = DrawSection(intersectionFoamSettings, Texts.intersectionFoamSettings, DrawIntersectionFoamSettings);
            if (shaderName == "Shader Graphs/Stylized Water For URP Desktop") refractionSettings = DrawSection(refractionSettings, Texts.refractionSettings, DrawRefractionSettings);

            causticsSettings = DrawSection(causticsSettings, Texts.causticsSettings, DrawCausticsSettings);
            if (shaderName == "Shader Graphs/Stylized Water For URP Desktop") surfaceSettings = DrawSection(surfaceSettings, Texts.surfaceSettings, DrawSurfaceSettings);
            waveSettings = DrawSection(waveSettings, Texts.waveSettings, DrawWaveSettings);
            additionalSettings = DrawSection(additionalSettings, Texts.additionalSettings, DrawAdditionalSettings);
            if (stylizedWater.meshRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On) EditorGUILayout.HelpBox("Water is casting shadows. You should turn this off.", MessageType.Warning);
        }

        bool DrawSection(bool settings, GUIContent title, DrawSettingsMethod DrawProperties)
        {
            settings = EditorGUILayout.BeginFoldoutHeaderGroup(settings, title, null, HelpMenu);
            if (settings)
            {
                EditorGUI.indentLevel++;
                DrawProperties();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();
            return settings;
        }

        void HelpMenu(Rect position)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Open Documentation"), false, OpenDocs, "");
            menu.AddItem(new GUIContent("Clear Section"), false, OpenDocs, "");
            menu.DropDown(position);
        }

        public void OpenDocs(object link) => Application.OpenURL("https://alexanderameye.gitbook.io/stylized-water-for-urp/shader-properties" + "#" + link);

        public void ClearSection()
        {

        }

        void DrawColorSettings()
        {
            EditorGUILayout.Space(); 
            EditorGUILayout.LabelField("Depth", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(useColorGradient, Texts.useColorGradient);

            if (!usesColorGradient)
            {
                EditorGUILayout.PropertyField(shallowColor, Texts.shallow);
                EditorGUILayout.PropertyField(deepColor, Texts.deep);
                EditorGUILayout.PropertyField(colorDepth, Texts.depth);
            }

            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(colorGradient, Texts.color);
                if (GUILayout.Button("Apply")) ApplyColorGradient(stylizedWater);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(colorDepth, Texts.depth);
            }

            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.LabelField("Horizon", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(horizonColor, Texts.color);
            EditorGUILayout.PropertyField(horizonDistanceColor, Texts.distance);

            /*if (isDesktopVariant)
            {
                EditorGUILayout.Space(); EditorGUILayout.Space();
                EditorGUILayout.LabelField("Underwater", EditorStyles.helpBox);
                EditorGUILayout.PropertyField(underwaterColor, Texts.color);
                EditorGUILayout.PropertyField(underwaterContribution, Texts.strength);
            }*/

            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.LabelField("Adjustments", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(colorContrast, Texts.contrast);
            EditorGUILayout.PropertyField(colorSaturation, Texts.saturation);
        }

        public static void ApplyColorGradient(StylizedWaterURP water)
        {
            water.colorGradientTexture = GradientTextureMaker.CreateGradientTexture(water.material, water.colorGradient);
            water.material.SetTexture("_ColorGradientTexture", water.colorGradientTexture);
        }

        void DrawSurfaceFoamSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("General", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(surfaceFoamTexture, Texts.texture);
            EditorGUILayout.PropertyField(surfaceFoamCutoff, Texts.cutoff);
            EditorGUILayout.PropertyField(surfaceFoamDistortion, Texts.distortion);

            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.LabelField("Primary (R)", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(surfaceFoamColor1, Texts.color);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var scaleX = EditorGUILayout.Slider("Scale", surfaceFoamScale.vector2Value.x, 0.1f, 20f);
                var movementX = EditorGUILayout.Slider("Direction", surfaceFoamMovement.vector4Value.x, 0f, 1f);
                var movementY = EditorGUILayout.Slider("Speed", surfaceFoamMovement.vector4Value.y, 0f, 2f);

                EditorGUILayout.Space(); EditorGUILayout.Space();
                EditorGUILayout.LabelField("Secondary (G)", EditorStyles.helpBox);
                EditorGUILayout.PropertyField(surfaceFoamColor2, Texts.color);

                var scaleY = EditorGUILayout.Slider("Scale", surfaceFoamScale.vector2Value.y, 0.1f, 20f);
                var movementZ = EditorGUILayout.Slider("Direction", surfaceFoamMovement.vector4Value.z, 0f, 1f);
                var movementW = EditorGUILayout.Slider("Speed", surfaceFoamMovement.vector4Value.w, 0f, 2f);

                if (check.changed)
                {
                    surfaceFoamScale.vector2Value = new Vector2(scaleX, scaleY);
                    surfaceFoamMovement.vector4Value = new Vector4(movementX, movementY, movementZ, movementW);
                }
            }

            EditorGUILayout.PropertyField(surfaceFoamOffset, Texts.foamOffset);
        }

        void DrawIntersectionFoamSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("General", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(intersectionFoamTexture, Texts.texture);
            EditorGUILayout.PropertyField(intersectionFoamCutoff, Texts.cutoff);
            EditorGUILayout.PropertyField(intersectionFoamDistortion, Texts.distortion);
            if (!usesVertexColors) EditorGUILayout.PropertyField(intersectionFoamDepth, Texts.depth);

            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shore Fade", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(shoreFadeColor, Texts.color);
            EditorGUILayout.PropertyField(shoreFade, Texts.fade);

            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.LabelField("Primary (R)", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(intersectionFoamColor1, Texts.color);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var scaleX = EditorGUILayout.Slider("Scale", intersectionFoamScale.vector2Value.x, 0.1f, 20f);
                var movementX = EditorGUILayout.Slider("Direction", intersectionFoamMovement.vector4Value.x, 0f, 1f);
                var movementY = EditorGUILayout.Slider("Speed", intersectionFoamMovement.vector4Value.y, 0f, 2f);

                EditorGUILayout.Space(); EditorGUILayout.Space();
                EditorGUILayout.LabelField("Secondary (G)", EditorStyles.helpBox);
                EditorGUILayout.PropertyField(intersectionFoamColor2, Texts.color);

                var scaleY = EditorGUILayout.Slider("Scale", intersectionFoamScale.vector2Value.y, 0.1f, 20f);
                var movementZ = EditorGUILayout.Slider("Direction", intersectionFoamMovement.vector4Value.z, 0f, 1f);
                var movementW = EditorGUILayout.Slider("Speed", intersectionFoamMovement.vector4Value.w, 0f, 2f);

                if (check.changed)
                {
                    intersectionFoamScale.vector2Value = new Vector2(scaleX, scaleY);
                    intersectionFoamMovement.vector4Value = new Vector4(movementX, movementY, movementZ, movementW);
                }
            }

            EditorGUILayout.Space();
        }

        void DrawRefractionSettings()
        {
            EditorGUILayout.Space();
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var newZ = EditorGUILayout.Slider("Strength", refraction.vector3Value.z, 0f, 2f);
                var newY = EditorGUILayout.Slider("Scale", refraction.vector3Value.y, 0.1f, 1f);
                var newX = EditorGUILayout.Slider("Speed", refraction.vector3Value.x, 0f, 2f);

                if (check.changed) refraction.vector3Value = new Vector3(newX, newY, newZ);
            }
            EditorGUILayout.PropertyField(refractionDepth, Texts.depthMask);
        }

        void DrawCausticsSettings()
        {
            EditorGUILayout.Space(); 
            EditorGUILayout.PropertyField(causticsTexture, Texts.texture);
            EditorGUILayout.PropertyField(causticsStrength, Texts.strength);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var newW = EditorGUILayout.Slider("Scale", causticsVisuals.vector4Value.w, 0.1f, 10f);
                var newZ = EditorGUILayout.Slider("Speed", causticsVisuals.vector4Value.z, 0f, 0.3f);
                var newY = EditorGUILayout.Slider("RGB Split", causticsVisuals.vector4Value.y, 0f, 1f);
                var newX = 0f;
                if (isDesktopVariant) newX = EditorGUILayout.Slider("Shadow Mask", causticsVisuals.vector4Value.x, 0f, 1f);

                if (check.changed) causticsVisuals.vector4Value = new Vector4(newX, newY, newZ, newW);
            }
            EditorGUILayout.PropertyField(causticsDepth, Texts.depth);
        }

        void DrawSurfaceSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lighting", EditorStyles.helpBox);
            lighting.enumValueIndex = EditorGUILayout.Popup(Texts.lighting, lighting.enumValueIndex, lighting.enumDisplayNames);
            EditorGUILayout.PropertyField(roughness, Texts.roughness);
            EditorGUILayout.PropertyField(enableReflections, Texts.enableReflections);

            if (isLit)
            {
                EditorGUILayout.Space(); EditorGUILayout.Space();
                EditorGUILayout.LabelField("Normals", EditorStyles.helpBox);
                EditorGUILayout.PropertyField(normalsTexture, Texts.texture);
                EditorGUILayout.PropertyField(normalsStrength, Texts.strength);
                EditorGUILayout.PropertyField(normalsScale, Texts.scale);
                EditorGUILayout.PropertyField(normalsSpeed, Texts.speed);
            }

            if(useReflections)
            {
                EditorGUILayout.Space(); EditorGUILayout.Space();
                EditorGUILayout.LabelField("Planar Reflections", EditorStyles.helpBox);
                EditorGUILayout.PropertyField(reflectionStrength, Texts.strength);
                EditorGUILayout.PropertyField(reflectionPlaneOffset, Texts.offset);
                EditorGUILayout.PropertyField(renderScale, Texts.renderScale);
                EditorGUILayout.PropertyField(reflectionLayer, Texts.layerMask);
                EditorGUILayout.PropertyField(reflectSkybox, Texts.reflectSkybox);
            }
        }

        void DrawWaveSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Visuals", EditorStyles.helpBox);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var newX = EditorGUILayout.Slider("Steepness", waveVisuals.vector3Value.x, 0f, 0.5f);
                var newY = EditorGUILayout.Slider("Spread", waveVisuals.vector3Value.y, 0.01f, 20f);
                var newZ = EditorGUILayout.Slider("Speed", waveVisuals.vector3Value.z, 0f, 3f);

                if (check.changed) waveVisuals.vector3Value = new Vector3(newX, newY, newZ);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Directions", EditorStyles.helpBox);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var newX = EditorGUILayout.Slider("Wave 1", waveDirections.vector4Value.x, 0f, 1f);
                var newY = EditorGUILayout.Slider("Wave 2", waveDirections.vector4Value.y, 0f, 1f);
                var newZ = EditorGUILayout.Slider("Wave 3", waveDirections.vector4Value.z, 0f, 1f);
                var newW = EditorGUILayout.Slider("Wave 4", waveDirections.vector4Value.w, 0f, 1f);

                if (check.changed) waveDirections.vector4Value = new Vector4(newX, newY, newZ, newW);
            }
        }

        void DrawAdditionalSettings()
        {
            EditorGUIUtility.labelWidth = 160;
            EditorGUILayout.Space(); 
            EditorGUILayout.PropertyField(hideComponents, Texts.hideComponents);
            EditorGUILayout.PropertyField(useVertexColors, Texts.useVertexColors);
            EditorGUILayout.PropertyField(hideReflectionCamera, Texts.hideReflectionCamera);
        }

        void GetWaterProperties()
        {
            if (!selected) return;

#region Colors
            shallowColor = serializedObject.FindProperty("shallowColor");
            deepColor = serializedObject.FindProperty("deepColor");
            colorGradient = serializedObject.FindProperty("colorGradient");
            useColorGradient = serializedObject.FindProperty("useColorGradient");
            colorDepth = serializedObject.FindProperty("colorDepth");
            horizonColor = serializedObject.FindProperty("horizonColor");
            horizonDistanceColor = serializedObject.FindProperty("horizonDistanceColor");
            colorContrast = serializedObject.FindProperty("colorContrast");
            colorSaturation = serializedObject.FindProperty("colorSaturation");
#endregion

            underwaterColor = serializedObject.FindProperty("underwaterColor");
            underwaterContribution = serializedObject.FindProperty("underwaterContribution");
            refractionDepth = serializedObject.FindProperty("refractionDepth");
            reflectionStrength = serializedObject.FindProperty("reflectionStrength");

#region Surface Foam
            surfaceFoamColor1 = serializedObject.FindProperty("surfaceFoamColor1");
            surfaceFoamOffset = serializedObject.FindProperty("surfaceFoamOffset");
            surfaceFoamColor2 = serializedObject.FindProperty("surfaceFoamColor2");
            surfaceFoamTexture = serializedObject.FindProperty("surfaceFoamTexture");
            surfaceFoamCutoff = serializedObject.FindProperty("surfaceFoamCutoff");
            surfaceFoamDistortion = serializedObject.FindProperty("surfaceFoamDistortion");
            surfaceFoamScale = serializedObject.FindProperty("surfaceFoamScale");
            surfaceFoamMovement = serializedObject.FindProperty("surfaceFoamMovement");
#endregion

#region Intersection Foam
            intersectionFoamDepth = serializedObject.FindProperty("intersectionFoamDepth");
            intersectionFoamTexture = serializedObject.FindProperty("intersectionFoamTexture");
            intersectionFoamColor1 = serializedObject.FindProperty("intersectionFoamColor1");
            intersectionFoamColor2 = serializedObject.FindProperty("intersectionFoamColor2");
            intersectionFoamCutoff = serializedObject.FindProperty("intersectionFoamCutoff");
            intersectionFoamDistortion = serializedObject.FindProperty("intersectionFoamDistortion");
            shoreFade = serializedObject.FindProperty("shoreFade");
            shoreFadeColor = serializedObject.FindProperty("shoreFadeColor");
            intersectionFoamScale = serializedObject.FindProperty("intersectionFoamScale");
            intersectionFoamMovement = serializedObject.FindProperty("intersectionFoamMovement");
#endregion

#region Surface 
            roughness = serializedObject.FindProperty("roughness");
            normalsTexture = serializedObject.FindProperty("normalsTexture");
            normalsStrength = serializedObject.FindProperty("normalsStrength");
            normalsScale = serializedObject.FindProperty("normalsScale");
            normalsSpeed = serializedObject.FindProperty("normalsSpeed");
            surfaceNormals = serializedObject.FindProperty("surfaceNormals");
            renderScale = serializedObject.FindProperty("renderScale");
            reflectionLayer = serializedObject.FindProperty("reflectionLayer");
            reflectSkybox = serializedObject.FindProperty("reflectSkybox");
            reflectionPlaneOffset = serializedObject.FindProperty("reflectionPlaneOffset");
            lighting = serializedObject.FindProperty("lighting");
            enableReflections = serializedObject.FindProperty("enableReflections");
#endregion

#region Caustics
            causticsStrength = serializedObject.FindProperty("causticsStrength");
            causticsTexture = serializedObject.FindProperty("causticsTexture");
            causticsDepth = serializedObject.FindProperty("causticsDepth");
            causticsVisuals = serializedObject.FindProperty("causticsVisuals");
#endregion

#region Refraction
            refraction = serializedObject.FindProperty("refraction");
#endregion

#region Additional Settings
            hideComponents = serializedObject.FindProperty("hideComponents");
            hideReflectionCamera = serializedObject.FindProperty("hideReflectionCamera");
            useVertexColors = serializedObject.FindProperty("useVertexColors");
#endregion

#region Waves
            waveVisuals = serializedObject.FindProperty("waveVisuals");
            waveDirections = serializedObject.FindProperty("waveDirections");
#endregion

            isDesktopVariant = stylizedWater.desktopShader;
            shaderName = stylizedWater.shaderName;
            isLit = lighting.enumValueIndex == 1;
            useReflections = enableReflections.boolValue;
            usesColorGradient = useColorGradient.boolValue;
            usesVertexColors = useVertexColors.boolValue;
        }
    }
}
#endif