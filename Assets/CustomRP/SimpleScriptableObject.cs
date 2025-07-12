using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MyConfig/SimpleObj", fileName = "SimpleScriptableObj")]
public class SimpleScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<InfoPair> pairs;

}

[System.Serializable]
public class  InfoPair
{
    public int Id;
    public string Name;
    public Texture Tex;
}
