using UnityEngine;
using UnityEditor;

public class HandlesMono : MonoBehaviour
{

    public SimpleScriptableObject scriptableObject;

    private void Start()
    {
        if(scriptableObject != null)
        {
            if(scriptableObject.pairs != null && scriptableObject.pairs.Count > 0)//list非空且有数据
            {
                var name = scriptableObject.pairs[0].Name;
                Debug.Log($"Name of first pair: {name}");
            }
        }
    }

}
