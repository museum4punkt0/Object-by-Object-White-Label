using UnityEngine;
using System.Collections;

public class PinchRotate : MonoBehaviour 
{ 
    #region Fields
    const float PINCHTURNRATIO = Mathf.PI / 2;
    const float MINTURNANGLE = 0;
    const float PINCHRATIO = 1;

    // <summary> // The delta of the angle between two touch points // </summary> 
    private float m_TurnAngleDelta;

    // <summary> // The angle between two touch points // </summary> 
    private float m_TurnAngle;
    private Quaternion m_DesiredRotation;
    private Transform m_ThisTransform;
    #endregion

    /// <summary> ///   Calculates Pinch and Turn - This should be used inside LateUpdate /// </summary>
    private void Calculate ()  
    {
            m_TurnAngle = m_TurnAngleDelta = 0;

            // if two fingers are touching the screen at the same time ... 
            if (Input.touchCount == 2)  
            { 
                Touch touch1 = Input.touches[0]; 
                Touch touch2 = Input.touches[1];

                // ... if at least one of them moved ... 
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)  
                {
                        // ... check the delta angle between them ... 
                        m_TurnAngle = Angle(touch1.position, touch2.position); 
                        float prevTurn = Angle(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition); 
                        m_TurnAngleDelta = Mathf.DeltaAngle(prevTurn, m_TurnAngle);

                        // ... if it's greater than a minimum threshold, it's a turn! 
                        if (Mathf.Abs(m_TurnAngleDelta) > MINTURNANGLE)  
                        { 
                                m_TurnAngleDelta *= PINCHTURNRATIO; 
                        }  
                        else  
                        { 
                                m_TurnAngle = m_TurnAngleDelta = 0; 
                        } 
                    } 
            }
    }

    private float Angle (Vector2 pos1, Vector2 pos2)  
    { 
            Vector2 from = pos2 - pos1; 
            Vector2 to = new Vector2(1, 0);
            float result = Vector2.Angle( from, to ); 
            Vector3 cross = Vector3.Cross( from, to );

            if (cross.y > 0)  
            { 
                result = 360f - result; 
            }
            return result; 
    }

    private void Awake() 
    {
        m_ThisTransform = transform;
    }

    void LateUpdate()
    {             
            Calculate();
            
            if (Mathf.Abs(m_TurnAngleDelta) > 0) 
            {
                m_DesiredRotation = m_ThisTransform.rotation * Quaternion.Euler(0, 0, -m_TurnAngleDelta); 
            }
            
            m_ThisTransform.rotation = m_DesiredRotation;
    }
}