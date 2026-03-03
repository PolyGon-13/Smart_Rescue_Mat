using UnityEngine;

public class Control_and_Gravity : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = 0;
        float y = 0;
        float z = 0;

        if (Input.GetKey(KeyCode.A)) x -= 1;
        if (Input.GetKey(KeyCode.D)) x += 1;
        if (Input.GetKey(KeyCode.W)) y += 1;
        if (Input.GetKey(KeyCode.S)) y -= 1;
        if (Input.GetKey(KeyCode.Q)) z += 1;
        if (Input.GetKey(KeyCode.E)) z -= 1;

        Vector3 moveDir = new Vector3(x, y, z) * moveSpeed * Time.deltaTime;
        transform.Translate(moveDir);

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }
    }
}