// GUIStyleの名前部分は選択してコピーができます。

// キー入力でウィンドウの操作が可能です。
// 左右キー：ページの移動
// 上下キー：スクロールの移動
// Enter：検索にフォーカスを移動
// Shift + 左右キー：GUIStyleの表示数を変更
// Ctrl + 左右キー：GUIStyleの表示分割数を変更
// Shift + Tab：Toolbarの切り替え

using GUIStyleViewer.DrawStyle;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GUIStyleViewer
{
    class DrawTool
    {
        public string ToolbarTitle { get; private set; }
        public IGUIDrawStyle GUIDrawerStyle { get; private set; }

        public DrawTool(string toolbarTitle, IGUIDrawStyle guiDrawerStyle)
        {
            ToolbarTitle = toolbarTitle;
            GUIDrawerStyle = guiDrawerStyle;
        }
    }

    public class GUIStyleViewer : EditorWindow
    {
        [MenuItem("Tools/GUIStyleの確認")]
        static void ShowWindow()
        {
            var window = GetWindow<GUIStyleViewer>();
            window.titleContent.text = nameof(GUIStyleViewer);
            window.Show();
        }

        GUIStyle _searchTextFieldStyle;
        Vector2 _searchTextFieldStyleSize;

        GUIStyle _searchCancelButtonEmptyStyle;
        Vector2 _searchCancelButtonEmptyStyleSize;

        GUIStyle _searchCancelButtonStyle;
        Vector2 _searchCancelButtonStyleSize;

        IEnumerable<GUIStyle> _searchGUIStyle;
        List<GUIStyle> _guiStyleList;

        Font _editorDefaultFont;

        DrawTool[] _drawTools = new DrawTool[]
        {
            new DrawTool("Label", new DrawLabel()),
            new DrawTool("Toggle", new DrawToggle()),
        };
        DrawTool _drawTool;

        string[] _toolbarTitles;
        int _drawToolIndex;

        int _currentPage;
        int _lastPage;

        static readonly int[] _ViewCounts = new int[] { 10, 20, 50, 100, 999 };
        int _viewCount = _ViewCounts[0];
        int _viewCountIndex;
        int _contentCount;

        void OnGUI()
        {
            Initialize();

            UpdateKeyEvent(Event.current);

            using (new EditorGUILayout.VerticalScope("AvatarMappingBox"))
            {
                _drawToolIndex = GUILayout.Toolbar(_drawToolIndex, _toolbarTitles);
                _drawTool = _drawTools[_drawToolIndex];

                DrawSearchWithOutString(out var searchStr);
                DrawContentCountEditView();
            }

            _drawTool.GUIDrawerStyle.Update(position.size);

            Draw(_drawTool.GUIDrawerStyle);
        }

        [System.NonSerialized] bool _initialized = false;
        void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _searchTextFieldStyle = "SearchTextField";
            _searchTextFieldStyleSize = _searchTextFieldStyle.CalcSize(GUIContent.none);

            _searchCancelButtonEmptyStyle = "SearchCancelButtonEmpty";
            _searchCancelButtonEmptyStyleSize = _searchTextFieldStyle.CalcSize(GUIContent.none);

            _searchCancelButtonStyle = "SearchCancelButton";
            _searchCancelButtonStyleSize = _searchCancelButtonStyle.CalcSize(GUIContent.none);
            _editorDefaultFont = (Font)EditorGUIUtility.Load(UnityEditor.Experimental.EditorResources.fontsPath + "Lucida Grande.ttf");

            _searchGUIStyle = GUI.skin.customStyles.Where(style => Regex.IsMatch(style.name, $".*{_searchStr}.*", RegexOptions.IgnoreCase));
            _guiStyleList = new List<GUIStyle>(_searchGUIStyle);

            ResetContentCount();

            _toolbarTitles = _drawTools.Select(tool => tool.ToolbarTitle).ToArray();

            foreach (var drawTool in _drawTools)
            {
                drawTool.GUIDrawerStyle.Initialize();
                if (drawTool.GUIDrawerStyle is IDrawLabel label)
                {
                    label.Initialize(_editorDefaultFont);
                }
            }

            _initialized = true;
        }

        void UpdateKeyEvent(Event guiEvent)
        {
            if (guiEvent.type == EventType.KeyUp)
            {
                if (guiEvent.keyCode == KeyCode.LeftArrow)
                {
                    if (guiEvent.shift)
                    {
                        _viewCountIndex = Mathf.Clamp(_viewCountIndex - 1, 0, _ViewCounts.Length - 1);
                        _viewCount = _ViewCounts[_viewCountIndex];
                        ResetContentCount();
                    }
                    else if (guiEvent.control)
                    {
                        _contentDivision = Mathf.Clamp(_contentDivision - 1, 1, 4);
                    }
                    else
                    {
                        _currentPage = (int)Mathf.Repeat(_currentPage - 1, _lastPage + 1);
                        _scrollPos = Vector2.zero;
                    }
                    guiEvent.Use();
                }
                else if (guiEvent.keyCode == KeyCode.RightArrow)
                {
                    if (guiEvent.shift)
                    {
                        _viewCountIndex = Mathf.Clamp(_viewCountIndex + 1, 0, _ViewCounts.Length - 1);
                        _viewCount = _ViewCounts[_viewCountIndex];
                        ResetContentCount();
                    }
                    else if (guiEvent.control)
                    {
                        _contentDivision = Mathf.Clamp(_contentDivision + 1, 1, 4);
                    }
                    else
                    {
                        _currentPage = (int)Mathf.Repeat(_currentPage + 1, _lastPage + 1);
                        _scrollPos = Vector2.zero;
                    }
                    guiEvent.Use();
                }
                else if (guiEvent.keyCode == KeyCode.Return)
                {
                    EditorGUI.FocusTextInControl(_SerachFieldControlName);
                    guiEvent.Use();
                }
                else if (guiEvent.keyCode == KeyCode.Tab && guiEvent.shift)
                {
                    _drawToolIndex = (int)Mathf.Repeat(_drawToolIndex + 1, _toolbarTitles.Length);
                    guiEvent.Use();
                }
            }
            else if (guiEvent.type == EventType.KeyDown)
            {
                if (guiEvent.keyCode == KeyCode.UpArrow)
                {
                    _scrollPos -= Vector2.up * EditorGUIUtility.singleLineHeight * 2;
                    guiEvent.Use();
                }
                else if (guiEvent.keyCode == KeyCode.DownArrow)
                {
                    _scrollPos += Vector2.up * EditorGUIUtility.singleLineHeight * 2;
                    guiEvent.Use();
                }
            }
        }

        Vector2 _scrollPos;
        int _contentDivision = 1;
        void Draw(IGUIDrawStyle drawer)
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
            {
                float width = position.width - 25 * _contentDivision;
                float divisionWidth = width / _contentDivision;

                foreach (var divStyleGroup in _guiStyleList.Select((style, index) => (style, index))
                    .Skip(_currentPage * _viewCount)
                    .Take(_viewCount)
                    .GroupBy(entry => entry.index / _contentDivision, entry => entry.style))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(divisionWidth)))
                    {
                        foreach (var style in divStyleGroup)
                        {
                            drawer.GetContentHeight(style, out var height);

                            using (new EditorGUILayout.VerticalScope("Badge", GUILayout.Width(divisionWidth), GUILayout.Height(height + EditorGUIUtility.singleLineHeight)))
                            {
                                EditorGUILayout.SelectableLabel(style.name, GUILayout.Width(divisionWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                drawer.DrawContent(style, divisionWidth, height);
                            }
                        }
                    }
                }

                GUILayout.Space(6);
                _scrollPos = scroll.scrollPosition;
            }

            using (new EditorGUILayout.HorizontalScope())
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                GUILayout.FlexibleSpace();
                _contentDivision = (int)GUILayout.HorizontalSlider(_contentDivision, 1, 4, GUILayout.Width(50));
                GUILayout.Label($"分割数 x{_contentDivision}");

                if (changed.changed)
                {
                    ResetContentCount();
                }
            }
        }

        GUIStyle _labelCenterStyle;
        void DrawContentCountEditView()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"ヒット数:{_contentCount}", GUILayout.Width(80));
                GUILayout.FlexibleSpace();

                if (EditorGUILayout.Toggle(false, "AC LeftArrow", GUILayout.Width(15)))
                {
                    _currentPage = Mathf.Max(0, _currentPage - 1);
                    _scrollPos = Vector2.zero;
                }

                _labelCenterStyle = _labelCenterStyle ?? new GUIStyle(GUI.skin.label);
                _labelCenterStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField($"{_currentPage + 1}/{_lastPage + 1}", _labelCenterStyle, GUILayout.Width(38));

                if (EditorGUILayout.Toggle(false, "AC RightArrow", GUILayout.Width(15)))
                {
                    _currentPage = Mathf.Min(_lastPage, _currentPage + 1);
                    _scrollPos = Vector2.zero;
                }
                GUILayout.Space(6);

                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    _viewCountIndex = (int)GUILayout.HorizontalSlider(_viewCountIndex, 0, _ViewCounts.Length - 1, GUILayout.Width(50));
                    _viewCount = _ViewCounts[_viewCountIndex];
                    GUILayout.Label($"表示数:{_viewCount}", GUILayout.Width(65));

                    if (changed.changed)
                    {
                        ResetContentCount();
                    }
                }
            }
        }

        const string _SerachFieldControlName = "SerachFieldControlName";
        string _searchStr;
        void DrawSearchWithOutString(out string outSearchStr)
        {
            var rect = GUILayoutUtility.GetRect(_searchTextFieldStyleSize.x, _searchTextFieldStyleSize.y);
            rect.x += 4;
            rect.width -= _searchCancelButtonEmptyStyleSize.x;
            GUI.SetNextControlName(_SerachFieldControlName);
            outSearchStr = EditorGUI.TextField(rect, _searchStr, _searchTextFieldStyle);

            rect.x += rect.width - 0.5f;
            rect.width = _searchCancelButtonEmptyStyleSize.x;
            if (GUI.Button(rect, "", outSearchStr == "" ? _searchCancelButtonEmptyStyle : _searchCancelButtonStyle))
            {
                if (outSearchStr != "")
                {
                    outSearchStr = "";
                    GUIUtility.keyboardControl = 0;
                }
            }

            if (_searchStr != outSearchStr)
            {
                _searchStr = outSearchStr;
                _guiStyleList.Clear();
                _guiStyleList.AddRange(_searchGUIStyle);

                ResetContentCount();
            }
        }

        void ResetContentCount()
        {
            _currentPage = 0;
            _contentCount = _guiStyleList.Count;
            _lastPage = (_contentCount - 1) / _viewCount;

            _scrollPos = Vector2.zero;
        }
    }
}

