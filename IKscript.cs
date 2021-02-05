using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IKscript : MonoBehaviour
{

    public int bones;

    public Transform Target;
    public Transform Pole;

    private GameObject[] Bones;
    private int nextBone, iterations;
    private float armLength, targetDistance;
    // Start is called before the first frame update
    void Start()
    {
        Bones = new GameObject[bones];
        findBones();
        nextBone = 1;
        armLength = Vector3.Distance(this.gameObject.transform.position, Bones[bones-1].transform.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = Vector3.Distance(Bones[bones - 1].transform.position, Target.position);

        IKorder();


    }

    void findBones()
    {
        Bones[0] = this.gameObject;
        for(int i = 1; i < bones; i++)
        {
            Bones[i] = Bones[i-1].transform.parent.gameObject;
        }
    }

    void setRoation(GameObject Bone, int next)
    {
        Vector3 endBone = this.gameObject.transform.position -Bone.transform.position;
        Vector3 Targetline = Target.position - Bone.transform.position;
        Quaternion boneRotation = Bone.transform.rotation;

        Quaternion rotation = Quaternion.FromToRotation(endBone, Targetline);
        Quaternion newRotation = rotation * boneRotation;

        Bone.transform.rotation = newRotation;
        

    }

    void pole(GameObject Bone, int next)
    {
        
        Plane plane = new Plane(Bones[next - 1].transform.position - Bones[next + 1].transform.position, Bones[next + 1].transform.position);
        Vector3 projectedPol = plane.ClosestPointOnPlane(Pole.position);
        Vector3 projectedBone = plane.ClosestPointOnPlane(Bone.transform.position);
        var angle = Vector3.SignedAngle(projectedBone - Bones[next + 1].transform.position, projectedPol - Bones[next + 1].transform.position, plane.normal);
        Bones[next].transform.position = Quaternion.AngleAxis(angle, plane.normal) * (Bones[next].transform.position - Bones[next+1].transform.position)  + Bones[next+1].transform.position;
        //Bones[next+1].transform.RotateAround(Bones[next+1].transform.position, plane.normal, angle);
    }


    void IKorder()
    {
        iterations = 0;
        do
        {
            for(int i = 1; i < bones; i++)
            {
                
                setRoation(Bones[i], i);
                
                if(i != 0 && i != bones-1)
                    pole(Bones[i], i);
            }

            iterations++;
        } while (Vector3.Distance(this.transform.position, Target.position) > 1  && iterations <= 10);
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < bones - 1; i++)
        {
            float scale = Vector3.Distance(Bones[i].transform.position, Bones[i + 1].transform.position);
            Handles.matrix = Matrix4x4.TRS(Bones[i].transform.position, Quaternion.FromToRotation(Vector3.up, Bones[i + 1].transform.position - Bones[i].transform.position), new Vector3(scale, Vector3.Distance(Bones[i + 1].transform.position, Bones[i].transform.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up *0.5f, new Vector3(0.2f,1,0.2f));
        }
    }
}
