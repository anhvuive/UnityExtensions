#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimatorAnimAttribute))]
public class AnimatorPropertyDrawer : PropertyDrawer
{
    private RuntimeAnimatorController _animator;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_animator == null)
        {
            var animAttribute = attribute as AnimatorAnimAttribute;
            if (string.IsNullOrEmpty(animAttribute.animatorPath))
            {
                if (property.serializedObject.targetObject.GetComponent<Animator>() != null)
                {
                    if (property.serializedObject.targetObject.GetComponent<Animator>() != null)
                    {
                        _animator = property.serializedObject.targetObject.GetComponent<Animator>()
                            .runtimeAnimatorController;
                    }
                }
                else
                {
                    if (property.serializedObject.targetObject.GetComponentInChildren<Animator>() != null)
                    {
                        _animator = property.serializedObject.targetObject.GetComponentInChildren<Animator>()
                            .runtimeAnimatorController;
                    }
                }
            }
            else
            {
                _animator = GetAssetAtPath<RuntimeAnimatorController>(animAttribute.animatorPath);
            }
        }

        if (_animator == null)
        {
            GUI.Box(position, "Not found animator");
        }
        else
        {
            EditorGUI.BeginProperty(position, label, property);
            float labelWidth = 120;
            if (position.width * .425f > labelWidth)
            {
                labelWidth = position.width * .425f;
            }

            var rect1 = new Rect(position.x, position.y, labelWidth, position.height);
            var rect2 = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            GUI.Label(rect1, label);
            if (GUI.Button(rect2, property.stringValue, EditorStyles.popup))
                Selector(property);
            EditorGUI.EndProperty();
        }
    }

    public static T GetAssetAtPath<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<T>(path);
#endif
        return default;
    }

    static GUIContent tempContent;

    static GUIContent TempContent(string text, Texture2D image = null, string tooltip = null)
    {
        if (tempContent == null) tempContent = new GUIContent();
        tempContent.text = text;
        tempContent.image = image;
        tempContent.tooltip = tooltip;
        return tempContent;
    }


    void Selector(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        var clips = _animator.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            string name = clips[i].name;
            menu.AddItem(new GUIContent(name), !property.hasMultipleDifferentValues && name == property.stringValue,
                HandleSelect, new SpineDrawerValuePair(name, property));
        }

        menu.ShowAsContext();
    }

    static void HandleSelect(object val)
    {
        var pair = (SpineDrawerValuePair)val;
        pair.property.stringValue = pair.stringValue;
        pair.property.serializedObject.ApplyModifiedProperties();
    }
}

public struct SpineDrawerValuePair
{
    public string stringValue;
    public SerializedProperty property;

    public SpineDrawerValuePair(string val, SerializedProperty property)
    {
        stringValue = val;
        this.property = property;
    }
}
#endif