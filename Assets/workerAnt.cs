using UnityEngine;
using Antymology.Terrain;
using System.Collections;

public class WorkerAnt : MonoBehaviour
{
    public float health;
    public float stepDelay = 1.0f; // second between steps
    private float stepTimer = 0f;
    private bool diggable = true;
    private bool worldReady = false;
    private Rigidbody rb;
    private bool tooManyAnts = false;
    private bool hitAHill = false;

    void Start()
    {
        health = 500;
        stepTimer = stepDelay; 
        StartCoroutine(WaitForWorldManager());
        rb = GetComponent<Rigidbody>();

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
    void FixedUpdate()
    {
        if (!worldReady) return; // don’t run ant logic until world is ready

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f){
            QueenAnt queen = WorldManager.Instance.queen;
            if (queen.queenDead)
            {
                Die();
            }
            // is ant dead?
            if (health <= 0)
            {
                Die();
            }

            // ok, now move forward
            Move();

            // is the block we're on eatable?
            BlockActions();     
       
            // is my friend ant with me?
            Donate();

            // decrease health
            health = health - 5;

            stepTimer = stepDelay;
        }
    }

    void Move()
    {
        Vector3 direction;
        QueenAnt queen = WorldManager.Instance.queen;

        if (health > 250 || (queen.health < 600)) // move towards nest if we have enough health to give or if the queen is close to dying
        // if ((queen.health < 600)) // move towards nest if we have enough health to give or if the queen is close to dying
        {
            direction = FindBestPheromoneDirection();
        }
        if (tooManyAnts)
        {
            direction = RandomDirection();
            tooManyAnts = false;
        }
        if (hitAHill){
            direction = RandomDirection();
            hitAHill = false;
        }
        else
        {
            direction = GoTowardsMulchiness();
        }
        Vector3 forwardStep = transform.position + direction;

        int targetX = Mathf.RoundToInt(forwardStep.x);
        int targetZ = Mathf.RoundToInt(forwardStep.z);

        if (!IsInsideWorld(targetX, targetZ))
            return; // don't move outside map

        // Start ray above the next position
        Vector3 rayStart = forwardStep + Vector3.up * 2.55f; // by making this value 2.55, we're saying that the max the ant can look up to move is 2 blocks (as specified by the assignment). We use 2.55 because the ant is slightly floating above the block
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 5f)) // by making this value 50, we're saying the ant can basically jump down to any level of block
        {
            float groundY = hit.point.y;

            // place ant on next block
            Vector3 newPosition = new Vector3(
                Mathf.Round(forwardStep.x),
                groundY,
                Mathf.Round(forwardStep.z)
            );

            if (WorldManager.Instance.GetBlock((int)Mathf.Round(forwardStep.x), (int)groundY, (int)Mathf.Round(forwardStep.z)) is AirBlock air)
            {
                // don't move! move to the side
                hitAHill = true;
                return;
            }


            if(!AreTwoAntsOnABlock(newPosition))
            {
                rb.MovePosition(newPosition);
            }
            else{
                // turn to a random directoin
                tooManyAnts = true;
            }
        }
        else{
            // theres a hill infront, so turn to a random direction!
            // tooManyAnts = true;
            // Move(); // i guess this is too many recursive calls....
            // tooManyAnts = false;
            // ok try turning around, orrr set a flag and say turn to random direction, yesss
            hitAHill = true;
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
            // it is mulch
            Mulch();

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
            // it is nest so donate to queen
            Nest();
        }

        if (block == 9)
        {
            Die();
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
            // MoveDown(); 
        }

    }

    void Grass()
    {
        //bro idk just ignore it?
    }

    void Acid()
    {
        //Ants standing on an AcidicBlock will have the rate at which their health decreases multiplied by 2
        health = health - 10;
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
        // donate
        DonateToQueen();
    }
    void Die()
    {
        Destroy(gameObject);
    }

    int CheckBlockMaterial(int x, int y, int z)
    {
        if (WorldManager.Instance == null)
        {
            Debug.LogWarning("WorkerAnt skipping logic: WorldManager not ready.");
            return 4;
        }

        AbstractBlock block = WorldManager.Instance.GetBlock(x, y, z);

        if (block == null)
        {
            Debug.LogWarning($"Block at ({x},{y-1},{z}) is null!");
            return 4;
        }

        return block.TextureID;
    }

    bool AreTwoAntsOnABlock(Vector3 positionFuture)
    {
        int numOfAnts = 0;
        if (WorldManager.Instance == null || WorldManager.Instance._instances == null)
        {
            return false; // no other ants yet
        }

        foreach (GameObject ant in WorldManager.Instance._instances)
        {
            if (ant == null || ant == this.gameObject) continue;

            Vector3Int otherBlock = GetBlockPosition(ant.transform.position);

            if (positionFuture == otherBlock)
            {
                numOfAnts++;
            }
        }
        if (numOfAnts >= 2)
        {
            return true;
        }

        return false;
    }

    bool IsAnotherAntOnSameBlock()
    {
        if (WorldManager.Instance == null || WorldManager.Instance._instances == null)
        {
            return false; // no other ants yet
        }
        Vector3Int myBlock = GetBlockPosition(transform.position);

        foreach (GameObject ant in WorldManager.Instance._instances)
        {
            if (ant == null || ant == this.gameObject) continue;

            Vector3Int otherBlock = GetBlockPosition(ant.transform.position);

            if (myBlock == otherBlock)
            {
                return true;
            }
        }

        return false;
    }
    GameObject AntFriend()
    {
        if (WorldManager.Instance == null || WorldManager.Instance._instances == null)
        {
            return this.gameObject; // no other ants yet
        }
        Vector3Int myBlock = GetBlockPosition(transform.position);

        foreach (GameObject ant in WorldManager.Instance._instances)
        {
            if (ant == null || ant == this.gameObject) continue;

            Vector3Int otherBlock = GetBlockPosition(ant.transform.position);

            if (myBlock == otherBlock)
            {
                return ant;
            }
        }
        return this.gameObject;
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


    void Donate()
    {
        //Ants may give some of their health to other ants occupying the same space (must be a zero-sum exchange
        if (IsAnotherAntOnSameBlock())
        {
            GameObject ant_friend = AntFriend();
            if (ant_friend != null)
            {
                WorkerAnt friend = ant_friend.GetComponent<WorkerAnt>();
                if (friend != null)
                {
                    if ((health > friend.health) && (health - 10 > 0) )
                    {
                        // donate
                        health = health - 10;
                        friend.health = friend.health + 10;
                    }
                    // move!
                    // Move();
                } else{
                    return;
                }
            } else{
                return;
            }
        }
    }

    void DonateToQueen()
    {
        QueenAnt queen = WorldManager.Instance.queen;
        if (queen != null){
            // donate no matter what
            int health_to_donate = (int)Mathf.Round(health/2);
            health = health - health_to_donate;
            queen.health = queen.health + health_to_donate;
        }else{
            return;
        }
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
    bool IsInsideWorld(int x, int z)
    {
        int maxX = WorldManager.Instance.WorldSizeX;
        int maxZ = WorldManager.Instance.WorldSizeZ;

        return x >= 0 && x < maxX &&
            z >= 0 && z < maxZ;
    }
    Vector3 FindBestPheromoneDirection()
    {
        Vector3[] directions = {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        float bestStrength = -1;
        Vector3 bestDir = directions[UnityEngine.Random.Range(0,4)];

        foreach (var dir in directions)
        {
            int x = Mathf.FloorToInt(transform.position.x + dir.x);
            int y = Mathf.FloorToInt(transform.position.y);
            int z = Mathf.FloorToInt(transform.position.z + dir.z);

            if (WorldManager.Instance.GetBlock(x,y,z) is AirBlock air)
            {
                if (air.closessToNest > bestStrength)
                {
                    bestStrength = air.closessToNest;
                    bestDir = dir;
                }
            }
        }

        return bestDir;
    }

    Vector3 GoTowardsMulchiness()
    {
        Vector3[] directions = {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        float bestStrength = -1;
        Vector3 bestDir = directions[UnityEngine.Random.Range(0,4)];

        foreach (var dir in directions)
        {
            int x = Mathf.FloorToInt(transform.position.x + dir.x);
            int y = Mathf.FloorToInt(transform.position.y);
            int z = Mathf.FloorToInt(transform.position.z + dir.z);

            if (WorldManager.Instance.GetBlock(x,y,z) is MulchBlock mulch)
            {
                if (mulch.mulchiSmell > bestStrength)
                {
                    bestStrength = mulch.mulchiSmell;
                    bestDir = dir;
                }
            }
        }

        return bestDir;
    }
}
