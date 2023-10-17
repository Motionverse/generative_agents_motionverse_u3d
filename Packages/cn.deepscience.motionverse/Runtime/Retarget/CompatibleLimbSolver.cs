using UnityEngine;
namespace MotionverseSDK
{
    public class CompatibleLimbSolver
    {
        private Vector3 IKPosition = Vector3.zero;
        private Quaternion IKRotation = Quaternion.identity;
        private Vector3 bendNormal = Vector3.zero;
        private float mag  = 0;
        private float sqr_mag = 0;
        private CompatibleVirtualBone[] bones;
        private Vector3 rootPosition;
        private Quaternion rootRotation;

        Vector3 targetPosition = Vector3.zero;
        Quaternion targetRotation = Quaternion.identity;
        Vector3 IKPositionOffest = Vector3.zero;
        Quaternion IKRotationOffest = Quaternion.identity;


        public bool checkInited
        {
            get;
            private set;
        } = false;

        public void OnRead(Vector3[] positions, Quaternion[] rotations)
        {
            if (positions is null || rotations is null || positions.Length != rotations.Length || positions.Length != 3) return;

            if (!checkInited)
            {
                bones = new CompatibleVirtualBone[3];
                for (int i = 0; i < positions.Length; ++i)
                {
                    bones[i] = new CompatibleVirtualBone(positions[i], rotations[i]);
                }
                checkInited = true;
            }

            for (int i = 0; i < positions.Length; ++i)
            {
                bones[i].Read(positions[i], rotations[i]);
            }
        }

        public void PreSolve()
        {
            mag = CompatibleVirtualBone.PreSolve(ref bones);
            sqr_mag = mag * mag;
            IKPosition = targetPosition + IKPositionOffest;
            IKRotation = IKRotationOffest * targetRotation;
        }

        public void Solve()
        {
            CompatibleVirtualBone.SolveTrigonometric(bones, 0, 1, 2, IKPosition, bendNormal, 1.0f);
            bones[2].solverRotation = IKRotation;
        }

        public bool Write(out Vector3[] positions, out Quaternion[] rotations)
        {
            positions = new Vector3[3];
            rotations = new Quaternion[3];
            if (!checkInited)
            {
                return false;
            }

            positions[0]=(bones[0].solverPosition);
            rotations[0]=(bones[0].solverRotation);
            positions[1]=(bones[1].solverPosition);
            rotations[1]=(bones[1].solverRotation);
            positions[2]=(bones[2].solverPosition);
            rotations[2]=(bones[2].solverRotation);
            return true;
        }



        public void SetIKPositionOffest(Vector3 offest)
        {
            IKPositionOffest = offest;
        }

        public void SetIKRotationOffest(Quaternion offest)
        {
            IKRotationOffest = offest;
        }


        public void SetTarget(Vector3 targetPos, Quaternion targetRot, Vector3 bendDir)
        {
            targetPosition = targetPos;
            targetRotation = targetRot;
            bendNormal = bendDir;
        }

    }
}
