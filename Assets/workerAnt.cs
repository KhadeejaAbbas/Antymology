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

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            Vector3 forwardStep = transform.position + transform.forward;

            // okay, this handles the case of our goal position being floating above a block
            Vector3 rayStart = forwardStep + Vector3.up * 10f;
            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 50f))
            {
                blockTopY = hit.point.y;
            }
            // hmm okay this is supposed to be our goal position of where we want to move to. 
            // our goal should be to the edge of the block
            
            // if there is a wall ahead, stop 
            if (Physics.Raycast(transform.position, transform.forward, 1f))
            {
                flag=true;
                // stop
            }
            // if there is a cliff ahead, stop
            Vector3 cliffRayStart = forwardStep + Vector3.up * 1f;
            if (!Physics.Raycast(cliffRayStart, Vector3.down, 2f))
            {
                flag=true;
                // stop
            }
            
            // Safe to move
            targetPosition = new Vector3(
                forwardStep.x,
                blockTopY,
                forwardStep.z
            );
            if (!flag){
                moving = true;
            }
            /*
            // now, I want the goal position to be at the very edge of a block
            // there are two cases to consider:
            //      1. if there is a block (hill ahead)
            //      2. if there is no block (raven ahead)

            // to handle case 1, we will use raycast, easy
            Vector3 rayStartHorizontal = transform.position;
            RaycastHit hitHorizontal;
            if (Physics.Raycast(rayStartHorizontal, transform.forward, out hitHorizontal, 1f))
            {
                float hillBlock = hitHorizontal.point.x;
                targetPosition = new Vector3(
                    hillBlock,
                    blockTopY,
                    forwardStep.z
                );
                // moving = true;
            }

            // to handle case 2, we need to know if the block ahead is going to dip
            Vector3 rayStartCliff = forwardStep + Vector3.up * 1f;
            RaycastHit hitCliff;

            if (Physics.Raycast(rayStartCliff, Vector3.down, out hitCliff, 2f))
            {
                float endX = hitCliff.point.x;
                targetPosition = new Vector3(
                    endX,
                    blockTopY,
                    forwardStep.z
                );
                // moving = true;
            }
            moving = true;
            */

        }

        if (moving)
        {
            // Move only horizontally
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // When XZ reached, snap/drop to correct Y
            // if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            // {
            //     transform.position = new Vector3(
            //         transform.position.x,
            //         targetHeight,
            //         transform.position.z
            //     );

            //     moving = false;
            // }
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                moving = false;
            }
        }
    }

}
