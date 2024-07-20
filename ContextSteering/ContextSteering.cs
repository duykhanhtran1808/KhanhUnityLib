using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ContextSteering : MonoBehaviour
{
    [SerializeField] int numOfRay = 8;
    [SerializeField] float distanceStop = 2f;
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float rotationModifier = 1f;
    [SerializeField] float rayDistance = 1.5f;
    [SerializeField] Transform targetTransform;

    public Vector3 bestVector = Vector3.zero;
    //Dictionary<float, Vector3> avoidVectors;
    HashSet<float> popularAngles;
    void Start()
    {
        //avoidVectors = new Dictionary<float, Vector3>();
        popularAngles = new HashSet<float>();
    }

    void Update()
    {
        CreateRay();
        if ((this.transform.position - targetTransform.position).magnitude > distanceStop)
        {
            RotateTowardsTarget();
            MoveTo(bestVector);
            
        }
        
    }

    private void MoveTo(Vector3 target)
    {
        
        //if(avoidVectors.Count > 0)
        //{
        //    foreach(Vector3 vectorAvoid in avoidVectors.Values)
        //    {
               
        //        this.transform.position = Vector3.MoveTowards(this.transform.position, vectorAvoid, moveSpeed * 2 * Time.deltaTime);
        //    }
        //}
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void RotateTowardsTarget()
    {
        Vector3 vectorToTarget = targetTransform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, rotSpeed * Time.deltaTime);
        
    }

    private void CreateRay()
    {
        //List<Vector3> possibleSolutions = new List<Vector3>();
        float shortestDistance = Mathf.Infinity;
        Vector3 shortestVector = Vector3.zero;
        float angle = 0;
        for(int i = 0; i < numOfRay; i ++)
        {
            Vector3 vectorForAngle = Quaternion.Euler(0, 0, angle) * transform.up;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vectorForAngle, rayDistance);

            foreach (RaycastHit2D hit in hits)
            {
                if ((hit.collider == null ||
                hit.collider.transform.gameObject == this.transform.GetChild(0).gameObject)
                && !popularAngles.Contains(angle))
                {
                    Vector3 futurePos = this.transform.position + vectorForAngle;
                    float futureDistance = Vector3.Distance(futurePos, targetTransform.position);
                    if(futureDistance < shortestDistance)
                    {
                        shortestVector = futurePos;
                    }
                    Debug.DrawRay(transform.position, vectorForAngle * rayDistance, Color.green);
                    //if(avoidVectors.ContainsKey(angle))
                    //{
                    //    avoidVectors.Remove(angle);
                    //}
                }
                else if (hit.collider != null)
                {
                    //hit.collider.gameObject == this.gameObject
                    if(!hit.collider.gameObject.CompareTag("Player"))
                    {
                        Debug.DrawRay(transform.position, vectorForAngle * rayDistance, Color.red);

                        float x = -vectorForAngle.x;
                        float y = -vectorForAngle.y;
                        Vector3 futurePos = this.transform.position + new Vector3(x, y);
                        //avoidVectors.Add(angle, futurePos);
                        popularAngles.Add(angle);
                        StartCoroutine(UnlockAngle(angle));

                    }
                    else
                    {
                        Debug.DrawRay(transform.position, vectorForAngle * rayDistance, Color.blue);
                    }
                }
            }
            angle -= 360 / numOfRay;
        }
        bestVector = shortestVector;
    }
    IEnumerator UnlockAngle(float angle)
    {
        yield return new WaitForSeconds(2f);
        popularAngles.Remove(angle);
    }

}
