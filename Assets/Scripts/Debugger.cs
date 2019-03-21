using UnityEngine;

public class Debugger : MonoBehaviour
{
    private Camera camera = null;

    [SerializeField]
    private Transform target = null;
    [SerializeField]
    private Unit seeker = null;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                target.position = hit.point;
                seeker.FindPath();
            }
        }
    }
}
