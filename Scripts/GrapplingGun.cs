using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;
    public Transform collectionPopup;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    public Vector2 grapplePoint;
    public Vector2 grappleDistanceVector;
    [HideInInspector] public RaycastHit2D grappleTarget;

    [SerializeField] private GameObject grappleOffsetParent;
    [SerializeField] private GameObject grapplePointLocal;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            if (grappleRope.enabled)
            {
                UpdateGrapplePoint();
                RotateGun(grapplePoint, false);

                if (grappleTarget.collider != null && grappleTarget.transform.tag == "Alien")
                    grappleTarget.transform.GetComponent<Animator>().SetBool("isGrappled", true);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            //m_rigidbody.gravityScale = 1;

            if (grappleTarget.collider != null && grappleTarget.transform.tag == "Alien")
                grappleTarget.transform.GetComponent<Animator>().SetBool("isGrappled", false);
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }

    public void PlayerCollision(Collider2D collider)
    {
        if (collider.tag == "Alien")
        {
            Alien alien = collider.GetComponent<Alien>();
            bool success = PlayerInventory.Add(alien.value);

            if (success)
            {
                SoundManager.instance.Play("Collection");

                Transform popupTransform = Instantiate(collectionPopup, transform.position, Quaternion.identity);
                popupTransform.GetComponent<CollectionPopup>().Setup(alien.value, alien.primary, alien.secondary);

                grapplePointLocal.transform.parent = grappleOffsetParent.transform;
                Destroy(collider.gameObject);

                if (grappleRope.enabled && grappleTarget.collider == collider)
                {
                    grappleRope.enabled = false;
                    m_springJoint2D.enabled = false;
                }
            }
        }
        else if (collider.tag == "Asteroid")
        {
            Asteroid asteroid = collider.GetComponent<Asteroid>();
            bool success = PlayerInventory.Subtract(asteroid.value);

            if (success)
            {
                SoundManager.instance.Play("Damage");

                Transform popupTransform = Instantiate(collectionPopup, transform.position, Quaternion.identity);
                popupTransform.GetComponent<CollectionPopup>().Setup(-asteroid.value, asteroid.primary, asteroid.secondary);
            }
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            //gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            grappleTarget = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (grappleTarget.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
            {
                if (Vector2.Distance(grappleTarget.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                {
                    grapplePointLocal.transform.position = grappleTarget.point;
                    grapplePointLocal.transform.parent = grappleTarget.transform;

                    grapplePoint = grappleTarget.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;

                    SoundManager.instance.Play("Grapple");
                }
            }
        }
    }

    void UpdateGrapplePoint()
    {
        grapplePoint = grapplePointLocal.transform.position;
        grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedBody = grappleTarget.rigidbody;
                    m_springJoint2D.connectedAnchor = grapplePointLocal.transform.localPosition;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;

                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    m_rigidbody.gravityScale = 0;
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }
}
