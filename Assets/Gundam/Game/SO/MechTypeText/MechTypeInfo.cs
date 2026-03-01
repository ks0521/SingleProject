using UnityEngine;

namespace SO.Mech
{
    [CreateAssetMenu(menuName = "Contents/MechTypeInfo")]
    public class MechTypeInfo : ScriptableObject
    {
        [TextArea] public string typeName;
        [TextArea] public string typeContext;
        [TextArea] public string weaponsHead;
        [TextArea] public string weapons;
    }
}