namespace GUIStyleViewer.DrawStyle
{
    interface IDrawLabel
    {
        void Initialize(Font font);
    }

    interface IGUIDrawStyle
    {
        void Initialize();
        void Update(Vector2 windowSize);

        void GetContentHeight(GUIStyle style, out float height);
        void DrawContent(GUIStyle style, float width);
        void DrawContent(GUIStyle style, float width, float height);
    }

    class DrawToggle : IGUIDrawStyle
    {
        bool _active;
        Vector2 _windowSize;
        GUIContent _content = GUIContent.none;

        public void Initialize() => _active = false;

        public void Update(Vector2 windowSize) => _windowSize = windowSize;

        public void GetContentHeight(GUIStyle style, out float height)
        {
            float textHeight = Mathf.Max(style.lineHeight, style.fontSize);
            height = style.CalcHeight(_content, _windowSize.x);
            height = Mathf.Max(height, textHeight);
            height = Mathf.Max(height, style.border.top);
        }

        public void DrawContent(GUIStyle style, float width, float height)
        {
            _active = EditorGUILayout.Toggle(_active.ToString(), _active, style, GUILayout.Width(width), GUILayout.Height(height));
        }

        public void DrawContent(GUIStyle style, float width)
        {
            GetContentHeight(style, out var height);
            DrawContent(style, width, height);
        }
    }

