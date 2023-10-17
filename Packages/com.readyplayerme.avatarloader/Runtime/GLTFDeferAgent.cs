using GLTFast;
using UnityEngine;

public class GLTFDeferAgent : MonoBehaviour
{
    public IDeferAgent GetGLTFastDeferAgent()
    {
        return GetComponent<IDeferAgent>();
    }
}
