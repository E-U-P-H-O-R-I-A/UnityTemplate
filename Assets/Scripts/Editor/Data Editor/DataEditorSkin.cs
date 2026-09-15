using UnityEditor;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public sealed partial class DataEditorSkin
    {
        public const float TOOLBAR_HEIGHT = 32f;
        public const float FOOTER_HEIGHT = 22f;

        public readonly Color WindowBackground;
        public readonly Color ToolbarBackground;
        public readonly Color Border;
        public readonly Color Separator;
        public readonly Color Accent;
        public readonly Color RowHover;
        public readonly Color RowSelected;
        public readonly Color PanelFill;

        public readonly GUIStyle Body;
        public readonly GUIStyle Toolbar;
        public readonly GUIStyle ToolbarTitle;
        public readonly GUIStyle Tab;
        public readonly GUIStyle TabHover;
        public readonly GUIStyle TabActive;
        public readonly GUIStyle Panel;
        public readonly GUIStyle PanelHeader;
        public readonly GUIStyle PanelTitle;
        public readonly GUIStyle ScrollContent;
        public readonly GUIStyle MetaLabel;
        public readonly GUIStyle RowTitle;
        public readonly GUIStyle RowTitleActive;
        public readonly GUIStyle RowSubtitle;
        public readonly GUIStyle RowEven;
        public readonly GUIStyle RowOdd;
        public readonly GUIStyle RowSelectedCard;
        public readonly GUIStyle RowHoverCard;
        public readonly Texture2D Dot;
        public readonly GUIStyle Section;
        public readonly GUIStyle SectionContent;
        public readonly GUIStyle ListHeader;
        public readonly GUIStyle SectionFoldout;
        public readonly GUIStyle NestedFoldout;
        public readonly GUIStyle Button;
        public readonly GUIStyle PrimaryButton;
        public readonly GUIStyle DangerButton;
        public readonly GUIStyle IconButton;
        public readonly GUIStyle PrimaryIconButton;
        public readonly GUIStyle DangerIconButton;
        public readonly GUIStyle SmallButton;
        public readonly GUIContent ImportIcon;
        public readonly GUIContent ExportIcon;
        public readonly GUIContent RevertIcon;
        public readonly GUIContent SaveIcon;
        public readonly GUIContent DeleteIcon;
        public readonly GUIContent DeleteAllIcon;
        public readonly GUIContent RefreshIcon;
        public readonly GUIContent FolderIcon;
        public readonly GUIContent AddIcon;
        public readonly GUIContent RemoveIcon;
        public readonly GUIStyle Footer;
        public readonly GUIStyle FooterLabel;
        public readonly GUIStyle FooterAccentLabel;
        public readonly GUIStyle EmptyLabel;
        public readonly GUIStyle InlineHint;

        public DataEditorSkin()
        {
            bool dark = EditorGUIUtility.isProSkin;

            WindowBackground = Rgb(dark ? 0x1E1F22 : 0xC6C6C8);
            ToolbarBackground = Rgb(dark ? 0x26272B : 0xD6D6D8);
            Border = Rgb(dark ? 0x141517 : 0xA4A4A6);
            Separator = Rgb(dark ? 0x35373C : 0xBDBDBF);
            Accent = Rgb(dark ? 0x4C8DFF : 0x2B66E0);
            RowHover = new Color(1f, 1f, 1f, dark ? 0.05f : 0.28f);
            RowSelected = new Color(Accent.r, Accent.g, Accent.b, dark ? 0.20f : 0.24f);

            PanelFill = Rgb(dark ? 0x2B2D33 : 0xE0E0E2);
            var panelFill = PanelFill;
            var sectionFill = Rgb(dark ? 0x32353B : 0xD5D5D8);
            var buttonFill = Rgb(dark ? 0x3A3D44 : 0xECECEE);
            var buttonHover = Rgb(dark ? 0x454951 : 0xF6F6F8);
            var buttonActive = Rgb(dark ? 0x2F3238 : 0xD2D2D4);
            var danger = Rgb(dark ? 0x4A3336 : 0xE8D5D5);
            var dangerHover = Rgb(dark ? 0x5C3B3F : 0xF2DCDC);
            var dangerText = Rgb(dark ? 0xE88B8B : 0x8E2B2B);

            var text = Rgb(dark ? 0xD2D3D6 : 0x222224);
            var textStrong = Rgb(dark ? 0xF0F0F2 : 0x141416);
            var textDim = Rgb(dark ? 0x868990 : 0x606064);

            Body = new GUIStyle { padding = new RectOffset(8, 8, 0, 6) };

            Toolbar = new GUIStyle
            {
                fixedHeight = TOOLBAR_HEIGHT,
                padding = new RectOffset(10, 8, 5, 5)
            };

            ToolbarTitle = Label(textStrong, 13, FontStyle.Bold);
            ToolbarTitle.alignment = TextAnchor.MiddleLeft;

            Tab = Card(Color.clear, Color.clear, 6, new RectOffset(10, 10, 0, 0));
            Tab.alignment = TextAnchor.MiddleCenter;
            Tab.fontSize = 12;
            Tint(Tab, textDim);

            TabHover = Card(RowHover, Color.clear, 6, new RectOffset(10, 10, 0, 0));
            TabHover.alignment = TextAnchor.MiddleCenter;
            TabHover.fontSize = 12;
            Tint(TabHover, text);

            TabActive = Card(panelFill, Border, 6, new RectOffset(10, 10, 0, 0));
            TabActive.alignment = TextAnchor.MiddleCenter;
            TabActive.fontSize = 12;
            TabActive.fontStyle = FontStyle.Bold;
            Tint(TabActive, textStrong);

            Panel = Card(panelFill, Border, 6, new RectOffset(0, 0, 2, 2));
            Panel.margin = new RectOffset(0, 0, 0, 0);
            PanelHeader = new GUIStyle { padding = new RectOffset(10, 8, 6, 6) };
            PanelTitle = Label(textStrong, 12, FontStyle.Bold);
            PanelTitle.alignment = TextAnchor.MiddleLeft;
            ScrollContent = new GUIStyle { padding = new RectOffset(4, 4, 4, 6) };

            MetaLabel = Label(textDim, 11, FontStyle.Normal);
            MetaLabel.alignment = TextAnchor.MiddleLeft;
            MetaLabel.padding = new RectOffset(0, 0, 0, 0);

            RowTitle = Label(text, 12, FontStyle.Normal);
            RowTitleActive = Label(textStrong, 12, FontStyle.Bold);
            RowSubtitle = Label(textDim, 10, FontStyle.Normal);

            RowEven = new GUIStyle { padding = new RectOffset(2, 2, 1, 1) };
            RowOdd = new GUIStyle
            {
                padding = new RectOffset(2, 2, 1, 1),
                normal = { background = Solid(new Color(1f, 1f, 1f, dark ? 0.028f : 0.16f)) }
            };

            RowSelectedCard = Card(RowSelected, new Color(Accent.r, Accent.g, Accent.b, dark ? 0.55f : 0.5f),
                6, new RectOffset(0, 0, 0, 0));
            RowHoverCard = Card(RowHover, Color.clear, 6, new RectOffset(0, 0, 0, 0));
            Dot = Rounded(Accent, Color.clear, 12);

            Section = Card(sectionFill, Border, 6, new RectOffset(6, 6, 4, 6));
            Section.margin = new RectOffset(0, 0, 0, 6);

            SectionContent = new GUIStyle { padding = new RectOffset(4, 6, 0, 2) };

            ListHeader = Label(textStrong, 12, FontStyle.Normal);
            ListHeader.alignment = TextAnchor.MiddleLeft;

            SectionFoldout = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold, fontSize = 12 };
            Tint(SectionFoldout, textStrong);

            NestedFoldout = new GUIStyle(EditorStyles.foldout) { fontSize = 12 };
            Tint(NestedFoldout, text);

            Button = ActionButton(buttonFill, buttonHover, buttonActive, text, Border);
            PrimaryButton = ActionButton(Accent,
                Color.Lerp(Accent, Color.white, 0.15f),
                Color.Lerp(Accent, Color.black, 0.15f),
                Color.white,
                Color.Lerp(Accent, Color.black, 0.25f));
            DangerButton = ActionButton(danger, dangerHover, danger, dangerText, Border);

            IconButton = IconVariant(Button);
            PrimaryIconButton = IconVariant(PrimaryButton);
            DangerIconButton = IconVariant(DangerButton);

            SmallButton = new GUIStyle(Button)
            {
                fontSize = 10,
                fixedHeight = 18f,
                padding = new RectOffset(6, 6, 0, 0),
                margin = new RectOffset(2, 2, 1, 1)
            };

            ImportIcon = new GUIContent(TrayArrowIcon(false, text), "Import JSON from clipboard");
            ExportIcon = new GUIContent(TrayArrowIcon(true, text), "Copy JSON to clipboard");
            RevertIcon = Icon("UndoHistory", "R", "Discard unsaved changes");
            SaveIcon = Icon("SaveAs", "S", "Save selected file");
            DeleteIcon = Icon("TreeEditor.Trash", "D", "Delete selected file");
            DeleteAllIcon = Icon("CrossIcon", "X", "Delete all save files");
            RefreshIcon = Icon("Refresh", "R", "Refresh file list");
            FolderIcon = Icon("FolderOpened Icon", "F", "Reveal folder in explorer");
            AddIcon = Icon("Toolbar Plus", "+", "Add");
            RemoveIcon = Icon("Toolbar Minus", "-", "Remove");

            Footer = new GUIStyle
            {
                fixedHeight = FOOTER_HEIGHT,
                padding = new RectOffset(10, 10, 4, 4)
            };

            FooterLabel = Label(textDim, 11, FontStyle.Normal);
            FooterAccentLabel = Label(Accent, 11, FontStyle.Bold);

            EmptyLabel = Label(textDim, 12, FontStyle.Normal);
            EmptyLabel.alignment = TextAnchor.MiddleCenter;

            InlineHint = Label(textDim, 11, FontStyle.Italic);
        }

        private static Color Rgb(int hex) =>
            new Color(((hex >> 16) & 0xFF) / 255f, ((hex >> 8) & 0xFF) / 255f, (hex & 0xFF) / 255f);

        private static GUIStyle Label(Color color, int fontSize, FontStyle fontStyle)
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                fontSize = fontSize,
                fontStyle = fontStyle,
                wordWrap = false,
                clipping = TextClipping.Clip
            };

            Tint(style, color);
            return style;
        }

        private static void Tint(GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            style.onNormal.textColor = color;
            style.hover.textColor = color;
            style.onHover.textColor = color;
            style.active.textColor = color;
            style.onActive.textColor = color;
            style.focused.textColor = color;
            style.onFocused.textColor = color;
        }

        private GUIStyle Card(Color fill, Color border, int radius, RectOffset padding)
        {
            int slice = Mathf.Min(radius + 2, 11);

            return new GUIStyle
            {
                normal = { background = Rounded(fill, border, radius) },
                border = new RectOffset(slice, slice, slice, slice),
                padding = padding,
                margin = new RectOffset(0, 0, 0, 0)
            };
        }

        private GUIStyle ActionButton(Color fill, Color hover, Color active, Color text, Color border)
        {
            var style = Card(fill, border, 5, new RectOffset(8, 8, 0, 0));

            style.hover.background = Rounded(hover, border, 5);
            style.active.background = Rounded(active, border, 5);
            style.focused.background = style.normal.background;
            style.onNormal.background = style.normal.background;
            style.onHover.background = style.hover.background;
            style.onActive.background = style.active.background;

            Tint(style, text);

            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 11;
            style.fixedHeight = 22f;
            style.margin = new RectOffset(2, 2, 0, 0);

            return style;
        }

        private static GUIStyle IconVariant(GUIStyle source)
        {
            return new GUIStyle(source)
            {
                imagePosition = ImagePosition.ImageOnly,
                padding = new RectOffset(7, 7, 5, 5),
                margin = new RectOffset(2, 2, 0, 0),
                fixedWidth = 30f,
                fixedHeight = 26f
            };
        }

        private static GUIContent Icon(string iconName, string fallbackText, string tooltip)
        {
            var texture = LoadIcon(iconName);

            return texture != null
                ? new GUIContent(texture, tooltip)
                : new GUIContent(fallbackText, tooltip);
        }

        private static Texture LoadIcon(string iconName)
        {
            if (EditorGUIUtility.isProSkin)
            {
                var darkIcon = EditorGUIUtility.FindTexture($"d_{iconName}");
                if (darkIcon != null)
                    return darkIcon;
            }

            return EditorGUIUtility.FindTexture(iconName);
        }
    }
}
