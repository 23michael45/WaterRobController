using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class PlaneClipHelper : MonoBehaviour
{

    public Transform _PlaneObject;

    Material sharedMaterial;

    public void OnEnable()
    {
        //let's just create a new material instance.
        Renderer r = GetComponent<Renderer>();
        if(r)
        {

            sharedMaterial = r.sharedMaterial;
        }
        else
        {
            sharedMaterial = GetComponent<Projector>().material;

        }
        
    }

    public void Start() { }


    //preview size for the planes. Shown when the object is selected.
    public float planePreviewSize = 5.0f;

    //Positions and rotations for the planes. The rotations will be converted into normals to be used by the shaders.
    [HideInInspector]
    public Vector3 planePosition = Vector3.zero;
    [HideInInspector]
    public Quaternion planeRotation = Quaternion.identity;

    //Only used for previewing a plane. Draws diagonals and edges of a limited flat plane.
    private void DrawPlane(Vector3 position, Quaternion rot)
    {
        var forward = rot * Vector3.forward;
        var left = rot * Vector3.left;

        var forwardLeft = position + forward * planePreviewSize * 0.5f + left * planePreviewSize * 0.5f;
        var forwardRight = forwardLeft - left * planePreviewSize;
        var backRight = forwardRight - forward * planePreviewSize;
        var backLeft = forwardLeft - forward * planePreviewSize;

        Gizmos.DrawLine(position, forwardLeft);
        Gizmos.DrawLine(position, forwardRight);
        Gizmos.DrawLine(position, backRight);
        Gizmos.DrawLine(position, backLeft);

        Gizmos.DrawLine(forwardLeft, forwardRight);
        Gizmos.DrawLine(forwardRight, backRight);
        Gizmos.DrawLine(backRight, backLeft);
        Gizmos.DrawLine(backLeft, forwardLeft);
    }

    private void OnDrawGizmosSelected()
    {
       
            DrawPlane(planePosition, planeRotation);
     
    }

    //Ideally the planes do not need to be updated every frame, but we'll just keep the logic here for simplicity purposes.
    public void Update()
    {

        //Only should enable one keyword. If you want to enable any one of them, you actually need to disable the others. 
        //This may be a bug...

        //pass the planes to the shader if necessary.

        planePosition = _PlaneObject.transform.position;

        planeRotation = Quaternion.FromToRotation(Vector3.up, _PlaneObject.transform.up);

        if (sharedMaterial != null)
        {
            sharedMaterial.SetVector("_planePos", planePosition);
            //plane normal vector is the rotated 'up' vector.
            sharedMaterial.SetVector("_planeNorm", planeRotation * Vector3.up);
        }

    }
}