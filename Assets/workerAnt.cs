using UnityEngine;

public class workerAnt : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool moving = false;
    private bool adjustingHeight = false;
    private float targetHeight;
    float blockTopY;
    bool flag;
    public float stepDelay = 1.0f; // half second between steps
    private float stepTimer = 0f;
    // Update is called once per frame
    void Update()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f){

            Vector3 forwardStep = transform.position + transform.forward;

            // Start ray above the next position
            Vector3 rayStart = forwardStep + Vector3.up * 10f;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f))
            {
                float groundY = hit.point.y;

                // Instantly place ant on next block
                transform.position = new Vector3(
                    Mathf.Round(forwardStep.x),
                    groundY,
                    Mathf.Round(forwardStep.z)
                );
            }
            stepTimer = stepDelay;

        }

        
    }

}
