#if UNITY_2021_3_OR_NEWER

using System;
using System.Collections.Generic;
using System.Reflection;
using SaintsField.Editor.Core;
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace SaintsField.Editor.Drawers.ButtonDrawers.DecButtonDrawer
{
    public partial class DecButtonAttributeDrawer
    {


        // private static string ClassButton(SerializedProperty property) => $"{property.propertyPath}__Button";
        private static string ClassLabelContainer(SerializedProperty property, int index) => $"{property.propertyPath}__{index}__LabelContainer";
        private static string ClassLabelError(SerializedProperty property, int index) => $"{property.propertyPath}__{index}__LabelError";
        private static string ClassExecError(SerializedProperty property, int index) => $"{property.propertyPath}__{index}__ExecError";

        protected static VisualElement DrawUIToolkit(SerializedProperty property, ISaintsAttribute saintsAttribute,
            int index, FieldInfo info, object parent, VisualElement container)
        {
            Button buttonElement = null;
            IVisualElementScheduledItem buttonTask = null;
            buttonElement = new Button(() =>
            {
                (string buttonError, object buttonResult) = CallButtonFunc(property, (DecButtonAttribute) saintsAttribute, info, parent);
                HelpBox helpBox = container.Query<HelpBox>(className: ClassExecError(property, index)).First();
                helpBox.style.display = buttonError == ""? DisplayStyle.None: DisplayStyle.Flex;
                helpBox.text = buttonError;

                buttonTask?.Pause();
                if (buttonResult is System.Collections.IEnumerator enumerator)
                {
                    buttonElement.userData = enumerator;
                    buttonTask?.Pause();
                    buttonTask = buttonElement.schedule.Execute(() =>
                    {
                        if (buttonElement.userData is System.Collections.IEnumerator bindEnumerator)
                        {
                            if (!bindEnumerator.MoveNext())
                            {
                                buttonTask?.Pause();
                            }
                        }
                    }).Every(1);
                }
            })
            {
                style =
                {
                    height = EditorGUIUtility.singleLineHeight,
                    flexGrow = 1,
                },
            };

            VisualElement labelContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    // flexGrow = 1,
                    justifyContent = Justify.Center,  // horizontal
                    alignItems = Align.Center,  // vertical
                },
                userData = "",
            };
            labelContainer.AddToClassList(ClassLabelContainer(property, index));
            // labelContainer.Add(new Label("test label"));

            buttonElement.Add(labelContainer);
            // button.AddToClassList();
            buttonElement.AddToClassList(ClassAllowDisable);
            return buttonElement;
        }

        protected static HelpBox DrawLabelError(SerializedProperty property, int index) => DrawError(ClassLabelError(property, index));

        protected static HelpBox DrawExecError(SerializedProperty property, int index) => DrawError(ClassExecError(property, index));

        private static HelpBox DrawError(string className)
        {
            HelpBox helpBox = new HelpBox("", HelpBoxMessageType.Error)
            {
                style =
                {
                    display = DisplayStyle.None,
                },
            };
            helpBox.AddToClassList(className);
            helpBox.AddToClassList(ClassAllowDisable);
            return helpBox;
        }

        protected override void OnUpdateUIToolkit(SerializedProperty property, ISaintsAttribute saintsAttribute,
            int index,
            VisualElement container, Action<object> onValueChangedCallback, FieldInfo info)
        {
            // if (parent == null)
            // {
            //     return;
            // }

            VisualElement labelContainer = container.Query<VisualElement>(className: ClassLabelContainer(property, index)).First();
            string oldXml = (string)labelContainer.userData;
            DecButtonAttribute decButtonAttribute = (DecButtonAttribute) saintsAttribute;

            object parent = SerializedUtils.GetFieldInfoAndDirectParent(property).parent;
            (string xmlError, string newXml) = RichTextDrawer.GetLabelXml(property, decButtonAttribute.ButtonLabel, decButtonAttribute.IsCallback, info, parent);

            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (newXml == null)
            {
                newXml = ObjectNames.NicifyVariableName(decButtonAttribute.FuncName);
            }

            HelpBox helpBox = container.Query<HelpBox>(className: ClassLabelError(property, index)).First();
            helpBox.style.display = xmlError == ""? DisplayStyle.None: DisplayStyle.Flex;
            helpBox.text = xmlError;

            if (oldXml == newXml)
            {
                return;
            }

            // Debug.Log($"update xml={newXml}");

            labelContainer.userData = newXml;
            labelContainer.Clear();
            IEnumerable<RichTextDrawer.RichTextChunk> richChunks = RichTextDrawer.ParseRichXml(newXml, property.displayName, info, parent);
            foreach (VisualElement visualElement in RichTextDrawer.DrawChunksUIToolKit(richChunks))
            {
                labelContainer.Add(visualElement);
            }
        }
    }
}
#endif
