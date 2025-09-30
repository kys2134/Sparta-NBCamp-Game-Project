#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using Backend.Object.Character.Player;

namespace Backend.Util.Editor
{
    [CustomEditor(typeof(PlayerMovementController))]
    public class MovementControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = (PlayerMovementController)target;

            if (component.mode != CastMode.MultipleRay)
            {
                return;
            }

            component.rows = EditorGUILayout.IntField("Rows", component.rows);
            component.count = EditorGUILayout.IntField("Count", component.count);
            component.isOffset = EditorGUILayout.Toggle("Is Offset", component.isOffset);

            DrawRaycastArrayPreview(component);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(component);
            }
        }

        private void DrawRaycastArrayPreview(PlayerMovementController component)
        {
            const float size = 3f;

            GUILayout.Space(5);

            var option = GUILayout.Height(100);
            var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, option);

            var x = space.x + ((space.width - space.height) / 2f);
            var y = space.y;
            var width = space.height;
            var height = space.height;
            Rect background = new (x, y, width, height);
            EditorGUI.DrawRect(background, GUI.skin.settings.selectionColor);

            var positions = component.multipleRayPositions;
            var center = new Vector2(background.x + (background.width / 2f), background.y + (background.height / 2f));

            if (positions != null && positions.Length != 0)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    var offset = new Vector2(positions[i].x, positions[i].z) * background.width / 2f * 0.9f;
                    var position = center + offset;

                    x = position.x - (size / 2f);
                    y = position.y - (size / 2f);
                    EditorGUI.DrawRect(new Rect(x, y, size, size), Color.magenta);
                }
            }

            if (positions != null && positions.Length != 0)
            {
                GUILayout.Label("Number of rays: " + positions.Length, EditorStyles.centeredGreyMiniLabel);
            }
        }
    }
}

#endif
