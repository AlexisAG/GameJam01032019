using AgToolkit.Core.DesignPattern.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterManager : Singleton<RegisterManager>
{
    [SerializeField]
    private StringGameObjectDictionary _prefabToInstantiate= new StringGameObjectDictionary();

    private StringGameObjectDictionary _instantiations = new StringGameObjectDictionary();

    private IEnumerator Start() 
    {
        // instantiate prefab
        foreach (string key in _prefabToInstantiate.Keys) 
        {
            _instantiations.Add(key, GameObject.Instantiate(_prefabToInstantiate[key], transform));
            yield return null;
        }
    }

    protected override void OnDestroy() 
    {
        // Destroy GameObject instanciate
        foreach (string key in _prefabToInstantiate.Keys) 
        {
            UnRegister(key);
        }

        base.OnDestroy();
    }

    public GameObject GetGameObjectInstance(string key)
    {
        GameObject go = _instantiations.ContainsKey(key) ? _instantiations[key] : null;
        
        return go;
    }

    public void Register(string key, GameObject prefab)
    {
        if (prefab == null) return;

        _instantiations.Add(key, GameObject.Instantiate(prefab, transform));
    }

    public void UnRegister(string key) 
    {
        if (!_instantiations.ContainsKey(key)) return;

        GameObject.Destroy(_instantiations[key]);
    }
}
