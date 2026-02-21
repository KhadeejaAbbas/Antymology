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
    private Rigidbody rb;
    public LayerMask groundMask; //  Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 900;
        stepTimer = stepDelay; 
        rb = GetComponent<Rigidbody>();
        valueText = FindObjectOfType<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {       
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
            // ok, now move forward
            Move(); 

            // build nest
            BuildNest();     
            
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
        
        // AbstractBlock bl = WorldManager.Instance.GetBlock(x, y, z);
        // if (bl is AirBlock a)
        // {
        Vector3 newPos = new Vector3(x, y + 5, z);
        rb.MovePosition(newPos);
        // }
    }

    void BuildNest()
    {

        // get curr pos
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);
        AbstractBlock b = WorldManager.Instance.GetBlock(x, y+1, z);

        if (!(b is NestBlock n)){ // dont move into own nest block plz
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

    }   

    // since it's the nest, i want it spreading and not diffusing
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

