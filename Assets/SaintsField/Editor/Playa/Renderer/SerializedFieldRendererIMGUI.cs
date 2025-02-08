using System;
using System.Linq;
using SaintsField.Editor.Core;
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SaintsField.Editor.Playa.Renderer
{
    public partial class SerializedFieldRenderer
    {
        private RichTextDrawer _richTextDrawer;

        ~SerializedFieldRenderer()
        {
            _richTextDrawer = null;
        }

        public override void OnDestroy()
        {
            _richTextDrawer?.Dispose();
            _richTextDrawer = null;
        }

        private class UnsetGuiStyleFixedHeight : IDisposable
        {
            private readonly GUIStyle _guiStyle;
            private readonly float _oldValue;

            public UnsetGuiStyleFixedHeight(GUIStyle guiStyle)
            {
                _guiStyle = guiStyle;
                _oldValue = guiStyle.fixedHeight;
                _guiStyle.fixedHeight = 0;
            }

            public void Dispose()
            {
                _guiStyle.fixedHeight = _oldValue;
            }
        }

        private class ImGuiListInfo
        {
            public SerializedProperty Property;
            public PreCheckResult PreCheckResult;

            public bool HasSearch;
            public bool HasPaging;
            public PagingInfo PagingInfo;
            public string SearchText = string.Empty;
            public int PageIndex;
            public int NumberOfItemsPrePage;
        }

        private ReorderableList _imGuiReorderableList;
        private ImGuiListInfo _imGuiListInfo;

        private void DrawListDrawerSettingsField(SerializedProperty property, Rect position, ArraySizeAttribute arraySizeAttribute, bool delayed)
        {
            Rect usePosition = new Rect(position)
            {
                y = position.y + 1,
                height = position.height - 2,
            };

            if (!property.isExpanded)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle headerBackground = "RL Header";
                    using (new UnsetGuiStyleFixedHeight(headerBackground))
                    {
                        headerBackground.Draw(usePosition, false, false, false, false);
                    }
                }

                Rect paddingTitle = new Rect(usePosition)
                {
                    x = usePosition.x + 6,
                    y = usePosition.y + 1,
                    height = usePosition.height - 2,
                    width = usePosition.width - 12,
                };
                DrawListDrawerHeader(paddingTitle, delayed);
                return;
            }

            PagingInfo newPagingInfo = GetPagingInfo(property, _imGuiListInfo.PageIndex, _imGuiListInfo.SearchText, _imGuiListInfo.NumberOfItemsPrePage);
            if (!newPagingInfo.IndexesCurPage.SequenceEqual(_imGuiListInfo.PagingInfo.IndexesCurPage))
            {
                _imGuiReorderableList = null;
            }

            _imGuiListInfo.PagingInfo = newPagingInfo;

            if (_imGuiReorderableList == null)
            {
                _imGuiReorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true)
                    {
                        headerHeight = SaintsPropertyDrawer.SingleLineHeight * ((_imGuiListInfo.HasPaging || _imGuiListInfo.HasSearch)? 2: 1),
                    };
                _imGuiReorderableList.drawHeaderCallback += v => DrawListDrawerHeader(v, delayed);
                _imGuiReorderableList.elementHeightCallback += DrawListDrawerItemHeight;
                _imGuiReorderableList.drawElementCallback += DrawListDrawerItem;

                if(arraySizeAttribute != null)
                {
                    if(arraySizeAttribute.Min > 0)
                    {
                        // _imGuiReorderableList.displayRemove = true;
                        // _imGuiReorderableList.onRemoveCallback += r =>
                        // {
                        //     Debug.Log(r);
                        // };
                        _imGuiReorderableList.onCanRemoveCallback += r =>
                        {
                            bool canRemove = r.count > arraySizeAttribute.Min;
                            // Debug.Log($"canRemove={canRemove}, count={r.count}, min={arraySizeAttribute.Min}");
                            return canRemove;
                        };
                    }

                    if (arraySizeAttribute.Max > 0)
                    {
                        _imGuiReorderableList.onCanAddCallback += r =>
                        {
                            bool canAdd = r.count < arraySizeAttribute.Max;
                            // Debug.Log($"canAdd={canAdd}, count={r.count}, max={arraySizeAttribute.Max}");
                            return canAdd;
                        };
                    }
                    // _imGuiReorderableList.onCanAddCallback += _ => !(arraySizeAttribute.Min >= 0 && property.arraySize <= arraySizeAttribute.Min);
                }
            }



            // Debug.Log(ReorderableList.defaultBehaviours);
            // Debug.Log(ReorderableList.defaultBehaviours.headerBackground);

            using(new UnsetGuiStyleFixedHeight("RL Header"))
            {
                _imGuiReorderableList.DoList(usePosition);
            }
        }

        private Texture2D _iconDown;
        private Texture2D _iconLeft;
        private Texture2D _iconRight;

        private void DrawListDrawerHeader(Rect rect, bool delayed)
        {
            // const float twoNumberInputWidth = 20;
            const float inputWidth = 30;
            // const float itemsLabelWidth = 75;
            const float itemsLabelWidth = 65;
            const float buttonWidth = 19;
            // const float pagingLabelWidth = 35;
            const float pagingLabelWidth = 30;
            const float pagingSepWidth = 8;

            const float gap = 5;

            (Rect titleRect, Rect controlRect) = RectUtils.SplitHeightRect(rect, EditorGUIUtility.singleLineHeight);
            controlRect.height -= 1;

            (Rect titleFoldRect, Rect titleButtonRect) = RectUtils.SplitWidthRect(titleRect, 16);

            if (!_imGuiListInfo.HasPaging && !_imGuiListInfo.HasSearch)  // draw element count container
            {
                (Rect titleButtonNewRect, Rect titleItemTotalRect) =
                    RectUtils.SplitWidthRect(titleButtonRect, titleButtonRect.width - 50);
                titleItemTotalRect.y += 1;
                titleItemTotalRect.height -= 2;

                using(EditorGUI.ChangeCheckScope changed = new EditorGUI.ChangeCheckScope())
                {
                    int newCount = EditorGUI.DelayedIntField(titleItemTotalRect, GUIContent.none,
                        _imGuiListInfo.PagingInfo.IndexesAfterSearch.Count);
                    if (changed.changed)
                    {
                        _imGuiListInfo.Property.arraySize = newCount;
                        _imGuiListInfo.Property.serializedObject.ApplyModifiedProperties();
                        _imGuiListInfo.PagingInfo = GetPagingInfo(_imGuiListInfo.Property, _imGuiListInfo.PageIndex,
                            _imGuiListInfo.SearchText, _imGuiListInfo.NumberOfItemsPrePage);
                        return;
                    }

                    // EditorGUI.LabelField(new Rect(titleItemTotalRect)
                    // {
                    //     width = titleItemTotalRect.width - 6,
                    // }, "Items", new GUIStyle("label") { alignment = TextAnchor.MiddleRight, normal =
                    // {
                    //     textColor = Color.gray,
                    // }, fontStyle = FontStyle.Italic});
                }

                titleButtonRect = titleButtonNewRect;
            }

            if(GUI.Button(titleButtonRect, "", GUIStyle.none))
            {
                _imGuiListInfo.Property.isExpanded = !_imGuiListInfo.Property.isExpanded;
                return;
            }

            PreCheckResult preCheckResult = _imGuiListInfo.PreCheckResult;
            if (preCheckResult.HasRichLabel)
            {
                if(_richTextDrawer == null)
                {
                    _richTextDrawer = new RichTextDrawer();
                }

                // Debug.Log(preCheckResult.RichLabelXml);
                if (_curXml != preCheckResult.RichLabelXml)
                {
                    _curXmlChunks =
                        RichTextDrawer
                            .ParseRichXml(preCheckResult.RichLabelXml, FieldWithInfo.SerializedProperty.displayName, FieldWithInfo.FieldInfo, FieldWithInfo.Target)
                            .ToArray();
                }

                _curXml = preCheckResult.RichLabelXml;

                _richTextDrawer.DrawChunks(titleButtonRect, new GUIContent(FieldWithInfo.SerializedProperty.displayName), _curXmlChunks);
            }
            else
            {
                EditorGUI.LabelField(titleButtonRect, _imGuiListInfo.Property.displayName);
            }

            if (_imGuiListInfo.Property.isExpanded)
            {
                if (!_iconDown)
                {
                    _iconDown = Util.LoadResource<Texture2D>("classic-dropdown-gray.png");
                }
                GUI.DrawTexture(titleFoldRect, _iconDown);
            }
            else
            {
                if (!_iconRight)
                {
                    _iconRight = Util.LoadResource<Texture2D>("classic-dropdown-right-gray.png");
                }
                GUI.DrawTexture(titleFoldRect, _iconRight);
                return;
            }

            float searchInputWidth = rect.width - inputWidth * 2 - itemsLabelWidth - pagingSepWidth - buttonWidth * 2 - pagingLabelWidth;

            (Rect searchRect, Rect pagingRect) = RectUtils.SplitWidthRect(controlRect, _imGuiListInfo.HasPaging? searchInputWidth: controlRect.width);

            if(_imGuiListInfo.HasSearch)
            {
                if(delayed)
                {
                    _imGuiListInfo.SearchText = EditorGUI.DelayedTextField(new Rect(searchRect)
                    {
                        width = searchRect.width - gap,
                    }, GUIContent.none, _imGuiListInfo.SearchText);
                }
                else
                {
                    _imGuiListInfo.SearchText = EditorGUI.TextField(new Rect(searchRect)
                    {
                        width = searchRect.width - gap,
                    }, GUIContent.none, _imGuiListInfo.SearchText);
                }
                if (string.IsNullOrEmpty(_imGuiListInfo.SearchText))
                {
                    EditorGUI.LabelField(new Rect(searchRect)
                    {
                        width = searchRect.width - 6,
                    }, "Search", new GUIStyle("label") { alignment = TextAnchor.MiddleRight, normal =
                    {
                        textColor = Color.gray,
                    }, fontStyle = FontStyle.Italic});
                }
            }

            if(_imGuiListInfo.HasPaging)
            {
                Rect numberOfItemsPerPageRect = new Rect(pagingRect)
                {
                    width = inputWidth,
                };
                _imGuiListInfo.NumberOfItemsPrePage = EditorGUI.IntField(numberOfItemsPerPageRect, GUIContent.none,
                    _imGuiListInfo.NumberOfItemsPrePage);

                Rect numberOfItemsSepRect = new Rect(numberOfItemsPerPageRect)
                {
                    x = numberOfItemsPerPageRect.x + numberOfItemsPerPageRect.width,
                    width = pagingSepWidth,
                };
                EditorGUI.LabelField(numberOfItemsSepRect, $"/");

                Rect numberOfItemsTotalRect = new Rect(numberOfItemsSepRect)
                {
                    x = numberOfItemsSepRect.x + numberOfItemsSepRect.width,
                    width = itemsLabelWidth,
                };
                using(EditorGUI.ChangeCheckScope changed = new EditorGUI.ChangeCheckScope())
                {
                    int newCount = EditorGUI.DelayedIntField(numberOfItemsTotalRect, GUIContent.none,
                        _imGuiListInfo.PagingInfo.IndexesAfterSearch.Count);
                    if (changed.changed)
                    {
                        _imGuiListInfo.Property.arraySize = newCount;
                        _imGuiListInfo.Property.serializedObject.ApplyModifiedProperties();
                        _imGuiListInfo.PagingInfo = GetPagingInfo(_imGuiListInfo.Property, _imGuiListInfo.PageIndex,
                            _imGuiListInfo.SearchText, _imGuiListInfo.NumberOfItemsPrePage);
                        return;
                    }
                }
                // EditorGUI.LabelField(totalItemRect, $"/ 8888 Items");

                EditorGUI.LabelField(numberOfItemsTotalRect, "Items", new GUIStyle("label") { alignment = TextAnchor.MiddleRight, normal =
                {
                    textColor = Color.gray,
                }, fontStyle = FontStyle.Italic});

                Rect prePageRect = new Rect(numberOfItemsTotalRect)
                {
                    x = numberOfItemsTotalRect.x + numberOfItemsTotalRect.width,
                    width = buttonWidth,
                };
                using (new EditorGUI.DisabledScope(_imGuiListInfo.PagingInfo.CurPageIndex <= 0))
                {
                    if (!_iconLeft)
                    {
                        _iconLeft = Util.LoadResource<Texture2D>("classic-dropdown-left.png");
                    }
                    if (GUI.Button(prePageRect, _iconLeft, EditorStyles.miniButtonLeft))
                    {
                        if (_imGuiListInfo.PagingInfo.CurPageIndex > 0)
                        {
                            _imGuiListInfo.PageIndex -= 1;
                        }
                    }
                }

                Rect pageRect = new Rect(prePageRect)
                {
                    x = prePageRect.x + prePageRect.width,
                    width = inputWidth,
                };
                _imGuiListInfo.PageIndex =
                    EditorGUI.IntField(pageRect, GUIContent.none, _imGuiListInfo.PageIndex + 1) - 1;
                Rect totalPageRect = new Rect(pageRect)
                {
                    x = pageRect.x + pageRect.width,
                    width = pagingLabelWidth,
                };
                EditorGUI.LabelField(totalPageRect, $"/ {_imGuiListInfo.PagingInfo.PageCount}");
                // EditorGUI.LabelField(totalPageRect, $"/ 888");

                Rect nextPageRect = new Rect(totalPageRect)
                {
                    x = totalPageRect.x + totalPageRect.width,
                    width = buttonWidth,
                };
                using (new EditorGUI.DisabledScope(_imGuiListInfo.PagingInfo.CurPageIndex >=
                                                   _imGuiListInfo.PagingInfo.PageCount - 1))
                {
                    if (!_iconRight)
                    {
                        _iconRight = Util.LoadResource<Texture2D>("classic-dropdown-right.png");
                    }
                    if (GUI.Button(nextPageRect, _iconRight, EditorStyles.miniButtonRight))
                    {
                        if (_imGuiListInfo.PagingInfo.CurPageIndex < _imGuiListInfo.PagingInfo.PageCount - 1)
                        {
                            _imGuiListInfo.PageIndex += 1;
                        }
                    }
                }
            }
        }

        private float DrawListDrawerItemHeight(int index)
        {
            if (_imGuiListInfo.PagingInfo.IndexesCurPage.Contains(index))
            {
                if(index >= _imGuiListInfo.Property.arraySize)
                {
                    return 0;
                }
                SerializedProperty element = FieldWithInfo.SerializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true);
            }

            return 0;
        }

        private void DrawListDrawerItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (rect.height <= 0)
            {
                return;
            }

            SerializedProperty property = _imGuiListInfo.Property.GetArrayElementAtIndex(index);

            Rect useRect = property.propertyType == SerializedPropertyType.Generic
                ? new Rect(rect)
                {
                    x = rect.x + 12,
                    width = rect.width - 12,
                }
                : rect;

            EditorGUI.PropertyField(useRect, property, new GUIContent($"Element {index}"), true);
        }
    }
}
