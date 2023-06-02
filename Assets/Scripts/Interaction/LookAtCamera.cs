using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Axis
    {
        X,
        Y,
        Z,
        All
    }
    #region Fields
    [SerializeField] private Axis m_RotateAxis = Axis.All;
    public Transform m_CameraTransform;
    private Transform m_ThisTransform;
    private Vector3 m_targetPosition;
    #endregion

    #region Methods
    #region Monobehaviour
    // Start is called before the first frame update
    void Start()
    {
        if(m_CameraTransform == null)
        {
            m_CameraTransform = Camera.main.transform;
        }

        m_ThisTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_RotateAxis)
        {
            case Axis.X:
                m_targetPosition = new Vector3(m_ThisTransform.position.x, m_CameraTransform.position.y, m_CameraTransform.position.z);
                break;
            case Axis.Y:
                m_targetPosition = new Vector3(m_CameraTransform.position.x, m_ThisTransform.position.y, m_CameraTransform.position.z);
                break;
            case Axis.Z:
                m_targetPosition = new Vector3(m_CameraTransform.position.x, m_CameraTransform.position.y, m_ThisTransform.position.z);
                break;
            case Axis.All:
            default:
                m_targetPosition = m_CameraTransform.position;
                break;
        }
        m_ThisTransform.LookAt(m_targetPosition);
    }
    #endregion
    #region Public
    public void Inflate(Camera camera)
    {
        m_CameraTransform = camera.transform;
    }
    #endregion
    #endregion
}
