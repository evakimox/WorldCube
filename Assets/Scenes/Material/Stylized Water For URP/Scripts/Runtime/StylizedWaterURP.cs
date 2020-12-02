// Designed & Developed by Alexander Ameye
// https://alexander-ameye.gitbook.io/stylized-water/
// Version 1.0.3

#if UNIVERSAL_RENDERER
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace StylizedWaterForURP
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter)), ExecuteAlways]
    [HelpURL("https://alexanderameye.gitbook.io/stylized-water-for-urp/")]
    public class StylizedWaterURP : MonoBehaviour
    {
#region Planar Reflections
        [Range(0f, 1f)]
        public float reflectionStrength = 1f;
        [Range(0.05f, 1f)]
        public float renderScale = 1f;
        public LayerMask reflectionLayer = -1;
        public bool reflectSkybox;
        public GameObject reflectionTarget;
        [Range(-0.5f, 0.5f)]
        public float reflectionPlaneOffset;
        private Camera _reflectionCamera;
        public bool enableReflections;
        UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraData;
        private RenderTexture _reflectionTexture;
        RenderTextureDescriptor previousDescriptor;
        private readonly int _planarReflectionTextureId = Shader.PropertyToID("_PlanarReflectionTexture");
        public static event Action<ScriptableRenderContext, Camera> BeginPlanarReflections;
#endregion

#region Colors
        public bool useColorGradient;
        [GradientUsage(true)]
        public Gradient colorGradient;
        public Texture2D colorGradientTexture;
        [ColorUsage(true, true)]
        public Color shallowColor;
        [ColorUsage(true, true)]
        public Color deepColor;
        [Range(0f, 2f)]
        public float colorDepth;
        [ColorUsage(true, true)]
        public Color horizonColor;
        [Range(0f, 20f)]
        public float horizonDistanceColor;
        [ColorUsage(false, false)]
        public Color underwaterColor;
        [Range(0f, 1f)]
        public float underwaterContribution;
        [Range(0f, 5f)]
        public float colorContrast;
        [Range(0f, 5f)]
        public float colorSaturation;
#endregion

#region Surface Foam
        public Texture surfaceFoamTexture;
        [ColorUsage(true, false)]
        public Color surfaceFoamColor1;
        [ColorUsage(true, false)]
        public Color surfaceFoamColor2;
        public Vector2 surfaceFoamScale;
        [Range(0f, 1f)]
        public float surfaceFoamCutoff;
        [Range(0f, 3f)]
        public float surfaceFoamDistortion;
        public Vector4 surfaceFoamMovement;
        public Vector2 surfaceFoamOffset;
#endregion

#region Intersection Foam
        public Texture intersectionFoamTexture;
        [ColorUsage(true, false)]
        public Color intersectionFoamColor1;
        [ColorUsage(true, false)]
        public Color intersectionFoamColor2;
        public Vector2 intersectionFoamScale;
        [Range(0f, 1f)]
        public float intersectionFoamCutoff;
        [Range(0f, 3f)]
        public float intersectionFoamDistortion;
        public Vector4 intersectionFoamMovement;
        [Range(0f, 3f)]
        public float intersectionFoamDepth;
        [Range(0f, 3f)]
        public float shoreFade;
        [ColorUsage(true, false)]
        public Color shoreFadeColor;
#endregion

#region Surface
        public enum Lighting { Unlit, Lit };
        public Lighting lighting;
        [Range(0f, 1f)]
        public float roughness;
        public Texture normalsTexture;
        [Range(0f, 2f)]
        public float normalsStrength;
        [Range(0f, 5f)]
        public float normalsScale;
        [Range(0f, 0.2f)]
        public float normalsSpeed;
#endregion

#region Refraction
        public Vector3 refraction;
        [Range(0f, 10f)]
        public float refractionDepth;
#endregion

#region Caustics
        [Range(0f, 5f)]
        public float causticsStrength;
        public Texture causticsTexture;
        [Range(0f, 10f)]
        public float causticsDepth;
        public Vector4 causticsVisuals;
#endregion

#region Waves
        public Vector3 waveVisuals;
        public Vector4 waveDirections;
#endregion

#region Additional Settings
        public bool hideComponents = true;
        public enum WaterUVS { Mesh, WorldSpace };
        public WaterUVS UVS;
        public bool useVertexColors;
        public bool hideReflectionCamera;
#endregion

        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Material material;

        public bool desktopShader;
        public string shaderName;

        void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += ExecutePlanarReflections;
            reflectionTarget = this.gameObject;

            if (!meshRenderer) meshRenderer = this.GetComponent<MeshRenderer>();
            if (!meshFilter) meshFilter = this.GetComponent<MeshFilter>();

            material = meshRenderer.sharedMaterial;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

            if (material && meshRenderer && meshFilter && !Application.isPlaying)
            {
                meshRenderer.sharedMaterial.hideFlags = (hideComponents) ? HideFlags.HideInInspector : HideFlags.None;
                meshRenderer.hideFlags = (hideComponents) ? HideFlags.HideInInspector : HideFlags.None;
                meshFilter.hideFlags = (hideComponents) ? HideFlags.HideInInspector : HideFlags.None;
            }

            this.gameObject.layer = 4;
            reflectionLayer = ~(1 << 4);

            ReadMaterialProperties();
            WriteMaterialProperties();
        }

        private void ResetHideFlags()
        {
            if (material && meshRenderer && meshFilter && !Application.isPlaying)
            {
                meshRenderer.sharedMaterial.hideFlags = HideFlags.None;
                meshRenderer.hideFlags = HideFlags.None;
                meshFilter.hideFlags = HideFlags.None;
            }
        }

        private void OnDisable() 
        {
            RenderPipelineManager.beginCameraRendering -= ExecutePlanarReflections;
            Cleanup();
            ResetHideFlags();
        }
        
        private void OnDestroy()
        {
    
            RenderPipelineManager.beginCameraRendering -= ExecutePlanarReflections;
            Cleanup();
            ResetHideFlags();
        }
        
        private void Cleanup()
        {
            if (_reflectionCamera)
            {
                _reflectionCamera.targetTexture = null;
                SafeDestroyObject(_reflectionCamera.gameObject);
            }

            if (_reflectionTexture) RenderTexture.ReleaseTemporary(_reflectionTexture);
        }

        void SafeDestroyObject(UnityEngine.Object obj)
        {
            if (Application.isEditor) DestroyImmediate(obj);
            else Destroy(obj);
        }

        private void UpdateReflectionCamera(Camera realCamera)
        {
           if (_reflectionCamera == null) _reflectionCamera = InitializeReflectionCamera();

            Vector3 pos = Vector3.zero;
            Vector3 normal = Vector3.up;
      
            if (reflectionTarget != null)
            {
                pos = reflectionTarget.transform.position + Vector3.up * reflectionPlaneOffset;
                normal = reflectionTarget.transform.up;
            }

            UpdateCamera(realCamera, _reflectionCamera);
            _reflectionCamera.gameObject.hideFlags = (hideReflectionCamera) ? HideFlags.HideAndDontSave : HideFlags.DontSave;
            #if UNITY_EDITOR
            EditorApplication.DirtyHierarchyWindowSorting();
            #endif
            
            var d = -Vector3.Dot(normal, pos);
            var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            var reflection = Matrix4x4.identity;
            reflection *= Matrix4x4.Scale(new Vector3(1, -1, 1));

            PlanarReflections.CalculateReflectionMatrix(ref reflection, reflectionPlane);
            var oldPosition = realCamera.transform.position - new Vector3(0, pos.y * 2, 0);
            var newPosition = PlanarReflections.ReflectPosition(oldPosition);
            _reflectionCamera.transform.forward = Vector3.Scale(realCamera.transform.forward, new Vector3(1, -1, 1));
            _reflectionCamera.worldToCameraMatrix = realCamera.worldToCameraMatrix * reflection;

            var clipPlane = CameraSpacePlane(_reflectionCamera, pos - Vector3.up * 0.1f, normal, 1.0f);
            var projection = realCamera.CalculateObliqueMatrix(clipPlane);
            _reflectionCamera.projectionMatrix = projection;
            _reflectionCamera.cullingMask = reflectionLayer;
            _reflectionCamera.transform.position = newPosition;
        }

        private void UpdateCamera(Camera src, Camera dest)
        {
            if (dest == null) return;

            dest.CopyFrom(src);
            dest.useOcclusionCulling = false;

            if (dest.gameObject.TryGetComponent(out UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camData))
            {
                camData.renderShadows = false;
                if (reflectSkybox) dest.clearFlags = CameraClearFlags.Skybox;
                else
                {
                    dest.clearFlags = CameraClearFlags.SolidColor;
                    dest.backgroundColor = Color.black;
                }
            }
        }

        private Camera InitializeReflectionCamera()
        {
            var go = new GameObject("", typeof(Camera));
            go.name = "Reflection Camera [" + go.GetInstanceID() + "]";
            var camData = go.AddComponent(typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)) as UnityEngine.Rendering.Universal.UniversalAdditionalCameraData;

            camData.requiresColorOption = CameraOverrideOption.Off;
            camData.requiresDepthOption = CameraOverrideOption.Off;
            camData.SetRenderer(0);

            var t = transform;
            var rcam = go.GetComponent<Camera>();
            rcam.transform.SetPositionAndRotation(t.position, t.rotation);
            rcam.depth = -10;
            rcam.enabled = false;
        
            return rcam;
        }

        private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            var m = cam.worldToCameraMatrix;
            var cameraPosition = m.MultiplyPoint(pos);
            var cameraNormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cameraNormal.x, cameraNormal.y, cameraNormal.z, -Vector3.Dot(cameraPosition, cameraNormal));
        }

        RenderTextureDescriptor GetDescriptor(Camera camera, float pipelineRenderScale)
        {
            var width = (int)Mathf.Max(camera.pixelWidth * pipelineRenderScale * renderScale);
            var height = (int)Mathf.Max(camera.pixelHeight * pipelineRenderScale * renderScale);
            var hdr = camera.allowHDR;
            var renderTextureFormat = hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

            return new RenderTextureDescriptor(width, height, renderTextureFormat, 16)
            {
                autoGenerateMips = true,
                useMipMap = true
            };
        }

        private void ReflectionTexture(Camera camera)
        {
            var descriptor = GetDescriptor(camera, UniversalRenderPipeline.asset.renderScale);

            if (_reflectionTexture == null)
            {
                _reflectionTexture = RenderTexture.GetTemporary(descriptor);
                previousDescriptor = descriptor;
            }

            else if (!descriptor.Equals(previousDescriptor))
            {
                if (_reflectionTexture) RenderTexture.ReleaseTemporary(_reflectionTexture);
                _reflectionTexture = RenderTexture.GetTemporary(descriptor);
                previousDescriptor = descriptor;
            }
            _reflectionCamera.targetTexture = _reflectionTexture;
        }

        private void ExecutePlanarReflections(ScriptableRenderContext context, Camera camera)
        {
            if(!enableReflections)
            {
                Cleanup();
                return;
            }
            if (camera.cameraType == CameraType.Reflection || camera.cameraType == CameraType.Preview || material == null || material.shader.name == "Shader Graphs/Stylized Water For URP Mobile" ) return;

            UpdateReflectionCamera(camera);
            ReflectionTexture(camera);

            var data = new PlanarReflectionSettingData();
            data.Set();

            BeginPlanarReflections?.Invoke(context, _reflectionCamera);
            UniversalRenderPipeline.RenderSingleCamera(context, _reflectionCamera);

            data.Restore();
            Shader.SetGlobalTexture(_planarReflectionTextureId, _reflectionTexture);
        }

        class PlanarReflectionSettingData
        {
            private readonly bool _fog;
            private readonly int _maxLod;
            private readonly float _lodBias;

            public PlanarReflectionSettingData()
            {
                _fog = RenderSettings.fog;
                _maxLod = QualitySettings.maximumLODLevel;
                _lodBias = QualitySettings.lodBias;
            }

            public void Set()
            {
                GL.invertCulling = true;
                RenderSettings.fog = false;
                QualitySettings.maximumLODLevel = 1;
                QualitySettings.lodBias = _lodBias * 0.5f;
            }

            public void Restore()
            {
                GL.invertCulling = false;
                RenderSettings.fog = _fog;
                QualitySettings.maximumLODLevel = _maxLod;
                QualitySettings.lodBias = _lodBias;
            }
        }

        public void ReadMaterialProperties()
        {
            if (meshRenderer) material = meshRenderer.sharedMaterial;
            if (!material) return;
            shaderName = material.shader.name;
            desktopShader = shaderName == "Shader Graphs/Stylized Water For URP Desktop";
            if (!(shaderName == "Shader Graphs/Stylized Water For URP Desktop" || shaderName == "Shader Graphs/Stylized Water For URP Mobile")) return;

#region Colors
            shallowColor = material.GetColor("_ShallowColor");
            deepColor = material.GetColor("_DeepColor");
            colorDepth = material.GetFloat("_ColorDepth");
            horizonColor = material.GetColor("_HorizonColor");
            horizonDistanceColor = material.GetFloat("_HorizonDistanceColor");
            useColorGradient = (material.IsKeywordEnabled("COLOR_GRADIENT_ON")) ? true : false;
            colorContrast = material.GetFloat("_ColorContrast");
            colorSaturation = material.GetFloat("_ColorSaturation");
#endregion

#region Surface Foam
            surfaceFoamCutoff = material.GetFloat("_SurfaceFoamCutoff");
            surfaceFoamDistortion = material.GetFloat("_SurfaceFoamDistortion");
            surfaceFoamColor1 = material.GetColor("_SurfaceFoamColor1");
            surfaceFoamScale = material.GetVector("_SurfaceFoamScale");
            surfaceFoamMovement = material.GetVector("_SurfaceFoamMovement");
            surfaceFoamOffset = material.GetVector("_SurfaceFoamOffset");
            surfaceFoamColor2 = material.GetColor("_SurfaceFoamColor2");
            surfaceFoamTexture = material.GetTexture("_SurfaceFoamTexture");
#endregion

#region Intersection Foam
            intersectionFoamCutoff = material.GetFloat("_IntersectionFoamCutoff");
            intersectionFoamDistortion = material.GetFloat("_IntersectionFoamDistortion");
            intersectionFoamScale = material.GetVector("_IntersectionFoamScale");
            intersectionFoamColor1 = material.GetColor("_IntersectionFoamColor1");
            intersectionFoamColor2 = material.GetColor("_IntersectionFoamColor2");
            intersectionFoamTexture = material.GetTexture("_IntersectionFoamTexture");
            intersectionFoamMovement = material.GetVector("_IntersectionFoamMovement");
            intersectionFoamDepth = material.GetFloat("_IntersectionFoamDepth");
            shoreFadeColor = material.GetColor("_ShoreFadeColor");
            shoreFade = material.GetFloat("_ShoreFade");
#endregion

#region Caustics
            causticsStrength = material.GetFloat("_CausticsStrength");
            causticsTexture = material.GetTexture("_CausticsTexture");
            causticsVisuals = material.GetVector("_CausticsVisuals");
            causticsDepth = material.GetFloat("_CausticsDepth");
#endregion

#region Waves
            waveVisuals = material.GetVector("_WaveVisuals");
            waveDirections = material.GetVector("_WaveDirections");
#endregion

#region Additional Settings
            if (material.IsKeywordEnabled("VERTEX_COLORS_ON")) useVertexColors = true;
            else useVertexColors = false;
#endregion

            if (shaderName == "Shader Graphs/Stylized Water For URP Desktop")
            {
                if (material.IsKeywordEnabled("ADVANCED_LIGHTING_ON")) lighting = Lighting.Lit;
                else lighting = Lighting.Unlit;

#region Surface
                roughness = material.GetFloat("_Roughness");
                normalsTexture = material.GetTexture("_NormalsTexture");
                normalsStrength = material.GetFloat("_NormalsStrength");
                normalsScale = material.GetFloat("_NormalsScale");
                normalsSpeed = material.GetFloat("_NormalsSpeed");
#endregion

#region Planar Reflections
                reflectionStrength = material.GetFloat("_ReflectionStrength");
#endregion

#region Underwater Color
                underwaterColor = material.GetColor("_UnderwaterColor");
#endregion

#region Refraction
                refraction = material.GetVector("_Refraction");
                refractionDepth = material.GetFloat("_RefractionDepth");
#endregion
            }
        }

        public void WriteMaterialProperties()
        {
            if (!material) return;
            string shaderName = material.shader.name;
            if (!(shaderName == "Shader Graphs/Stylized Water For URP Desktop" || shaderName == "Shader Graphs/Stylized Water For URP Mobile")) return;

            if (material && meshRenderer && meshFilter && meshRenderer.sharedMaterial && !Application.isPlaying)
            {
                meshRenderer.sharedMaterial.hideFlags = (hideComponents) ? HideFlags.HideInInspector : HideFlags.None;
                meshRenderer.hideFlags = (hideComponents) ? HideFlags.HideInInspector : HideFlags.None;
                meshFilter.hideFlags = (hideComponents) ? HideFlags.HideInInspector : HideFlags.None;
            }

#region Colors
            if (useColorGradient) material.EnableKeyword("COLOR_GRADIENT_ON");
            else material.DisableKeyword("COLOR_GRADIENT_ON");
            material.SetColor("_ShallowColor", shallowColor);
            material.SetColor("_DeepColor", deepColor);
            material.SetFloat("_ColorDepth", colorDepth);
            material.SetColor("_HorizonColor", horizonColor);
            material.SetFloat("_HorizonDistanceColor", horizonDistanceColor);
            material.SetTexture("_ColorGradientTexture", colorGradientTexture);
            material.SetFloat("_ColorContrast", colorContrast);
            material.SetFloat("_ColorSaturation", colorSaturation);
#endregion

#region Surface Foam
            material.SetTexture("_SurfaceFoamTexture", surfaceFoamTexture);
            material.SetFloat("_SurfaceFoamCutoff", surfaceFoamCutoff);
            material.SetFloat("_SurfaceFoamDistortion", surfaceFoamDistortion);
            material.SetColor("_SurfaceFoamColor1", surfaceFoamColor1);
            material.SetVector("_SurfaceFoamScale", surfaceFoamScale);
            material.SetVector("_SurfaceFoamMovement", surfaceFoamMovement);
            material.SetVector("_SurfaceFoamOffset", surfaceFoamOffset);
            material.SetColor("_SurfaceFoamColor2", surfaceFoamColor2);
#endregion

#region Intersection Foam
            material.SetTexture("_IntersectionFoamTexture", intersectionFoamTexture);
            material.SetFloat("_IntersectionFoamCutoff", intersectionFoamCutoff);
            material.SetFloat("_IntersectionFoamDistortion", intersectionFoamDistortion);
            material.SetColor("_IntersectionFoamColor1", intersectionFoamColor1);
            material.SetColor("_IntersectionFoamColor2", intersectionFoamColor2);
            material.SetVector("_IntersectionFoamScale", intersectionFoamScale);
            material.SetVector("_IntersectionFoamMovement", intersectionFoamMovement);
            material.SetFloat("_IntersectionFoamDepth", intersectionFoamDepth);
            material.SetColor("_ShoreFadeColor", shoreFadeColor);
            material.SetFloat("_ShoreFade", shoreFade);
#endregion

#region Caustics
            material.SetFloat("_CausticsStrength", causticsStrength);
            material.SetTexture("_CausticsTexture", causticsTexture);
            material.SetVector("_CausticsVisuals", causticsVisuals);
            material.SetFloat("_CausticsDepth", causticsDepth);
#endregion

#region Waves
            material.SetVector("_WaveVisuals", waveVisuals);
            material.SetVector("_WaveDirections", waveDirections);
#endregion

#region Additional Settings
            if (useVertexColors) material.EnableKeyword("VERTEX_COLORS_ON");
            else material.DisableKeyword("VERTEX_COLORS_ON");
#endregion

            if (shaderName == "Shader Graphs/Stylized Water For URP Desktop")
            {
                if (lighting == Lighting.Lit) material.EnableKeyword("ADVANCED_LIGHTING_ON");
                else material.DisableKeyword("ADVANCED_LIGHTING_ON");

#region Surface
                material.SetFloat("_Roughness", roughness);
                material.SetTexture("_NormalsTexture", normalsTexture);
                material.SetFloat("_NormalsStrength", normalsStrength);
                material.SetFloat("_NormalsScale", normalsScale);
                material.SetFloat("_NormalsSpeed", normalsSpeed);
#endregion

#region Planar Reflections
                if(enableReflections) material.SetFloat("_ReflectionStrength", reflectionStrength);
                else material.SetFloat("_ReflectionStrength", 0);
#endregion

#region Underwater Color
                underwaterColor.a = underwaterContribution;
                material.SetColor("_UnderwaterColor", underwaterColor);
#endregion

#region Refraction
                material.SetVector("_Refraction", refraction);
                material.SetFloat("_RefractionDepth", refractionDepth);
#endregion
            }
        }
    }
}
#endif
