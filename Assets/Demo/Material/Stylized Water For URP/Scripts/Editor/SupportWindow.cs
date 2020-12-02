// Designed & Developed by Alexander Ameye
// https://alexander-ameye.gitbook.io/stylized-water/
// Version 1.0.3

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace StylizedWaterForURP
{
    class RunOnImport : AssetPostprocessor
    {
        private static string registryKeyShowWindow = "ameye.stylizedwaterforurp.showsupportwindow";
        public static bool showWindow
        {
            get { return EditorPrefs.GetBool(registryKeyShowWindow, true); }
            set { EditorPrefs.SetBool(registryKeyShowWindow, value); }
        }

        static RunOnImport() => EditorApplication.update += CheckToOpenSupportWindow;

        static void CheckToOpenSupportWindow()
        {
            if (!EditorApplication.isUpdating && showWindow)
            {
                SupportWindow.ShowWindow();
                showWindow = false;
                EditorApplication.update -= CheckToOpenSupportWindow;
            }
        }
    }

    public class SupportWindow : EditorWindow
    {
        private VisualElement root;

        private VisualElement supportTab;
        private VisualElement aboutTab;
        private VisualElement acknowledgementsTab;
        private VisualElement errorsTab;

        private VisualTreeAsset support;
        private VisualTreeAsset about;
        private VisualTreeAsset acknowledgements;
        private VisualTreeAsset errors;

        private StyleSheet styleSheet;

        static ListRequest listRequest;
        static SearchRequest searchRequest;
        static AddRequest addRequest;

        static Label unityVersionLabel;
        static Label activeRendererLabel;
        static Label URPVersionLabel;
        static Label depthTextureLabel;
        static Label opaqueTextureLabel;

        static VisualElement unityVersionIcon;
        static VisualElement activeRendererIcon;
        static VisualElement URPVersionIcon;
        static VisualElement depthTextureIcon;
        static VisualElement opaqueTextureIcon;

        static Button unityVersionFix;
        static Button activeRendererFix;
        static Button URPVersionFix;
        static Button depthTextureFix;
        static Button opaqueTextureFix;

        static Button aboutButton;
        static Button supportButton;
        static Button configureButton;
        static Button acknowledgementsButton;

        static Texture2D neutral;
        static Texture2D positive;
        static Texture2D negative;

        [MenuItem("Tools/Stylized Water For URP/About and Support")]
        public static SupportWindow ShowWindow()
        {
            var window = GetWindow<SupportWindow>();
            window.minSize = new Vector2(400, 400);
            window.maxSize = new Vector2(400, 400);
            return window;
        }

        private void OnEnable()
        {
            titleContent.text = "Support";
            titleContent.image = EditorGUIUtility.IconContent("Settings").image;
            Init();
        }

        private void Init()
        {
            root = this.rootVisualElement;
            supportTab = new VisualElement();
            aboutTab = new VisualElement();
            errorsTab = new VisualElement();
            acknowledgementsTab = new VisualElement();

            support = GetUXML("Support");
            about = GetUXML("About");
            acknowledgements = GetUXML("Acknowledgements");
            errors = GetUXML("Find Errors");

            if (support) support.CloneTree(supportTab);
            if (about) about.CloneTree(aboutTab);
            if (acknowledgements) acknowledgements.CloneTree(acknowledgementsTab);
            if (errors) errors.CloneTree(errorsTab);

            root.Add(supportTab);
            root.Add(aboutTab);
            root.Add(acknowledgementsTab);
            root.Add(errorsTab);

            supportTab.style.display = DisplayStyle.None;
            aboutTab.style.display = DisplayStyle.None;
            errorsTab.style.display = DisplayStyle.Flex;
            acknowledgementsTab.style.display = DisplayStyle.None;

            unityVersionLabel = root.Q<Label>("Unity Version");
            activeRendererLabel = root.Q<Label>("Active Renderer");
            URPVersionLabel = root.Q<Label>("URP Version");
            depthTextureLabel = root.Q<Label>("Depth Texture");
            opaqueTextureLabel = root.Q<Label>("Opaque Texture");

            unityVersionIcon = root.Q<VisualElement>("Unity Version Icon");
            activeRendererIcon = root.Q<VisualElement>("Active Renderer Icon");
            URPVersionIcon = root.Q<VisualElement>("URP Version Icon");
            depthTextureIcon = root.Q<VisualElement>("Depth Texture Icon");
            opaqueTextureIcon = root.Q<VisualElement>("Opaque Texture Icon");

            unityVersionFix = root.Q<Button>("Unity Version Fix");
            activeRendererFix = root.Q<Button>("Active Renderer Fix");
            URPVersionFix = root.Q<Button>("URP Version Fix");
            depthTextureFix = root.Q<Button>("Depth Texture Fix");
            opaqueTextureFix = root.Q<Button>("Opaque Texture Fix");

            supportButton = root.Q<Button>("SupportButton");
            aboutButton = root.Q<Button>("AboutButton");
            acknowledgementsButton = root.Q<Button>("AcknowledgementsButton");
            configureButton = root.Q<Button>("ConfigureButton");

            neutral = Resources.Load<Texture2D>("Icons/Neutral");
            positive = Resources.Load<Texture2D>("Icons/Positive");
            negative = Resources.Load<Texture2D>("Icons/Negative");

            root.Query<Button>().ForEach((button) =>
               {
                   button.clickable.clickedWithEventInfo += Clickedbutton;
               });

            SetUnchecked();
        }

        public static StyleSheet GetStyleSheet(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(StyleSheet)} {name}");
            if (guids.Length == 0)
                return null;
            var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return sheet;
        }

        public static VisualTreeAsset GetUXML(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(VisualTreeAsset)} {name}");
            if (guids.Length == 0)
                return null;
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return uxml;
        }

        private void Clickedbutton(EventBase tab)
        {

            Button button = tab.target as Button;
            if (button.name == "SupportButton")
            {
                supportTab.style.display = DisplayStyle.Flex;
                aboutTab.style.display = DisplayStyle.None;
                errorsTab.style.display = DisplayStyle.None;
                acknowledgementsTab.style.display = DisplayStyle.None;
            }

            else if (button.name == "AboutButton")
            {
                supportTab.style.display = DisplayStyle.None;
                aboutTab.style.display = DisplayStyle.Flex;
                errorsTab.style.display = DisplayStyle.None;
                acknowledgementsTab.style.display = DisplayStyle.None;
            }

            else if (button.name == "ConfigureButton")
            {
                supportTab.style.display = DisplayStyle.None;
                aboutTab.style.display = DisplayStyle.None;
                errorsTab.style.display = DisplayStyle.Flex;
                acknowledgementsTab.style.display = DisplayStyle.None;
            }

            else if (button.name == "AcknowledgementsButton")
            {
                supportTab.style.display = DisplayStyle.None;
                aboutTab.style.display = DisplayStyle.None;
                errorsTab.style.display = DisplayStyle.None;
                acknowledgementsTab.style.display = DisplayStyle.Flex;
            }

            else if (button.name == "Check")
            {
                SetUnchecked();
                CheckUnityVersion();
            }

            else if (button.name == "Manual")
            {
                Application.OpenURL("https://alexander-ameye.gitbook.io/stylized-water-for-urp/support/troubleshooting");
            }

            else if (button.name == "Forum")
            {
                Application.OpenURL("https://forum.unity.com/threads/stylized-water-for-urp-desktop-mobile-released.846313/");
            }

            else if (button.name == "Contact")
            {
                Application.OpenURL("https://alexander-ameye.gitbook.io/stylized-water-for-urp/");
            }

            else if (button.name == "Review")
            {
                Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/stylized-water-for-urp-162025");
            }

            else if (button.name == "Twitter")
            {
                Application.OpenURL("https://twitter.com/alexanderameye");
            }

            else if (button.name == "Website")
            {
                Application.OpenURL("https://alexanderameye.github.io/");
            }

            else if (button.name == "Unity Version Fix")
            {
                Application.OpenURL("https://unity3d.com/get-unity/download");
            }

            else if (button.name == "Active Renderer Fix")
            {
                SettingsService.OpenProjectSettings("Project/Graphics");
            }

            else if (button.name == "Depth Texture Fix")
            {
#if UNIVERSAL_RENDERER
                RenderPipelineAsset activeRenderer = GraphicsSettings.currentRenderPipeline;
                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline = activeRenderer as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;

                EditorGUIUtility.PingObject(pipeline);
#endif
            }

            else if (button.name == "Opaque Texture Fix")
            {
#if UNIVERSAL_RENDERER
                RenderPipelineAsset activeRenderer = GraphicsSettings.currentRenderPipeline;
                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline = activeRenderer as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;

                EditorGUIUtility.PingObject(pipeline);
#endif
            }

            else if (button.name == "URP Version Fix")
            {
                addRequest = Client.Add("com.unity.render-pipelines.universal");
                EditorApplication.update += InstallURP;

                activeRendererIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                activeRendererFix.style.visibility = Visibility.Hidden;

                depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                depthTextureFix.style.visibility = Visibility.Hidden;

                opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                opaqueTextureFix.style.visibility = Visibility.Hidden;
            }

            else if (button.name == "KeenanWoodall")
            {
                Application.OpenURL("https://twitter.com/keenanwoodall");
            }

            else if (button.name == "JoshSauter")
            {
                Application.OpenURL("https://github.com/JoshSauter");
            }
        }

        private void SetUnchecked()
        {
            unityVersionLabel.text = "Untested";
            activeRendererLabel.text = "Untested";
            URPVersionLabel.text = "Untested";
            depthTextureLabel.text = "Untested";
            opaqueTextureLabel.text = "Untested";

            unityVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            activeRendererIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            URPVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

            unityVersionFix.style.visibility = Visibility.Hidden;
            activeRendererFix.style.visibility = Visibility.Hidden;
            URPVersionFix.style.visibility = Visibility.Hidden;
            depthTextureFix.style.visibility = Visibility.Hidden;
            opaqueTextureFix.style.visibility = Visibility.Hidden;
        }

        private void CheckUnityVersion()
        {
            string unityVersion = Application.unityVersion;
            unityVersionLabel.text = unityVersion;

            if (unityVersion.StartsWith("2019.3") || unityVersion.StartsWith("2020.1"))
            {
                unityVersionIcon.style.backgroundImage = Background.FromTexture2D(positive);
                CheckSRPVersion();
            }

            else
            {
                unityVersionIcon.style.backgroundImage = Background.FromTexture2D(negative);
                unityVersionFix.style.visibility = Visibility.Visible;
            }
        }

        private void CheckSRPVersion()
        {
            URPVersionLabel.text = "Testing ...";
            URPVersionIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            listRequest = Client.List();
            searchRequest = Client.Search("com.unity.render-pipelines.universal");
            EditorApplication.update += FindSRPVersion;
        }

        static void FindErrors()
        {
            activeRendererIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
            opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);

            RenderPipelineAsset activeRenderer = GraphicsSettings.currentRenderPipeline;
            if (activeRenderer == null)
            {
                activeRendererLabel.text = "Default";
                depthTextureLabel.text = "Untested";
                opaqueTextureLabel.text = "Untested";

                activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                activeRendererFix.style.visibility = Visibility.Visible;

                depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                depthTextureFix.style.visibility = Visibility.Hidden;

                opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                opaqueTextureFix.style.visibility = Visibility.Hidden;
            }

