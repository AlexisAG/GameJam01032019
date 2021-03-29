using System;
using AgToolkit.Core.Helper.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AgToolkit.Core.Helper.Drawer;
#endif

[Serializable]
public class StringGameObjectDictionary : SerializableDictionary<string, GameObject>
{
}

[Serializable]
public class Vector3Vector3Dictionary : SerializableDictionary<Vector3, Vector3>
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
public class StringGameObjectDictionaryDrawer : SerializableDictionaryPropertyDrawer
{

}
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Vector3Vector3Dictionary))]
public class Vector3Vector3DictionaryDrawer : SerializableDictionaryPropertyDrawer
{

}
#endif
