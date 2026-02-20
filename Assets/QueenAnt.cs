using UnityEngine;
using Antymology.Terrain;
using System.Collections; 

public class QueenAnt : MonoBehaviour
{
    public float health;
    public float stepDelay = 1.0f; // second between steps
    private float stepTimer = 0f;
    private bool worldReady = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 900;
        StartCoroutine(WaitForWorldManager());

    }

    IEnumerator WaitForWorldManager()
    {
        // Wait until the WorldManager singleton exists
        while (WorldManager.Instance == null)
        {
            yield return null; // wait for next frame
        }

        worldReady = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!worldReady) return; // don’t run ant logic until world is ready
       
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f){

            // is ant dead?
            if (health <= 0)
            {
                Die();
            }

            Move();
            // build nest
            BuildNest();     

            // ok, now move forward
            // Move();


            // decrease health
            health = health - 1;

            stepTimer = stepDelay;
        }
    }

    void Move()
    {
        Vector3 direction = RandomDirection();
        Vector3 forwardStep = transform.position + direction;

        // Start ray above the next position
        Vector3 rayStart = forwardStep + Vector3.up * 2.55f; // by making this value 2.55, we're saying that the max the ant can look up to move is 2 blocks (as specified by the assignment). We use 2.55 because the ant is slightly floating above the block

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f)) // by making this value 50, we're saying the ant can basically jump down to any level of block
        {
            float groundY = hit.point.y;
            // place ant on next block
            transform.position = new Vector3(
                Mathf.Round(forwardStep.x),
                groundY,
                Mathf.Round(forwardStep.z)
            );
        }
    }

    void BuildNest()
    {
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);
        WorldManager.Instance.SetBlock(x, y, z, new NestBlock());

        // minus health
        health = health - 300;

        // move queen ontop of block
        transform.position = new Vector3(
            x,
            y+1,
            z
        );
    }   
    void Die()
    {
        Destroy(gameObject);
    }
    Vector3 RandomDirection()
    {
        int direction = UnityEngine.Random.Range(0, 4); 

        switch (direction)
        {
            case 0: return Vector3.forward;
            case 1: return Vector3.back;
            case 2: return Vector3.right;
            default: return Vector3.left;
        }
    }
}