#if UNIVERSAL_RENDERER
            else if (activeRenderer.GetType().Name.Contains("UniversalRenderPipelineAsset"))
            {
                activeRendererLabel.text = "Universal";
                activeRendererIcon.style.backgroundImage = Background.FromTexture2D(positive);
                activeRendererFix.style.visibility = Visibility.Hidden;

                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline = activeRenderer as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
                if (pipeline.supportsCameraDepthTexture)
                {
                    depthTextureLabel.text = "Enabled";
                    depthTextureIcon.style.backgroundImage = Background.FromTexture2D(positive);
                    depthTextureFix.style.visibility = Visibility.Hidden;
                }

                else
                {
                    depthTextureLabel.text = "Disabled";
                    depthTextureIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    depthTextureFix.style.visibility = Visibility.Visible;
                }

                if (pipeline.supportsCameraOpaqueTexture)
                {
                    opaqueTextureLabel.text = "Enabled";
                    opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(positive);
                    opaqueTextureFix.style.visibility = Visibility.Hidden;
                }

                else
                {
                    opaqueTextureLabel.text = "Disabled";
                    opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(negative);
                    opaqueTextureFix.style.visibility = Visibility.Visible;
                }
            }
#endif
            else
            {
                activeRendererLabel.text = "High-Definition";
                depthTextureLabel.text = "Untested";
                opaqueTextureLabel.text = "Untested";

                activeRendererIcon.style.backgroundImage = Background.FromTexture2D(negative);
                activeRendererFix.style.visibility = Visibility.Hidden;

                depthTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                depthTextureFix.style.visibility = Visibility.Hidden;

                opaqueTextureIcon.style.backgroundImage = Background.FromTexture2D(neutral);
                opaqueTextureFix.style.visibility = Visibility.Hidden;
            }
        }

        static void FindSRPVersion()
        {
#if UNIVERSAL_RENDERER
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name == "com.unity.render-pipelines.universal")
                        {
                            var version = package.version;
                            URPVersionLabel.text = version;

                            if (version.StartsWith("7.1.2") || version.StartsWith("7.1.3") || version.StartsWith("7.1.4") || version.StartsWith("7.1.5") || version.StartsWith("7.1.6")
                            || version.StartsWith("7.1.7") || version.StartsWith("7.1.8") || version.StartsWith("7.2") || version.StartsWith("7.3") || version.StartsWith("7.4") || version.StartsWith("7.5"))
                            {
                                URPVersionIcon.style.backgroundImage = Background.FromTexture2D(positive);
                                URPVersionFix.style.visibility = Visibility.Hidden;
                                FindErrors();
                                break;
                            }
                            else
                            {
                                URPVersionIcon.style.backgroundImage = Background.FromTexture2D(negative);
                                URPVersionFix.style.visibility = Visibility.Visible;
                                break;
                            }
                        }
                    }

                }
                else if (listRequest.Status >= StatusCode.Failure)
                    Debug.Log(listRequest.Error.message);

                EditorApplication.update -= FindSRPVersion;
            }
#else
              URPVersionLabel.text = "Not Installed";
              URPVersionIcon.style.backgroundImage = Background.FromTexture2D(negative);
              URPVersionFix.style.visibility = Visibility.Visible;
#endif
        }

        static void InstallURP()
        {
            URPVersionLabel.text = "Installing ...";
            URPVersionFix.style.visibility = Visibility.Hidden;

            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                    Debug.Log("Installed: " + addRequest.Result.packageId);
                else if (addRequest.Status >= StatusCode.Failure)
                    Debug.Log(addRequest.Error.message);

                EditorApplication.update -= InstallURP;
            }
        }
    }
}
