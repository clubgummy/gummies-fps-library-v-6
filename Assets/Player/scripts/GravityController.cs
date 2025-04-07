using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float velocity;
    public float object_mass = 0.05f;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);


    public void ApplyGravity(CharacterController controller, float jumpForce)
    {
        velocity += gravity.y * object_mass * Time.deltaTime;

        controller.Move(new Vector3(0, velocity * jumpForce, 0));
    }

    public void Apply_InversGravity(CharacterController controller, float jumpForce)
    {
        velocity -= gravity.y * object_mass * Time.deltaTime;

        controller.Move(new Vector3(0, velocity * jumpForce, 0));
    }
}
