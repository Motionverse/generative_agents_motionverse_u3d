using UnityEngine;
namespace MotionverseSDK
{
    public struct Joint_t
    {
        public float px;
        public float py;
        public float pz;

        public float qx;
        public float qy;
        public float qz;
        public float qw;

        public int limb;
        public int index;
        public static Joint_t Create(Vector3 pos, Quaternion rot)
        {
            Joint_t joint = new Joint_t();
            joint.px = pos.x;
            joint.py = pos.y;
            joint.pz = pos.z;
            joint.qx = rot.x;
            joint.qy = rot.y;
            joint.qz = rot.z;
            joint.qw = rot.w;

            joint.limb = -1;
            joint.index = 0;
            return joint;
        }
    };
    
}