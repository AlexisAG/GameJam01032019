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

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringGameObjectDictionary))]
public class StringGameObjectDictionaryDrawer : SerializableDictionaryPropertyDrawer
{

}
#endif
