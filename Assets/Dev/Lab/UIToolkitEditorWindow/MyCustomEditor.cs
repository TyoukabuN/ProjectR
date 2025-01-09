using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Styles = PJR.Timeline.Editor.Styles;
using GUIUtil = PJR.Timeline.Editor.GUIUtil;
using UIControlUtil = PJR.Timeline.Editor.UIControlUtil;
using Animancer.Editor;
using PJR.Timeline.Editor;

namespace PJR.Timeline
{
    public class MyCustomEditor : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/MyCustomEditor #%Q")]
        public static void ShowExample()
        {
            MyCustomEditor wnd = GetWindow<MyCustomEditor>();
            wnd.titleContent = new GUIContent("MyCustomEditor");
        }

        public void CreateGUI()
        {
            Styles.ReloadStylesIfNeeded();

            VisualElement root = rootVisualElement;


            var layout = new VisualElement();
            layout.style.flexBasis = new StyleLength(new Length(0) { unit = (LengthUnit)2 });
            layout.style.flexShrink = 1;
            layout.style.flexGrow = 0;
            layout.style.flexDirection = FlexDirection.Row;
            layout.style.flexWrap = Wrap.Wrap;
            layout.name = "ToolBarButtonLayout";


            //Button button = UIControlUtil.GetButton("gotoBegininButton");
            Button gotoBegininButton = new Button();
            gotoBegininButton.name = "gotoBegininButton";
            gotoBegininButton.style.backgroundImage = new StyleBackground(Styles.gotoBeginingContent.image as Texture2D);
            gotoBegininButton.style.width = new StyleLength() { value = 27f };
            gotoBegininButton.style.height = new StyleLength() { value = 20f };
            gotoBegininButton.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>() { value = ScaleMode.ScaleToFit };
            gotoBegininButton.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log(123);
            },TrickleDown.TrickleDown);
            gotoBegininButton.SetMargin0();
            gotoBegininButton.SetBorderRadius0();
            layout.Add(gotoBegininButton);

            Button previousFrameContent = new Button();
            previousFrameContent.name = "previousFrameContent";
            previousFrameContent.style.backgroundImage = new StyleBackground(Styles.previousFrameContent.image as Texture2D);
            previousFrameContent.style.width = new StyleLength() { value = 27f };
            previousFrameContent.style.height = new StyleLength() { value = 20f };
            previousFrameContent.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>() { value = ScaleMode.ScaleToFit };
            previousFrameContent.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log(123);
            }, TrickleDown.TrickleDown);
            previousFrameContent.SetMargin0();
            previousFrameContent.SetBorderRadius0();
            layout.Add(previousFrameContent);


            root.Add(layout);
        }

        void SetupButtonHandler()
        {
            VisualElement root = rootVisualElement;
            var buttons = root.Query<Button>();
            buttons.ForEach(RegisterHandler);
        }

        void RegisterHandler(Button button)
        {
            button.RegisterCallback<ClickEvent>(PrintClikMsg);
        }

        private int _clickCount;
        void PrintClikMsg(ClickEvent evt)
        {
            var root = rootVisualElement;

            ++_clickCount;

            var button = evt.currentTarget as Button;
            var toggle = root.Q<Toggle>("toggle3") as Toggle;
            Debug.Log("Button was Clicked!" + (toggle.value ? $" Count:{_clickCount}" : string.Empty));
        }


        //public void CreateGUI()
        //{
        //    // Each editor window contains a root VisualElement object
        //    VisualElement root = rootVisualElement;

        //    // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //    VisualElement label = new Label("Hello World! From C#");
        //    root.Add(label);

        //    // Import UXML
        //    var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Dev/Lab/UIToolkitRes/MyCustomEditor.uxml");
        //    VisualElement labelFromUXML = visualTree.Instantiate();
        //    root.Add(labelFromUXML);

        //    // A stylesheet can be added to a VisualElement.
        //    // The style will be applied to the VisualElement and all of its children.
        //    var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dev/Lab/UIToolkitRes/MyCustomEditor.uss");
        //    VisualElement labelWithStyle = new Label("Hello World! With Style");
        //    labelWithStyle.styleSheets.Add(styleSheet);
        //    root.Add(labelWithStyle);
        //}
    }
}