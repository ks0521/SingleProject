using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Utilities
{
    public static class ExtentionMethod
    {
        /// <summary>
        /// X,Y축의 입력값만큼 speed로 움직임
        /// </summary>
        /// <param name="rb">이동시킬 대상의 RigidBody</param>
        /// <param name="axisX">X축 입력(앞뒤) </param>
        /// <param name="axisZ">Z축 입력(좌우)</param>
        /// <param name="speed">이동 속도</param>
        public static void CustomMove(this Rigidbody rb, float axisX, float axisZ, float speed)
        {
            Vector3 velocity = new Vector3();
            velocity.x = axisX;
            velocity.y = 0;
            velocity.z = axisZ;
            
            velocity.Normalize();
            velocity = rb.transform.TransformDirection(velocity) * speed;
            
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
        }
    }

}
