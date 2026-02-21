using UnityEngine;
using Antymology.Terrain;
using System.Collections; 
using TMPro; 

public class QueenAnt : MonoBehaviour
{
    public TextMeshProUGUI valueText; // Reference to the UI text object
    public float health;
    public int NestBlockCount;
    public bool queenDead = false;
    public float stepDelay = 20f; // second between steps
    private float stepTimer = 0f;
    private bool airB = true;
    // private bool worldReady = false;
    private Rigidbody rb;
    public LayerMask groundMask; //  Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 900;
        stepTimer = stepDelay; 
        // StartCoroutine(WaitForWorldManager());
        rb = GetComponent<Rigidbody>();
        valueText = FindObjectOfType<TextMeshProUGUI>();

    }

    // IEnumerator WaitForWorldManager()
    // {
    //     // Wait until the WorldManager singleton exists
    //     while (WorldManager.Instance == null)
    //     {
    //         yield return null; // wait for next frame
    //     }

    //     worldReady = true;
    // }
    // Update is called once per frame
    void FixedUpdate()
    {
        // if (!worldReady) return; // don’t run ant logic until world is ready
       
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f){

            // is ant dead?
            if (health <= 0)
            {
                // kill the ants first
                queenDead = true;
                Die();
            }

            if (health >= 900)
            {
                health = 900; //setting a limit on the queen health
            }
            Move(); 

            // build nest
            // if (airB){
            BuildNest();     
            // }
            // since it's the nest, i want it spreading and not diffusing
            // SpreadPheromone();
            // ok, now move forward
            // Move();


            // decrease health
            // health = health - 1;

            stepTimer = stepDelay;
        }
    }

    void Move()
    {
        int antLayer = LayerMask.NameToLayer("Ant"); // get the layer number
        int ignoreAntMask = ~(1 << antLayer);        // invert the bit to hit everything except ants

        Vector3 direction = RandomDirection();
        Vector3 forwardStep = transform.position + direction;
        
        int x = Mathf.FloorToInt(forwardStep.x);
        int y = Mathf.FloorToInt(forwardStep.y);
        int z = Mathf.FloorToInt(forwardStep.z);
        // Start ray above the next position
        // Vector3 rayStart = forwardStep + Vector3.up * 10f; // by making this value 2.55, we're saying that the max the ant can look up to move is 2 blocks (as specified by the assignment). We use 2.55 because the ant is slightly floating above the block

        // if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f, ignoreAntMask)) // by making this value 50, we're saying the ant can basically jump down to any level of block
        // {
        //     float groundY = hit.point.y;
        //     // place ant on next block
        //     Vector3 newPosition = new Vector3(
        //         x,
        //         groundY,
        //         z
        //     );
        //     rb.MovePosition(newPosition);

        // }
        // Keep current Y and only move horizontally
        // airB = false;
        // while(!airB)
        // {
        AbstractBlock bl = WorldManager.Instance.GetBlock(x, y, z);

        if (bl is AirBlock a)
        // if (WorldManager.Instance.GetBlock((int)x,(int)y,(int)z) is AirBlock air)
        {
            // airB = true;
            Vector3 newPos = new Vector3(x, rb.position.y, z);
            rb.MovePosition(newPos);
        }
        // else{
            // airB = false;
        // }
            //dont move there! try else where
            // direction = RandomDirection();
            // forwardStep = transform.position + direction;
            // x = Mathf.FloorToInt(forwardStep.x);
            // y = Mathf.FloorToInt(forwardStep.y);
            // z = Mathf.FloorToInt(forwardStep.z);
        // }
        // else{
            // airB = false;
            // while(!airB)
            // {
                // if (WorldManager.Instance.GetBlock((int)x,(int)rb.position.y,(int)z) is AirBlock a)
                // {
                    // airB = true;
                // }
                //dont move there! try else where
                // direction = RandomDirection();
                // forwardStep = transform.position + direction;
                // x = Mathf.FloorToInt(forwardStep.x);
                // z = Mathf.FloorToInt(forwardStep.z);
            // }
            // Vector3 newPos = new Vector3(x, rb.position.y, z);
// 
            // rb.MovePosition(newPos);
        // }
    }
    // void MoveDown()
    // {
    //     // int x = Mathf.FloorToInt(transform.position.x);
    //     // int z = Mathf.FloorToInt(transform.position.z);
    //     // Start ray above the next position
    //     // Vector3 rayStart = transform.position + Vector3.up * 10f; // by making this value 2.55, we're saying that the max the ant can look up to move is 2 blocks (as specified by the assignment). We use 2.55 because the ant is slightly floating above the block

    //     // if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f)) // by making this value 50, we're saying the ant can basically jump down to any level of block
    //     // {
    //         // float groundY = hit.point.y;
    //         // place ant on next block
    //         Vector3 newPosition = new Vector3(
    //             transform.position.x,
    //             transform.position.y + 1,
    //             transform.position.z
    //         );
    //         rb.MovePosition(newPosition);
    //     // }
    // }
    void BuildNest()
    {

        // is ant dead?
        // if (health <= 0)
        // {
        //     // kill all the ants
        //     Die();
        // }

        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);
        AbstractBlock b = WorldManager.Instance.GetBlock(x, y+1, z);

        if (!(b is NestBlock n)){
            // minus health
            health = health - 300;
            WorldManager.Instance.SetBlock(x, y+1, z, new NestBlock());

            NestBlockCount++;
            valueText.text = "Number of Nest Blocks: " + NestBlockCount.ToString(); 

            AbstractBlock block = WorldManager.Instance.GetBlock(x, y+2, z);

            if (block is AirBlock air)
            {
                air.closessToNest = 10f;
                SpreadPheromone(air, x, y+2, z);
            }
            Move();
        }
        // MoveDown();
        // move queen ontop of block
        // transform.position = new Vector3(
        //     x,
        //     y+1,
        //     z
        // );
    }   

    void SpreadPheromone(AirBlock air, int x, int y, int z)
    {
        float spreadAmount = air.closessToNest * 0.05f;

        SpreadToNeighbor(x+1,y,z, spreadAmount);
        SpreadToNeighbor(x-1,y,z, spreadAmount);
        SpreadToNeighbor(x,y,z+1, spreadAmount);
        SpreadToNeighbor(x,y,z-1, spreadAmount);
        SpreadToNeighbor(x+1,y-1,z, spreadAmount);
        SpreadToNeighbor(x-1,y-1,z, spreadAmount);
        SpreadToNeighbor(x,y-1,z+1, spreadAmount);
        SpreadToNeighbor(x,y-1,z-1, spreadAmount);
    }
    void SpreadToNeighbor(int x, int y, int z, float amount)
    {
        AbstractBlock block = WorldManager.Instance.GetBlock(x, y, z);

        if (block is AirBlock neighbor)
        {
            neighbor.closessToNest += amount;
        }
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

