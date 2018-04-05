using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TerrainData))]
public class TerrainDataEditor :Editor {
  private ReorderableList list;
  private int numberOfLines     = 7;
  private float elementSpacing  = 10;
  private float propertySpacing = 5;

  private float elementHeight;

  private void OnEnable() {

    elementHeight = numberOfLines * EditorGUIUtility.singleLineHeight + (numberOfLines - 1) * propertySpacing;


    list = new ReorderableList(serializedObject,
              serializedObject.FindProperty("layers"),
              true, true, true, true) {

      drawHeaderCallback = (Rect rect) => {
        EditorGUI.LabelField(rect, "Terrain Layers");
      }
    };

    list.elementHeight = elementHeight + 2 * elementSpacing;

    list.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) => {
      var element = list.serializedProperty.GetArrayElementAtIndex(index);
      rect.y += elementSpacing;

      float lineHeight = EditorGUIUtility.singleLineHeight;

      int i = 1;

      EditorGUI.PropertyField(
           new Rect(rect.x, rect.y, rect.width , lineHeight),
          element.FindPropertyRelative("layerName"));

      EditorGUI.PropertyField(
          new Rect(rect.x, rect.y + i++*(lineHeight + propertySpacing), rect.width, lineHeight),
          element.FindPropertyRelative("albedo"));

      EditorGUI.PropertyField(
          new Rect(rect.x, rect.y + i++*(lineHeight + propertySpacing), rect.width, lineHeight),
          element.FindPropertyRelative("normal"));

      EditorGUI.PropertyField(
         new Rect(rect.x, rect.y + i*(lineHeight + propertySpacing), rect.width/2 - 50, lineHeight),
         element.FindPropertyRelative("tintColor"), GUIContent.none);

      EditorGUI.PropertyField(
          new Rect(rect.x + rect.width/2 - 40, rect.y + i++*(lineHeight + propertySpacing), rect.width/2 + 40, lineHeight),
          element.FindPropertyRelative("tintBlend"), GUIContent.none);

      EditorGUI.PropertyField(
      new Rect(rect.x, rect.y + i++*(lineHeight + propertySpacing), rect.width, lineHeight),
      element.FindPropertyRelative("textureScale"));

      EditorGUI.PropertyField(
          new Rect(rect.x, rect.y + i++*(lineHeight + propertySpacing), rect.width, lineHeight),
          element.FindPropertyRelative("startHeight"));

      EditorGUI.PropertyField(
          new Rect(rect.x, rect.y + i++*(lineHeight + propertySpacing), rect.width, lineHeight),
          element.FindPropertyRelative("blendHeight"));

      
    };
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();
    DrawDefaultInspector();
    //GUILayout.Space(30);
    //GUILayout.Label("Terrain Texturing");
    GUILayout.Space(10);
    list.DoLayoutList();
    serializedObject.ApplyModifiedProperties();
  }
}