    class DrawLabel : IGUIDrawStyle, IDrawLabel
    {
        GUIContent _content = new GUIContent();

        Font _editorDefaultFont;
        Vector2 _windowSize;

        TextGenerator _textGenerator;
        TextGenerationSettings _textSettings;

        public void Initialize(Font font) => _editorDefaultFont = font;

        public void Initialize()
        {
            _textGenerator = new TextGenerator();

            _textSettings = new TextGenerationSettings();
            _textSettings.generateOutOfBounds = false;
            _textSettings.scaleFactor = 1;
            _textSettings.pivot = Vector2.one * 0.5f;
        }

        public void Update(Vector2 windowSize) => _windowSize = windowSize;

        int GetWordWrapLineCount(GUIStyle style, string text)
        {
            _textSettings.font = style.font ?? _editorDefaultFont;
            _textSettings.fontSize = style.fontSize == 0 ? _editorDefaultFont.fontSize : style.fontSize;
            _textSettings.fontStyle = style.fontStyle;

            _textSettings.textAnchor = style.alignment;
            _textSettings.generationExtents = new Vector2(style.fixedWidth, style.fixedHeight);

            _textSettings.horizontalOverflow = style.stretchWidth ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap;
            _textSettings.verticalOverflow = style.stretchHeight ? VerticalWrapMode.Overflow : VerticalWrapMode.Truncate;

            _textGenerator.Populate(text, _textSettings);
            return _textGenerator.lineCount;
        }

        public void GetContentHeight(GUIStyle style, out float height)
        {
            _content.text = style.name;

            float fontSize = style.fontSize > 0 ? style.fontSize : _editorDefaultFont.fontSize;
            float labelHeight = fontSize;

            if (style.wordWrap && !style.stretchWidth && !style.isHeightDependantOnWidth && style.imagePosition != ImagePosition.ImageOnly)
            {
                labelHeight = GetWordWrapLineCount(style, _content.text) * style.lineHeight;
            }

            height = style.CalcHeight(_content, style.fixedWidth);
            height = Mathf.Max(height, labelHeight);
            height = Mathf.Max(height, style.border.top);
        }

        public void DrawContent(GUIStyle style, float width, float height)
        {
            EditorGUILayout.LabelField(style.name, style, GUILayout.Width(width), GUILayout.Height(height));
        }

        public void DrawContent(GUIStyle style, float width)
        {
            GetContentHeight(style, out var height);
            DrawContent(style, height);
        }
    }
}