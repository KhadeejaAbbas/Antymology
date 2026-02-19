using UnityEngine;
using Antymology.Terrain;

public class workerAnt : MonoBehaviour
{
    public float health;
    public float stepDelay = 1.0f; // second between steps
    private float stepTimer = 0f;
    public float stepDelay2 = 1.0f; // second between steps
    private float stepTimer2 = 0f;
    private bool diggable = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveDown();
        health = 100;
    }

    void Update()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f){
            // is ant dead?
            if (health == 0)
            {
                Die();
            }
            // is the block we're on eatable?
            // BlockActions();     

            // are we properly on a block?
            MoveDown();

            // ok, now move forward
            // Move();
       
            health = health - 5;

            stepTimer = stepDelay;
        }
    }

    void Move()
    {
        Vector3 forwardStep = transform.position + transform.forward;

        // Start ray above the next position
        Vector3 rayStart = forwardStep + Vector3.up * 3f; // by making this value 3, we're saying that the max the ant can look up to move is 2 blocks (as specified by the assignment)

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

    void BlockActions()
    {
        // what block are we on?
        int block = CheckBlockMaterial((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

            // list:
    // 0: stone
    // 1: mulch
    // 2: grass
    // 3: acid
    // 4: air
    // 5: container
    // 6: nest

        if (block == 0)
        {
            // it is stone
            Stone();
        }

        if (block == 1)
        {
            Mulch();
            //move down
            // it is mulch
            // Mulch();
        }
        if (block == 2)
        {
            // it is grass
            Grass();
        }

        if (block == 3)
        {
            // it is acid
            Acid();
        }
        if (block == 4)
        {
            // it is air
            Air();
        }
        if (block == 5)
        {
            // it is container
            Container();
        }

        if (block == 6)
        {
            // it is nest
            Nest();
        }
    }

    void Stone()
    {
        //bro idk just ignore it?
    }

    void Mulch()
    {
        // can eat it if no one else is on it
        if (!IsAnotherAntOnSameBlock()){
            // eat the block!
            RemoveBlockUnderAnt();
            // increase health
            health+= 10;
            // move down
            MoveDown(); 
        }

    }

    void Grass()
    {
        //bro idk just ignore it?
    }

    void Acid()
    {
        //bro idk just ignore it?
    }

    void Air()
    {
        //bro idk just ignore it?
    }
    void Container()
    {
        // Ants cannot dig up a block of type ContainerBlock
        diggable = false;
    }

    void Nest()
    {
        //bro idk just ignore it?
    }
    void Die()
    {
        Destroy(gameObject);
    }

    int CheckBlockMaterial(int x, int y, int z)
    {
        // Get the block from the world
        AbstractBlock block = WorldManager.Instance.GetBlock(x, y - 1, z);

        // Get its texture ID
        int textureID = block.TextureID;

        return textureID;
    }

    bool IsAnotherAntOnSameBlock()
    {
        Vector3Int myBlock = GetBlockPosition(transform.position);

        foreach (GameObject ant in WorldManager.Instance._instances)
        {
            if (ant == this.gameObject) continue;

            Vector3Int otherBlock = GetBlockPosition(ant.transform.position);

            if (myBlock == otherBlock)
            {
                return true;
            }
        }

        return false;
    }

    Vector3Int GetBlockPosition(Vector3 worldPos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y), // block under feet
            Mathf.FloorToInt(worldPos.z)
        );
    }
    void RemoveBlockUnderAnt()
    {
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);

        WorldManager.Instance.SetBlock(x, y, z, new AirBlock());
    }

    void MoveDown()
    {
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);
        // Start ray above the position
        Vector3 rayDown = transform.position + Vector3.up * 10f; // by making this value 3, we're saying that the max the ant can look up to move is 2 blocks (as specified by the assignment)

        if (Physics.Raycast(rayDown, Vector3.down, out RaycastHit hitDown, 50f)) // by making this value 50, we're saying the ant can basically jump down to any level of block
        {
            // place ant on next block
            transform.position = new Vector3(
                transform.position.x,
                hitDown.point.y,
                transform.position.z
            );
        }
    }

    // void MoveDownVoxel()
// {


//     // move down until we hit non-air
//     while (WorldManager.Instance.GetBlock(x, y - 1, z) is AirBlock)
//     {
//         y--;
//     }

//     transform.position = new Vector3(x + 0.5f, y, z + 0.5f); // optional +0.5 for center of block
// }
}
