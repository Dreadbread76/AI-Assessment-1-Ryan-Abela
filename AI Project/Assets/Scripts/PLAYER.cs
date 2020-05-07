using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Game Systems/RPG/Player/Movement")]
[RequireComponent(typeof(CharacterController))]

public class PLAYER : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Physics")]
    public CharacterController controller;
    public float gravity = -20f;

    [Header("Movement ")]
    public float speed = 5f;
    public float walkSpeed = 5f;
    public float jumpSpeed = 8f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2f;
    public bool crouching;
    public bool walking;
    public Vector3 moveDirection;

    [Header("Stats")]
    
    public static float playerHealth = 100f;
    

    [Header("Bullets")]
    public GameObject bulletSpawn;
    public GameObject bullet;
    public float bulletSpeed = 12f;

    [System.Serializable]
    public struct KeybindInputs
    {
    
    }
    
    void Start()
    {


        //controller = this.gameObject.AddComponent<CharacterController>();
        
        
    }

    // Update is called once per frame
    #region Update Function
    #endregion

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            GameObject ShootBullet = Instantiate(bullet, bulletSpawn.transform.position, Camera.main.transform.rotation);
            Rigidbody ShootBulletRigid = ShootBullet.GetComponent<Rigidbody>();
            ShootBulletRigid.AddForce(Camera.main.transform.forward * bulletSpeed);
            

        }

        //MOVE WHEN GROUNDED
        if (controller.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y += gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            speed = sprintSpeed;
        }
       
        else
        {
            speed = walkSpeed;
        }
       
       
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            playerHealth = playerHealth - 20f;
            Debug.Log("health = " + playerHealth);
            
        }
    }
    public void MyHealth()
    {

    }
}
