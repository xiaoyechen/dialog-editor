using UnityEngine;

public class MoveToFront : MonoBehaviour {

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}
