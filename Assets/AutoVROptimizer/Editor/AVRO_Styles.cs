/*
Copyright (c) 2025 Valem Studio

This asset is the intellectual property of Valem Studio and is distributed under the Unity Asset Store End User License Agreement (EULA).

Unauthorized reproduction, modification, or redistribution of any part of this asset outside the terms of the Unity Asset Store EULA is strictly prohibited.

For support or inquiries, please contact Valem Studio via social media or through the publisher profile on the Unity Asset Store.
*/

using UnityEngine;
using UnityEditor;
namespace AVRO
{
    public class AVRO_Styles
    {
        public static GUIStyle CenteredText
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.alignment = TextAnchor.MiddleCenter;
                _style.fontSize = 12;
                return _style;
            }
        }

        public static GUIStyle Title
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.fontSize = 14;
                _style.fontStyle = FontStyle.Bold;
                _style.alignment = TextAnchor.UpperLeft;
                _style.padding = new RectOffset(4, 4, 8, 8);
                _style.margin = new RectOffset(0, 0, 2, 2);
                _style.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.1f));
                return _style;
            }
        }

        public static GUIStyle GreyTitle
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.fontSize = 12;
                _style.fontStyle = FontStyle.Bold;
                _style.alignment = TextAnchor.UpperLeft;
                _style.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                _style.padding = new RectOffset(2, 2, 4, 4);
                _style.margin = new RectOffset(0, 0, 2, 2);
                _style.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.1f));
                return _style;
            }
        }
#pragma warning disable UDR0001 // Domain Reload Analyzer
        public static GUIStyle TicketStyle;
#pragma warning restore UDR0001 // Domain Reload Analyzer
        public static GUIStyle Ticket
        {
            get
            {
                if (TicketStyle == null)
                {
                    TicketStyle = new GUIStyle(EditorStyles.helpBox);
                    TicketStyle.padding = new RectOffset(16, 0, 4, 4);
                    TicketStyle.margin = new RectOffset(0, 4, 4, 4);
                    TicketStyle.normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.05f));
                    TicketStyle.hover.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.2f));
                    TicketStyle.active.background = MakeTex(2, 2, Color.blue);
                }

                return TicketStyle;
            }
        }


        public static GUIStyle BigTicket
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.helpBox);
                _style.padding = new RectOffset(16, 0, 4, 12);
                _style.margin = new RectOffset(0, 4, 4, 4);
                _style.normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.05f));
                _style.hover.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.2f));
                return _style;
            }
        }

        public static GUIStyle TicketText
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.fontSize = 12;
                _style.alignment = TextAnchor.LowerLeft;
                return _style;
            }
        }

        public static GUIStyle BigTicketText
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.fontSize = 12;
                _style.fixedHeight = 32;
                _style.alignment = TextAnchor.LowerLeft;
                _style.padding = new RectOffset(0, 0, 0, 6);
                return _style;
            }
        }

        public static GUIStyle TicketGreyText
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.fontSize = 12;
                _style.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                _style.clipping = TextClipping.Clip;
                _style.alignment = TextAnchor.LowerLeft;
                return _style;
            }
        }

        public static GUIStyle BigTicketGreyText
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.fontSize = 12;
                _style.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                _style.clipping = TextClipping.Clip;
                _style.fixedHeight = 32;
                _style.alignment = TextAnchor.LowerLeft;
                _style.padding = new RectOffset(0, 0, 0, 6);
                return _style;
            }
        }

        public static GUIStyle TicketButton
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.miniButton);
                _style.fixedWidth = 64;
                _style.fixedHeight = 24;
                _style.margin = new RectOffset(5, 5, 0, 0);
                return _style;
            }
        }

        public static GUIStyle TicketButtonGrey
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.miniButton);
                _style.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                _style.fixedWidth = 64;
                _style.fixedHeight = 24;
                _style.normal.background = MakeTex(2, 2, new Color(0.6f, 0.6f, 0.6f, 0.1f));
                _style.hover = _style.normal;
                _style.active = _style.normal;
                _style.margin = new RectOffset(5, 5, 0, 0);
                return _style;
            }
        }

        public static GUIStyle SmallTicketButton
        {
            get
            {
                var _style = new GUIStyle();
                _style.fixedWidth = 24;
                _style.fixedHeight = 24;
                _style.padding = new RectOffset(0, 0, 4, 0);
                _style.margin = new RectOffset(20, 0, 0, 0);
                return _style;
            }
        }

        public static GUIStyle CommentGrey
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
#if UNITY_2023_1_OR_NEWER
                _style.clipping = TextClipping.Ellipsis;
#else
            _style.clipping = TextClipping.Clip;
#endif
                _style.fixedHeight = 32;
                _style.margin = new RectOffset(5, 5, 12, 12);
                return _style;
            }
        }
#pragma warning disable UDR0001 // Domain Reload Analyzer
        public static GUIStyle TagStyle;
#pragma warning restore UDR0001 // Domain Reload Analyzer
        public static GUIStyle Tag
        {
            get
            {
                if (TagStyle == null)
                {
                    TagStyle = new GUIStyle(EditorStyles.helpBox);
                    TagStyle.fixedHeight = 16;
                    TagStyle.fixedWidth = 68;
                    TagStyle.padding = new RectOffset(0, 0, 0, 2);
                    SetBackgroundColor(TagStyle, Color.red);
                }
                return TagStyle;
            }
        }

        public static GUIStyle TagText
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.label);
                _style.normal.textColor = Color.black;
                _style.hover.textColor = Color.black;
                _style.fontSize = 10;
                _style.alignment = TextAnchor.MiddleCenter;
                return _style;
            }
        }

        public static GUIStyle ScrollBackground
        {
            get
            {
                var _style = new GUIStyle(EditorStyles.helpBox);
                _style.margin = new RectOffset(5, 0, 0, 0);
                SetBackgroundColor(_style, new Color(0, 0, 0, 0.2f));
                return _style;
            }
        }

        public static void SetBackgroundColor(GUIStyle _style, Color _color)
        {
            if (_style != null)
            {
                _style.normal.background = MakeTex(2, 2, _color);
            }
        }
        public static void SetBackgroundColor(GUIStyle _style, Color _color, Color _hoverColor)
        {
            if (_style != null)
            {
                _style.normal.background = MakeTex(2, 2, _color);
                _style.hover.background = MakeTex(2, 2, _hoverColor);
            }
        }

        private static Texture2D MakeTex(int _width, int _height, Color _color)
        {
            Color[] _pix = new Color[_width * _height];
            for (int i = 0; i < _pix.Length; i++)
                _pix[i] = _color;

            Texture2D result = new Texture2D(_width, _height);
            result.SetPixels(_pix);
            result.Apply();
            return result;
        }
    }
}