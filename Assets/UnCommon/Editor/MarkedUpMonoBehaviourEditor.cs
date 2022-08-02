using UnityEngine;
using UnityEditor;
using MarkupAttributes.Editor;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
internal class MarkedUpMonoBehaviourEditor : MarkedUpEditor
{
}

[CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
internal class MarkedUpScriptableObjectEditor : MarkedUpEditor
{
}
