using UnityEngine;

public class Move : MonoBehaviour
{
    Rigidbody rb;

    public float force = 50f;
    public float rotationSpeed = 100f;
    public ForceMode forceMode = ForceMode.Force;
    Vector3 direction;

    void Start()
    {
        direction = this.transform.forward;
        rb = this.GetComponent<Rigidbody>();
        Debug.Log(this.transform.forward);
        Debug.Log(this.transform.up);
        Debug.Log(this.transform.right);
    }

    void Update() {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        direction = new Vector3(horizontal, 0, vertical);
    }

    void FixedUpdate()
    {
        var horizontal = direction.x;
        var vertical = direction.z;

        #region Using physics (Rigidbody)

        if (vertical != 0) {
            var v = vertical * force * Time.deltaTime;
            // rb.AddForce(this.transform.forward * v, forceMode);
            // rb.velocity = this.transform.forward * vertical * force;
            rb.MovePosition(this.transform.position + (this.transform.forward * v));
        }

        // if (horizontal != 0) {
        //     var h = horizontal * rotationSpeed * Time.deltaTime;
        //     rb.AddTorque(this.transform.up * h);
        // }

        #endregion

        #region Using Transform (does not work with physics / collisions)

        // if (vertical != 0) {
        //     var v = vertical * force * Time.deltaTime;
        //     this.transform.Translate(Vector3.forward * v);
        // }

        if (horizontal != 0) {
            var h = horizontal * rotationSpeed * Time.deltaTime;
            this.transform.Rotate(Vector3.up, h);
        }

        #endregion

        Debug.DrawRay(this.transform.position, this.transform.forward, Color.red);

    }
}
