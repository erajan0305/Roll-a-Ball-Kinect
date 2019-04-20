using UnityEngine;
using Windows.Kinect;
using System;
using UnityEngine.UI;
using System.Collections;
public class DetectJoints : MonoBehaviour
{

    public GameObject BodySrcManager;
    public GameObject player;
    public Rigidbody rb;
    // public JointType TrackedJoint;
    private BodySourceManager bodyManager;
    private Body[] bodies;
    public float multiplier;
    public float multiplier1;
    
    // Use this for initialization
    void Start()
    {
        if (BodySrcManager == null)
        {
            Debug.Log("Assign Game Object with Body Source Manager");
        }
        else
        {
            rb = player.GetComponent<Rigidbody>();
            bodyManager = BodySrcManager.GetComponent<BodySourceManager>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (BodySrcManager == null)
        {
            return;
        }

        bodies = bodyManager.GetData();
        if (bodies == null)
        {
            return;
        }
        foreach (var body in bodies)
        {
            if (body == null)
            {
                continue;
            }
            if (body.IsTracked)
            {
                float diff;
                Vector3 SpineBase = new Vector3(body.Joints[JointType.SpineBase].Position.X, body.Joints[JointType.SpineBase].Position.Y, body.Joints[JointType.SpineBase].Position.Z);
                Vector3 HipLeft = new Vector3(body.Joints[JointType.HipLeft].Position.X, body.Joints[JointType.HipLeft].Position.Y, body.Joints[JointType.HipLeft].Position.Z);
                Vector3 Neck = new Vector3(body.Joints[JointType.Neck].Position.X, body.Joints[JointType.Neck].Position.Y, body.Joints[JointType.Neck].Position.Z);
                Vector3 HipRight = new Vector3(body.Joints[JointType.HipRight].Position.X, body.Joints[JointType.HipRight].Position.Y, body.Joints[JointType.HipRight].Position.Z);
                Vector3 AnkleRight = new Vector3(body.Joints[JointType.AnkleRight].Position.X, body.Joints[JointType.AnkleRight].Position.Y, body.Joints[JointType.AnkleRight].Position.Z);
                Vector3 AnkleLeft = new Vector3(body.Joints[JointType.AnkleLeft].Position.X, body.Joints[JointType.AnkleLeft].Position.Y, body.Joints[JointType.AnkleLeft].Position.Z);

                float HipLeftAngle = (float)GetAngle(HipLeft, SpineBase, Neck);
                float HipRightAngle = (float)GetAngle(HipRight, SpineBase, Neck);

                HipLeftAngle = (float)Math.Round((HipLeftAngle/360),3);
                HipRightAngle= (float)Math.Round((HipRightAngle / 360), 3);
                //bodyManager.text.text = "HLA: " + HipLeftAngle;
                //bodyManager.text1.text = "HRA: " + HipRightAngle;

                var playerObject = GameObject.Find("Player");
                Vector3 playerPos = playerObject.transform.position;
                if (HipLeftAngle < HipRightAngle)
                {
                    /*bodyManager.normal.text = " ";
                    bodyManager.normal1.text = "HLA<HRA";*/
                    //bodyManager.normal.text = "V:" + playerPos.z;
                    //bodyManager.normal1.text = "H:" + playerPos.x;
                    Vector3 movement = new Vector3(Vector3.left.x * HipLeftAngle * multiplier, 0.0f, 0.0f);
                    rb.AddForce(movement);
                    //player.transform.Rotate(Vector3.left, HipLeftAngle*5f);
                    //player.transform.Translate(-1f*Time.deltaTime,0f,0f);
                }
                else if (HipRightAngle < HipLeftAngle)
                {
                    /*bodyManager.normal1.text = " ";
                    bodyManager.normal.text = "HLA>HRA";*/       
                    //bodyManager.normal.text = "V:" + playerPos.z;
                    //bodyManager.normal1.text = "H:" + playerPos.x;
                    Vector3 movement = new Vector3(Vector3.right.x * HipRightAngle * multiplier, 0.0f, 0.0f);
                    rb.AddForce(movement);
                    //player.transform.Rotate(Vector3.right,HipRightAngle*5f);
                    //player.transform.Translate(1f*Time.deltaTime, 0f, 0f);
                }
                else
                {
                    player.transform.position = new Vector3(playerPos.x,playerPos.y,playerPos.z);
                    rb.AddForce(new Vector3(0, 0, 0));
                }

                if ((AnkleRight.z - AnkleLeft.z) > 0.1)
                {
                    diff = AnkleRight.z - AnkleLeft.z;
                    bodyManager.normal.text = "V:" + playerPos.z;
                    Vector3 movement = new Vector3(0.0f, 0.0f, Vector3.back.z * diff * multiplier1);
                    rb.AddForce(movement);
                }
                else if ((AnkleRight.z - AnkleLeft.z) < -0.1)
                {
                    diff = AnkleRight.z - AnkleLeft.z;
                    bodyManager.normal.text = "V:" + playerPos.z;
                    Vector3 movement = new Vector3(0.0f, 0.0f, -1*Vector3.forward.z * diff * multiplier1);
                    rb.AddForce(movement);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            bodyManager.count = bodyManager.count + 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        bodyManager.countText.text = "Pokemon: " + bodyManager.count.ToString();
        if (bodyManager.count >= 6)
        {
            bodyManager.winText.text = "All Caught!";
        }
    }

    public double GetAngle(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        vec1.Normalize();
        vec2.Normalize();
        vec3.Normalize();
        double vec1x = vec1.x;
        //double vec1y = vec1.y;
        //double vec1z = vec1.z;

        double vec2x = vec2.x;
        // double vec2y = vec2.y;
        //double vec2z = vec2.z;

        double vec3x = vec3.x;
        //double vec3y = vec3.y;
        //double vec3z = vec3.z;

        double shiftx1 = vec1x - vec2x;
        //double shifty1 = vec1y - vec2y;
        //double shiftz1 = vec1z - vec2z;

        double shiftx2 = vec3x - vec2x;
        //double shifty2 = vec3y - vec2y;
        //double shiftz2 = vec3z - vec2z;

        double product = shiftx1 * shiftx2;// + shifty1 * shifty2;// + shiftz1 * shiftz2;
        double mag1 = Math.Abs(Math.Sqrt(Math.Pow(shiftx1, 2)));// + Math.Pow(shifty1, 2)));// + Math.Pow(shiftz1, 2)));
        double mag2 = Math.Abs(Math.Sqrt(Math.Pow(shiftx2, 2)));// + Math.Pow(shifty2, 2)));// + Math.Pow(shiftz2, 2)));
        double temp = product / (mag1 * mag2);
        double angle = (180 / Math.PI) * Math.Atan(temp);
        angle = Math.Round(angle, 3);
        //bodyManager.normal1.text = angle.ToString();
        return angle;
    }
}