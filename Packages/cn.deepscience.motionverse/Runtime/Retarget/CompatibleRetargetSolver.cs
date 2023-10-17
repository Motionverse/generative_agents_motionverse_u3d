using System.Collections.Generic;
using UnityEngine;
namespace MotionverseSDK
{
    public class CompatibleRetargetSolver
    {
        private CompatibleVirtualBone rootPose;
        private CompatibleVirtualBone[] spinesPose;
        private CompatibleVirtualBone[] leftLegPose;
        private CompatibleVirtualBone[] rightLegPose;
        private CompatibleVirtualBone[] necksPose;
        private CompatibleVirtualBone[] leftArmPose;
        private CompatibleVirtualBone[] rightArmPose;

        private CompatibleVirtualBone[] leftThumbsPose;
        private CompatibleVirtualBone[] leftIndexsPose;
        private CompatibleVirtualBone[] leftMiddlesPose;
        private CompatibleVirtualBone[] leftRingsPose;
        private CompatibleVirtualBone[] leftPinkysPose;

        private CompatibleVirtualBone[] rightThumbsPose;
        private CompatibleVirtualBone[] rightIndexsPose;
        private CompatibleVirtualBone[] rightMiddlesPose;
        private CompatibleVirtualBone[] rightRingsPose;
        private CompatibleVirtualBone[] rightPinkysPose;

        private CompatibleVirtualBone root;
        private CompatibleVirtualBone[] spines;
        private CompatibleVirtualBone[] leftLeg;
        private CompatibleVirtualBone[] rightLeg;
        private CompatibleVirtualBone[] necks;
        private CompatibleVirtualBone[] leftArm;
        private CompatibleVirtualBone[] rightArm;

        private CompatibleVirtualBone[] leftThumbs;
        private CompatibleVirtualBone[] leftIndexs;
        private CompatibleVirtualBone[] leftMiddles;
        private CompatibleVirtualBone[] leftRings;
        private CompatibleVirtualBone[] leftPinkys;

        private CompatibleVirtualBone[] rightThumbs;
        private CompatibleVirtualBone[] rightIndexs;
        private CompatibleVirtualBone[] rightMiddles;
        private CompatibleVirtualBone[] rightRings;
        private CompatibleVirtualBone[] rightPinkys;

        private Dictionary<RetargetBoneType, Quaternion> localRotationDictionary = new Dictionary<RetargetBoneType, Quaternion>();
        private Quaternion[] repositionLeftArmRots = new Quaternion[3];
        private Quaternion[] repositionRightArmRots = new Quaternion[3];
        Vector3 leftRotaboneRelativeToForearm = Vector3.zero;
        Vector3 rightRotaboneRelativeToForearm = Vector3.zero;
        Vector3 srcLeftBendNormalRelativeToUpperArm = Vector3.zero;
        Vector3 srcRightBendNormalRelativeToUpperArm = Vector3.zero;

        private Vector3 originPosition = Vector3.zero;
        private Quaternion originRotation = Quaternion.identity;

        private float lowBodyScale = 1f;
        private float highScale = 1f;
        private Dictionary<RetargetBoneType, Joint_t> lastSrcFrame = null;
        private Vector3 lastTargetHipPosition = Vector3.zero;
        CompatibleLimbSolver[] arms = new CompatibleLimbSolver[2];

        private bool hasFinger = false;
        private bool hasRotabone = false;

        public Vector3 leftHandPosOffset = Vector3.zero;
        public Quaternion leftHandRotOffset = Quaternion.identity;
        public Vector3 rightHandPosOffset = Vector3.zero;
        public Quaternion rightHandRotOffset = Quaternion.identity;
        public Quaternion headOffset = Quaternion.identity;

        public bool initialized
        {
            get;
            private set;
        } = false;

        public bool Init(Dictionary<RetargetBoneType, Joint_t> srcTPose, Dictionary<RetargetBoneType, Joint_t> tarTPose)
        {
            bool fingerTag1 = false;
            bool rotaboneTag1 = false;
            bool fingerTag2 = false;
            bool rotaboneTag2 = false;
            if (Check(srcTPose, ref fingerTag1, ref rotaboneTag1) && Check(tarTPose, ref fingerTag2, ref rotaboneTag2))
            {
                hasRotabone = rotaboneTag2;
                hasFinger = fingerTag1 && fingerTag2;

                ReadAttributes(tarTPose);
                RecordLocalRotations(srcTPose, tarTPose);
                lowBodyScale = (Vector3.Distance(leftLegPose[3].solverPosition, leftLegPose[2].solverPosition) +
                    Vector3.Distance(leftLegPose[2].solverPosition, leftLegPose[1].solverPosition) +
                    Vector3.Distance(leftLegPose[1].solverPosition, leftLegPose[0].solverPosition) +
                    Vector3.Distance(leftLegPose[0].solverPosition, spinesPose[0].solverPosition) +
                    Vector3.Distance(spinesPose[0].solverPosition, rightLegPose[0].solverPosition) +
                    Vector3.Distance(rightLegPose[0].solverPosition, rightLegPose[1].solverPosition) +
                    Vector3.Distance(rightLegPose[1].solverPosition, rightLegPose[2].solverPosition) +
                    Vector3.Distance(rightLegPose[2].solverPosition, rightLegPose[3].solverPosition)) /
                    (Vector3.Distance(Position(srcTPose[RetargetBoneType.LeftToe]), Position(srcTPose[RetargetBoneType.LeftFoot])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.LeftFoot]), Position(srcTPose[RetargetBoneType.LeftCalf])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.LeftCalf]), Position(srcTPose[RetargetBoneType.LeftThigh])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.LeftThigh]), Position(srcTPose[RetargetBoneType.Hip])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.Hip]), Position(srcTPose[RetargetBoneType.RightThigh])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.RightThigh]), Position(srcTPose[RetargetBoneType.RightCalf])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.RightCalf]), Position(srcTPose[RetargetBoneType.RightFoot])) +
                    Vector3.Distance(Position(srcTPose[RetargetBoneType.RightFoot]), Position(srcTPose[RetargetBoneType.RightToe])));
                highScale = (spinesPose[0].solverPosition.y - rootPose.solverPosition.y) / 
                    (Position(srcTPose[RetargetBoneType.Hip]).y - Position(srcTPose[RetargetBoneType.Root]).y);

                for (int i = 0; i < 2; ++i)
                {
                    arms[i] = new CompatibleLimbSolver();
                }
                srcLeftBendNormalRelativeToUpperArm = Quaternion.Inverse(Rotation(srcTPose[RetargetBoneType.LeftUpperarm])) * Vector3.up;
                srcRightBendNormalRelativeToUpperArm = Quaternion.Inverse(Rotation(srcTPose[RetargetBoneType.RightUpperarm])) * Vector3.down;

                initialized = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateSolver(Dictionary<RetargetBoneType, Joint_t> frame)
        {
            if (!initialized) return false;
            bool fingerTag = false;
            bool rotaboneTag = false;
            if (Check(frame, ref fingerTag, ref rotaboneTag))
            {
                ApplyRotations(frame);
                ApplyHeadOffset();
                RetargetDisplacement(frame);
                Vector3[] fixedHands = CalNormlizedHandTarget(frame);
                Vector3[] positions = new Vector3[3];
                Quaternion[] rotations = new Quaternion[3];
                Get(leftArm, 1, ref positions, ref rotations);
                arms[0].OnRead(positions, rotations);
                arms[0].SetIKPositionOffest(leftHandPosOffset);
                arms[0].SetIKRotationOffest(leftHandRotOffset);
                arms[0].SetTarget(fixedHands[0], leftArm[3].solverRotation, Rotation(frame[RetargetBoneType.LeftUpperarm]) * srcLeftBendNormalRelativeToUpperArm);
                arms[0].PreSolve();
                arms[0].Solve();
                arms[0].Write(out positions, out rotations);
                CompatibleVirtualBone.RotateTo(ref leftArm, 1, rotations[0]);
                CompatibleVirtualBone.RotateTo(ref leftArm, 2, rotations[1]);
                CompatibleVirtualBone.RotateTo(ref leftArm, 3, rotations[2]);

                Get(rightArm, 1, ref positions, ref rotations);
                arms[1].OnRead(positions, rotations);
                arms[1].SetIKPositionOffest(rightHandPosOffset);
                arms[1].SetIKRotationOffest(rightHandRotOffset);
                arms[1].SetTarget(fixedHands[1], rightArm[3].solverRotation, Rotation(frame[RetargetBoneType.RightUpperarm]) * srcRightBendNormalRelativeToUpperArm);
                arms[1].PreSolve();
                arms[1].Solve();
                arms[1].Write(out positions, out rotations);
                CompatibleVirtualBone.RotateTo(ref rightArm, 1, rotations[0]);
                CompatibleVirtualBone.RotateTo(ref rightArm, 2, rotations[1]);
                CompatibleVirtualBone.RotateTo(ref rightArm, 3, rotations[2]);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            initialized = false;
            lowBodyScale = 1f;
            highScale = 1f;
            originPosition = Vector3.zero;
            originRotation = Quaternion.identity;
            leftRotaboneRelativeToForearm = Vector3.zero;
            rightRotaboneRelativeToForearm = Vector3.zero;
            lastTargetHipPosition = Vector3.zero;
            localRotationDictionary.Clear();
            for (int i = 0; i < repositionLeftArmRots.Length; ++i)
            {
                repositionLeftArmRots[i] = Quaternion.identity;
                repositionRightArmRots[i] = Quaternion.identity;
            }
            lastSrcFrame = null;
        }

        public Dictionary<RetargetBoneType, Joint_t> GetFrame()
        {
            if (!initialized) return new Dictionary<RetargetBoneType, Joint_t>();

            Dictionary<RetargetBoneType, Joint_t> frame = new Dictionary<RetargetBoneType, Joint_t>();
            frame.Add(RetargetBoneType.Root, Joint_t.Create(root.solverPosition, root.solverRotation));
            frame.Add(RetargetBoneType.Hip, Joint_t.Create(spines[0].solverPosition, spines[0].solverRotation));
            frame.Add(RetargetBoneType.LeftThigh, Joint_t.Create(leftLeg[0].solverPosition, leftLeg[0].solverRotation));
            frame.Add(RetargetBoneType.LeftCalf, Joint_t.Create(leftLeg[1].solverPosition, leftLeg[1].solverRotation));
            frame.Add(RetargetBoneType.LeftFoot, Joint_t.Create(leftLeg[2].solverPosition, leftLeg[2].solverRotation));
            frame.Add(RetargetBoneType.LeftToe, Joint_t.Create(leftLeg[3].solverPosition, leftLeg[3].solverRotation));
            frame.Add(RetargetBoneType.RightThigh, Joint_t.Create(rightLeg[0].solverPosition, rightLeg[0].solverRotation));
            frame.Add(RetargetBoneType.RightCalf, Joint_t.Create(rightLeg[1].solverPosition, rightLeg[1].solverRotation));
            frame.Add(RetargetBoneType.RightFoot, Joint_t.Create(rightLeg[2].solverPosition, rightLeg[2].solverRotation));
            frame.Add(RetargetBoneType.RightToe, Joint_t.Create(rightLeg[3].solverPosition, rightLeg[3].solverRotation));
            frame.Add(RetargetBoneType.Spine, Joint_t.Create(spines[1].solverPosition, spines[1].solverRotation));
            frame.Add(RetargetBoneType.Spine1, Joint_t.Create(spines[2].solverPosition, spines[2].solverRotation));
            frame.Add(RetargetBoneType.Chest, Joint_t.Create(spines[3].solverPosition, spines[3].solverRotation));
            frame.Add(RetargetBoneType.Neck, Joint_t.Create(necks[0].solverPosition, necks[0].solverRotation));
            frame.Add(RetargetBoneType.Head, Joint_t.Create(necks[1].solverPosition, necks[1].solverRotation));
            frame.Add(RetargetBoneType.LeftShoulder, Joint_t.Create(leftArm[0].solverPosition, leftArm[0].solverRotation));
            frame.Add(RetargetBoneType.LeftUpperarm, Joint_t.Create(leftArm[1].solverPosition, leftArm[1].solverRotation));
            frame.Add(RetargetBoneType.LeftForearm, Joint_t.Create(leftArm[2].solverPosition, leftArm[2].solverRotation));
            frame.Add(RetargetBoneType.LeftHand, Joint_t.Create(leftArm[3].solverPosition, leftArm[3].solverRotation));
            frame.Add(RetargetBoneType.RightShoulder, Joint_t.Create(rightArm[0].solverPosition, rightArm[0].solverRotation));
            frame.Add(RetargetBoneType.RightUpperarm, Joint_t.Create(rightArm[1].solverPosition, rightArm[1].solverRotation));
            frame.Add(RetargetBoneType.RightForearm, Joint_t.Create(rightArm[2].solverPosition, rightArm[2].solverRotation));
            frame.Add(RetargetBoneType.RightHand, Joint_t.Create(rightArm[3].solverPosition, rightArm[3].solverRotation));
            if (hasRotabone)
            {
                Quaternion standardHandRot = leftArm[3].solverRotation * repositionLeftArmRots[2];
                Quaternion standardForearmRot = leftArm[2].solverRotation * repositionLeftArmRots[0];
                Vector3 right = standardForearmRot * Vector3.right;
                Vector3 forward = Vector3.ProjectOnPlane(standardHandRot * Vector3.forward, right);
                Vector3 up = Vector3.Cross(forward, right);
                Quaternion qProject = Quaternion.Slerp(standardForearmRot, Quaternion.LookRotation(forward, up), 0.5f);
                Quaternion leftRotaboneRotation = qProject * Quaternion.Inverse(repositionLeftArmRots[1]);

                standardHandRot = rightArm[3].solverRotation * repositionRightArmRots[2];
                standardForearmRot = rightArm[2].solverRotation * repositionRightArmRots[0];
                right = standardForearmRot * Vector3.right;
                forward = Vector3.ProjectOnPlane(standardHandRot * Vector3.forward, right);
                up = Vector3.Cross(forward, right);
                qProject = Quaternion.Slerp(standardForearmRot, Quaternion.LookRotation(forward, up), 0.5f);
                Quaternion rightRotaboneRotation = qProject * Quaternion.Inverse(repositionRightArmRots[1]);
                Vector3 leftRotabonePos = leftArm[2].solverRotation * leftRotaboneRelativeToForearm + leftArm[2].solverPosition;
                Vector3 rightRotabonePos = rightArm[2].solverRotation * rightRotaboneRelativeToForearm + rightArm[2].solverPosition;

                frame.Add(RetargetBoneType.LeftRotaBone, Joint_t.Create(leftRotabonePos, leftRotaboneRotation));
                frame.Add(RetargetBoneType.RightRotaBone, Joint_t.Create(rightRotabonePos, rightRotaboneRotation));
            }
            if (hasFinger)
            {
                for (int i = 0; i < fingerTypes.Length; ++i)
                {
                    switch (i / 4) 
                    {
                        case 0:
                            frame.Add(fingerTypes[i], Joint_t.Create(leftThumbs[i % 4].solverPosition, leftThumbs[i % 4].solverRotation));
                            break;
                        case 1:
                            frame.Add(fingerTypes[i], Joint_t.Create(leftIndexs[i % 4].solverPosition, leftIndexs[i % 4].solverRotation));
                            break;
                        case 2:
                            frame.Add(fingerTypes[i], Joint_t.Create(leftMiddles[i % 4].solverPosition, leftMiddles[i % 4].solverRotation));
                            break;
                        case 3:
                            frame.Add(fingerTypes[i], Joint_t.Create(leftRings[i % 4].solverPosition, leftRings[i % 4].solverRotation));
                            break;
                        case 4:
                            frame.Add(fingerTypes[i], Joint_t.Create(leftPinkys[i % 4].solverPosition, leftPinkys[i % 4].solverRotation));
                            break;
                        case 5:
                            frame.Add(fingerTypes[i], Joint_t.Create(rightThumbs[i % 4].solverPosition, rightThumbs[i % 4].solverRotation));
                            break;
                        case 6:
                            frame.Add(fingerTypes[i], Joint_t.Create(rightIndexs[i % 4].solverPosition, rightIndexs[i % 4].solverRotation));
                            break;
                        case 7:
                            frame.Add(fingerTypes[i], Joint_t.Create(rightMiddles[i % 4].solverPosition, rightMiddles[i % 4].solverRotation));
                            break;
                        case 8:
                            frame.Add(fingerTypes[i], Joint_t.Create(rightRings[i % 4].solverPosition, rightRings[i % 4].solverRotation));
                            break;
                        case 9:
                            frame.Add(fingerTypes[i], Joint_t.Create(rightPinkys[i % 4].solverPosition, rightPinkys[i % 4].solverRotation));
                            break;
                        default:
                            break;
                    }
                }
            }
            return TransformToOrigin(frame);
        }

        public void SetOrigin(Vector3 position, Quaternion rotation)
        {
            originPosition = position;
            originRotation = rotation;
        }

        #region util method

        private static RetargetBoneType[] bodyTypes = new RetargetBoneType[]
        {
                RetargetBoneType.Root,
                RetargetBoneType.Hip,
                RetargetBoneType.LeftThigh,
                RetargetBoneType.LeftCalf,
                RetargetBoneType.LeftFoot,
                RetargetBoneType.LeftToe,
                RetargetBoneType.RightThigh,
                RetargetBoneType.RightCalf,
                RetargetBoneType.RightFoot,
                RetargetBoneType.RightToe,
                RetargetBoneType.Spine,
                RetargetBoneType.Spine1,
                RetargetBoneType.Chest,
                RetargetBoneType.Neck,
                RetargetBoneType.Head,
                RetargetBoneType.LeftShoulder,
                RetargetBoneType.LeftUpperarm,
                RetargetBoneType.LeftForearm,
                RetargetBoneType.LeftHand,
                RetargetBoneType.RightShoulder,
                RetargetBoneType.RightUpperarm,
                RetargetBoneType.RightForearm,
                RetargetBoneType.RightHand,
            };

        private static RetargetBoneType[] fingerTypes = new RetargetBoneType[]
        {
                RetargetBoneType.LeftThumb1,
                RetargetBoneType.LeftThumb2,
                RetargetBoneType.LeftThumb3,
                RetargetBoneType.LeftThumb4,
                RetargetBoneType.LeftIndex1,
                RetargetBoneType.LeftIndex2,
                RetargetBoneType.LeftIndex3,
                RetargetBoneType.LeftIndex4,
                RetargetBoneType.LeftMiddle1,
                RetargetBoneType.LeftMiddle2,
                RetargetBoneType.LeftMiddle3,
                RetargetBoneType.LeftMiddle4,
                RetargetBoneType.LeftRing1,
                RetargetBoneType.LeftRing2,
                RetargetBoneType.LeftRing3,
                RetargetBoneType.LeftRing4,
                RetargetBoneType.LeftPinky1,
                RetargetBoneType.LeftPinky2,
                RetargetBoneType.LeftPinky3,
                RetargetBoneType.LeftPinky4,
                RetargetBoneType.RightThumb1,
                RetargetBoneType.RightThumb2,
                RetargetBoneType.RightThumb3,
                RetargetBoneType.RightThumb4,
                RetargetBoneType.RightIndex1,
                RetargetBoneType.RightIndex2,
                RetargetBoneType.RightIndex3,
                RetargetBoneType.RightIndex4,
                RetargetBoneType.RightMiddle1,
                RetargetBoneType.RightMiddle2,
                RetargetBoneType.RightMiddle3,
                RetargetBoneType.RightMiddle4,
                RetargetBoneType.RightRing1,
                RetargetBoneType.RightRing2,
                RetargetBoneType.RightRing3,
                RetargetBoneType.RightRing4,
                RetargetBoneType.RightPinky1,
                RetargetBoneType.RightPinky2,
                RetargetBoneType.RightPinky3,
                RetargetBoneType.RightPinky4,
        };

        private static RetargetBoneType[] rotaboneTypes = new RetargetBoneType[]
        {
                RetargetBoneType.LeftRotaBone,
                RetargetBoneType.RightRotaBone,
        };

        private Dictionary<RetargetBoneType, Joint_t> TransformToOrigin(Dictionary<RetargetBoneType, Joint_t> frame)
        {
            Dictionary<RetargetBoneType, Joint_t> transformJoints = new Dictionary<RetargetBoneType, Joint_t>();
            foreach (var item in frame)
            {
                Vector3 tranformPos = originRotation * Position(item.Value) + originPosition;
                Quaternion transformRot = originRotation * Rotation(item.Value);
                transformJoints.Add(item.Key, Joint_t.Create(tranformPos, transformRot));
            }
            return transformJoints;
        }

        private void ApplyRotations(Dictionary<RetargetBoneType, Joint_t> frame)
        {
            ReadBeforeSolve();
            Vector3 lastChestPos = spines[3].solverPosition;
            Quaternion lastChestRot = spines[3].solverRotation;
            Vector3 lastHipPos = spines[0].solverPosition;
            Quaternion lastHipRot = spines[0].solverRotation;
            CompatibleVirtualBone.RotateTo(ref spines, 0, Rotation(frame[RetargetBoneType.Hip]) * localRotationDictionary[RetargetBoneType.Hip]);
            CompatibleVirtualBone.RotateTo(ref spines, 1, Rotation(frame[RetargetBoneType.Spine]) * localRotationDictionary[RetargetBoneType.Spine]);
            CompatibleVirtualBone.RotateTo(ref spines, 2, Rotation(frame[RetargetBoneType.Spine1]) * localRotationDictionary[RetargetBoneType.Spine1]);
            CompatibleVirtualBone.RotateTo(ref spines, 3, Rotation(frame[RetargetBoneType.Chest]) * localRotationDictionary[RetargetBoneType.Chest]);
            TranslateArms(lastChestPos, lastChestRot, spines[3].solverPosition, spines[3].solverRotation);
            TranslateNecks(lastChestPos, lastChestRot, spines[3].solverPosition, spines[3].solverRotation);
            TranslateLegs(lastHipPos, lastHipRot, spines[0].solverPosition, spines[0].solverRotation);

            CompatibleVirtualBone.RotateTo(ref leftLeg, 0, Rotation(frame[RetargetBoneType.LeftThigh]) * localRotationDictionary[RetargetBoneType.LeftThigh]);
            CompatibleVirtualBone.RotateTo(ref leftLeg, 1, Rotation(frame[RetargetBoneType.LeftCalf]) * localRotationDictionary[RetargetBoneType.LeftCalf]);
            CompatibleVirtualBone.RotateTo(ref leftLeg, 2, Rotation(frame[RetargetBoneType.LeftFoot]) * localRotationDictionary[RetargetBoneType.LeftFoot]);
            CompatibleVirtualBone.RotateTo(ref leftLeg, 3, Rotation(frame[RetargetBoneType.LeftToe]) * localRotationDictionary[RetargetBoneType.LeftToe]);
            CompatibleVirtualBone.RotateTo(ref rightLeg, 0, Rotation(frame[RetargetBoneType.RightThigh]) * localRotationDictionary[RetargetBoneType.RightThigh]);
            CompatibleVirtualBone.RotateTo(ref rightLeg, 1, Rotation(frame[RetargetBoneType.RightCalf]) * localRotationDictionary[RetargetBoneType.RightCalf]);
            CompatibleVirtualBone.RotateTo(ref rightLeg, 2, Rotation(frame[RetargetBoneType.RightFoot]) * localRotationDictionary[RetargetBoneType.RightFoot]);
            CompatibleVirtualBone.RotateTo(ref rightLeg, 3, Rotation(frame[RetargetBoneType.RightToe]) * localRotationDictionary[RetargetBoneType.RightToe]);

            CompatibleVirtualBone.RotateTo(ref necks, 0, Rotation(frame[RetargetBoneType.Neck]) * localRotationDictionary[RetargetBoneType.Neck]);
            CompatibleVirtualBone.RotateTo(ref necks, 1, Rotation(frame[RetargetBoneType.Head]) * localRotationDictionary[RetargetBoneType.Head]);

            CompatibleVirtualBone.RotateTo(ref leftArm, 0, Rotation(frame[RetargetBoneType.LeftShoulder]) * localRotationDictionary[RetargetBoneType.LeftShoulder]);
            CompatibleVirtualBone.RotateTo(ref leftArm, 1, Rotation(frame[RetargetBoneType.LeftUpperarm]) * localRotationDictionary[RetargetBoneType.LeftUpperarm]);
            CompatibleVirtualBone.RotateTo(ref leftArm, 2, Rotation(frame[RetargetBoneType.LeftForearm]) * localRotationDictionary[RetargetBoneType.LeftForearm]);
            CompatibleVirtualBone.RotateTo(ref leftArm, 3, Rotation(frame[RetargetBoneType.LeftHand]) * localRotationDictionary[RetargetBoneType.LeftHand]);

            CompatibleVirtualBone.RotateTo(ref rightArm, 0, Rotation(frame[RetargetBoneType.RightShoulder]) * localRotationDictionary[RetargetBoneType.RightShoulder]);
            CompatibleVirtualBone.RotateTo(ref rightArm, 1, Rotation(frame[RetargetBoneType.RightUpperarm]) * localRotationDictionary[RetargetBoneType.RightUpperarm]);
            CompatibleVirtualBone.RotateTo(ref rightArm, 2, Rotation(frame[RetargetBoneType.RightForearm]) * localRotationDictionary[RetargetBoneType.RightForearm]);
            CompatibleVirtualBone.RotateTo(ref rightArm, 3, Rotation(frame[RetargetBoneType.RightHand]) * localRotationDictionary[RetargetBoneType.RightHand]);

            if (hasFinger)
            {
                CompatibleVirtualBone.RotateTo(ref leftThumbs, 0, Rotation(frame[RetargetBoneType.LeftThumb1]) * localRotationDictionary[RetargetBoneType.LeftThumb1]);
                CompatibleVirtualBone.RotateTo(ref leftThumbs, 1, Rotation(frame[RetargetBoneType.LeftThumb2]) * localRotationDictionary[RetargetBoneType.LeftThumb2]);
                CompatibleVirtualBone.RotateTo(ref leftThumbs, 2, Rotation(frame[RetargetBoneType.LeftThumb3]) * localRotationDictionary[RetargetBoneType.LeftThumb3]);
                CompatibleVirtualBone.RotateTo(ref leftThumbs, 3, Rotation(frame[RetargetBoneType.LeftThumb4]) * localRotationDictionary[RetargetBoneType.LeftThumb4]);

                CompatibleVirtualBone.RotateTo(ref leftIndexs, 0, Rotation(frame[RetargetBoneType.LeftIndex1]) * localRotationDictionary[RetargetBoneType.LeftIndex1]);
                CompatibleVirtualBone.RotateTo(ref leftIndexs, 1, Rotation(frame[RetargetBoneType.LeftIndex2]) * localRotationDictionary[RetargetBoneType.LeftIndex2]);
                CompatibleVirtualBone.RotateTo(ref leftIndexs, 2, Rotation(frame[RetargetBoneType.LeftIndex3]) * localRotationDictionary[RetargetBoneType.LeftIndex3]);
                CompatibleVirtualBone.RotateTo(ref leftIndexs, 3, Rotation(frame[RetargetBoneType.LeftIndex4]) * localRotationDictionary[RetargetBoneType.LeftIndex4]);

                CompatibleVirtualBone.RotateTo(ref leftMiddles, 0, Rotation(frame[RetargetBoneType.LeftMiddle1]) * localRotationDictionary[RetargetBoneType.LeftMiddle1]);
                CompatibleVirtualBone.RotateTo(ref leftMiddles, 1, Rotation(frame[RetargetBoneType.LeftMiddle2]) * localRotationDictionary[RetargetBoneType.LeftMiddle2]);
                CompatibleVirtualBone.RotateTo(ref leftMiddles, 2, Rotation(frame[RetargetBoneType.LeftMiddle3]) * localRotationDictionary[RetargetBoneType.LeftMiddle3]);
                CompatibleVirtualBone.RotateTo(ref leftMiddles, 3, Rotation(frame[RetargetBoneType.LeftMiddle4]) * localRotationDictionary[RetargetBoneType.LeftMiddle4]);

                CompatibleVirtualBone.RotateTo(ref leftRings, 0, Rotation(frame[RetargetBoneType.LeftRing1]) * localRotationDictionary[RetargetBoneType.LeftRing1]);
                CompatibleVirtualBone.RotateTo(ref leftRings, 1, Rotation(frame[RetargetBoneType.LeftRing2]) * localRotationDictionary[RetargetBoneType.LeftRing2]);
                CompatibleVirtualBone.RotateTo(ref leftRings, 2, Rotation(frame[RetargetBoneType.LeftRing3]) * localRotationDictionary[RetargetBoneType.LeftRing3]);
                CompatibleVirtualBone.RotateTo(ref leftRings, 3, Rotation(frame[RetargetBoneType.LeftRing4]) * localRotationDictionary[RetargetBoneType.LeftRing4]);

                CompatibleVirtualBone.RotateTo(ref leftPinkys, 0, Rotation(frame[RetargetBoneType.LeftPinky1]) * localRotationDictionary[RetargetBoneType.LeftPinky1]);
                CompatibleVirtualBone.RotateTo(ref leftPinkys, 1, Rotation(frame[RetargetBoneType.LeftPinky2]) * localRotationDictionary[RetargetBoneType.LeftPinky2]);
                CompatibleVirtualBone.RotateTo(ref leftPinkys, 2, Rotation(frame[RetargetBoneType.LeftPinky3]) * localRotationDictionary[RetargetBoneType.LeftPinky3]);
                CompatibleVirtualBone.RotateTo(ref leftPinkys, 3, Rotation(frame[RetargetBoneType.LeftPinky4]) * localRotationDictionary[RetargetBoneType.LeftPinky4]);

                CompatibleVirtualBone.RotateTo(ref rightThumbs, 0, Rotation(frame[RetargetBoneType.RightThumb1]) * localRotationDictionary[RetargetBoneType.RightThumb1]);
                CompatibleVirtualBone.RotateTo(ref rightThumbs, 1, Rotation(frame[RetargetBoneType.RightThumb2]) * localRotationDictionary[RetargetBoneType.RightThumb2]);
                CompatibleVirtualBone.RotateTo(ref rightThumbs, 2, Rotation(frame[RetargetBoneType.RightThumb3]) * localRotationDictionary[RetargetBoneType.RightThumb3]);
                CompatibleVirtualBone.RotateTo(ref rightThumbs, 3, Rotation(frame[RetargetBoneType.RightThumb4]) * localRotationDictionary[RetargetBoneType.RightThumb4]);

                CompatibleVirtualBone.RotateTo(ref rightIndexs, 0, Rotation(frame[RetargetBoneType.RightIndex1]) * localRotationDictionary[RetargetBoneType.RightIndex1]);
                CompatibleVirtualBone.RotateTo(ref rightIndexs, 1, Rotation(frame[RetargetBoneType.RightIndex2]) * localRotationDictionary[RetargetBoneType.RightIndex2]);
                CompatibleVirtualBone.RotateTo(ref rightIndexs, 2, Rotation(frame[RetargetBoneType.RightIndex3]) * localRotationDictionary[RetargetBoneType.RightIndex3]);
                CompatibleVirtualBone.RotateTo(ref rightIndexs, 3, Rotation(frame[RetargetBoneType.RightIndex4]) * localRotationDictionary[RetargetBoneType.RightIndex4]);

                CompatibleVirtualBone.RotateTo(ref rightMiddles, 0, Rotation(frame[RetargetBoneType.RightMiddle1]) * localRotationDictionary[RetargetBoneType.RightMiddle1]);
                CompatibleVirtualBone.RotateTo(ref rightMiddles, 1, Rotation(frame[RetargetBoneType.RightMiddle2]) * localRotationDictionary[RetargetBoneType.RightMiddle2]);
                CompatibleVirtualBone.RotateTo(ref rightMiddles, 2, Rotation(frame[RetargetBoneType.RightMiddle3]) * localRotationDictionary[RetargetBoneType.RightMiddle3]);
                CompatibleVirtualBone.RotateTo(ref rightMiddles, 3, Rotation(frame[RetargetBoneType.RightMiddle4]) * localRotationDictionary[RetargetBoneType.RightMiddle4]);

                CompatibleVirtualBone.RotateTo(ref rightRings, 0, Rotation(frame[RetargetBoneType.RightRing1]) * localRotationDictionary[RetargetBoneType.RightRing1]);
                CompatibleVirtualBone.RotateTo(ref rightRings, 1, Rotation(frame[RetargetBoneType.RightRing2]) * localRotationDictionary[RetargetBoneType.RightRing2]);
                CompatibleVirtualBone.RotateTo(ref rightRings, 2, Rotation(frame[RetargetBoneType.RightRing3]) * localRotationDictionary[RetargetBoneType.RightRing3]);
                CompatibleVirtualBone.RotateTo(ref rightRings, 3, Rotation(frame[RetargetBoneType.RightRing4]) * localRotationDictionary[RetargetBoneType.RightRing4]);

                CompatibleVirtualBone.RotateTo(ref rightPinkys, 0, Rotation(frame[RetargetBoneType.RightPinky1]) * localRotationDictionary[RetargetBoneType.RightPinky1]);
                CompatibleVirtualBone.RotateTo(ref rightPinkys, 1, Rotation(frame[RetargetBoneType.RightPinky2]) * localRotationDictionary[RetargetBoneType.RightPinky2]);
                CompatibleVirtualBone.RotateTo(ref rightPinkys, 2, Rotation(frame[RetargetBoneType.RightPinky3]) * localRotationDictionary[RetargetBoneType.RightPinky3]);
                CompatibleVirtualBone.RotateTo(ref rightPinkys, 3, Rotation(frame[RetargetBoneType.RightPinky4]) * localRotationDictionary[RetargetBoneType.RightPinky4]);
            }
        }

        private void RetargetDisplacement(Dictionary<RetargetBoneType, Joint_t> frame)
        {
            Vector3 srcHipPos = Position(frame[RetargetBoneType.Hip]);
            Vector3 targetHipPos = Vector3.zero;
            if (lastSrcFrame == null)
            {
                targetHipPos = new Vector3(srcHipPos.x, srcHipPos.y * highScale, srcHipPos.z);
            }
            else
            {
                Vector3 offset = srcHipPos - Position(lastSrcFrame[RetargetBoneType.Hip]);
                Vector3 tmp = lastTargetHipPosition + offset;
                float y = srcHipPos.y * highScale;
                targetHipPos = new Vector3(tmp.x, y, tmp.z);
            }
            MoveTo(targetHipPos);
            lastSrcFrame = frame;
            lastTargetHipPosition = targetHipPos;
        }

        private void MoveTo(Vector3 position)
        {
            Vector3 offset = position - spines[0].solverPosition;
            CompatibleVirtualBone.Move(ref spines, offset);
            CompatibleVirtualBone.Move(ref leftLeg, offset);
            CompatibleVirtualBone.Move(ref rightLeg, offset);
            CompatibleVirtualBone.Move(ref necks, offset);
            CompatibleVirtualBone.Move(ref leftArm, offset);
            CompatibleVirtualBone.Move(ref rightArm, offset);
            if (hasFinger)
            {
                CompatibleVirtualBone.Move(ref leftThumbs, offset);
                CompatibleVirtualBone.Move(ref leftIndexs, offset);
                CompatibleVirtualBone.Move(ref leftMiddles, offset);
                CompatibleVirtualBone.Move(ref leftRings, offset);
                CompatibleVirtualBone.Move(ref leftPinkys, offset);

                CompatibleVirtualBone.Move(ref rightThumbs, offset);
                CompatibleVirtualBone.Move(ref rightIndexs, offset);
                CompatibleVirtualBone.Move(ref rightMiddles, offset);
                CompatibleVirtualBone.Move(ref rightRings, offset);
                CompatibleVirtualBone.Move(ref rightPinkys, offset);
            }
        }

        private void TranslateHip(Vector3 newHipPos, Quaternion newHipRot)
        {
            Vector3 lastHipPos = spines[0].solverPosition;
            Quaternion lastHipRot = spines[0].solverRotation;
            Vector3 lastChestPos = spines[3].solverPosition;
            Quaternion lastChestRot = spines[3].solverRotation;
            CompatibleVirtualBone.TranslateRoot(spines, spines[0].solverPosition, spines[0].solverRotation, newHipPos, newHipRot);
            TranslateLegs(lastHipPos, lastHipRot, newHipPos, newHipRot);
            TranslateArms(lastChestPos, lastChestRot, spines[3].solverPosition, spines[3].solverRotation);
            TranslateNecks(lastChestPos, lastChestRot, spines[3].solverPosition, spines[3].solverRotation);
        }

        private void TranslateLegs(Vector3 lastHipPos, Quaternion lastHipRot, Vector3 newHipPos, Quaternion newHipRot)
        {
            CompatibleVirtualBone.TranslateRoot(leftLeg, lastHipPos, lastHipRot, newHipPos, newHipRot);
            CompatibleVirtualBone.TranslateRoot(rightLeg, lastHipPos, lastHipRot, newHipPos, newHipRot);
        }

        private void TranslateArms(Vector3 lastChestPos, Quaternion lastChestRot, Vector3 newChestPos, Quaternion newChestRot)
        {
            CompatibleVirtualBone.TranslateRoot(leftArm, lastChestPos, lastChestRot, newChestPos, newChestRot);
            CompatibleVirtualBone.TranslateRoot(rightArm, lastChestPos, lastChestRot, newChestPos, newChestRot);
        }

        private void TranslateNecks(Vector3 lastChestPos, Quaternion lastChestRot, Vector3 newChestPos, Quaternion newChestRot)
        {
            CompatibleVirtualBone.TranslateRoot(necks, lastChestPos, lastChestRot, newChestPos, newChestRot);
        }

        private void ReadAttributes(Dictionary<RetargetBoneType, Joint_t> tar)
        {
            if (!initialized)
            {
                rootPose = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Root]), Rotation(tar[RetargetBoneType.Root]));
                spinesPose = new CompatibleVirtualBone[4];
                leftLegPose = new CompatibleVirtualBone[4];
                rightLegPose = new CompatibleVirtualBone[4];
                necksPose = new CompatibleVirtualBone[2];
                leftArmPose = new CompatibleVirtualBone[4];
                rightArmPose = new CompatibleVirtualBone[4];

                leftThumbsPose = new CompatibleVirtualBone[4];
                leftIndexsPose = new CompatibleVirtualBone[4];
                leftMiddlesPose = new CompatibleVirtualBone[4];
                leftRingsPose = new CompatibleVirtualBone[4];
                leftPinkysPose = new CompatibleVirtualBone[4];

                rightThumbsPose = new CompatibleVirtualBone[4];
                rightIndexsPose = new CompatibleVirtualBone[4];
                rightMiddlesPose = new CompatibleVirtualBone[4];
                rightRingsPose = new CompatibleVirtualBone[4];
                rightPinkysPose = new CompatibleVirtualBone[4];
                spinesPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Hip]), Rotation(tar[RetargetBoneType.Hip]));
                spinesPose[1]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.Spine]), Rotation(tar[RetargetBoneType.Spine]));
                spinesPose[2]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.Spine1]), Rotation(tar[RetargetBoneType.Spine1]));
                spinesPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Chest]), Rotation(tar[RetargetBoneType.Chest]));
                leftLegPose[0]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThigh]), Rotation(tar[RetargetBoneType.LeftThigh]));
                leftLegPose[1]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftCalf]), Rotation(tar[RetargetBoneType.LeftCalf]));
                leftLegPose[2]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftFoot]), Rotation(tar[RetargetBoneType.LeftFoot]));
                leftLegPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftToe]), Rotation(tar[RetargetBoneType.LeftToe]));
                rightLegPose[0]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThigh]), Rotation(tar[RetargetBoneType.RightThigh]));
                rightLegPose[1]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightCalf]), Rotation(tar[RetargetBoneType.RightCalf]));
                rightLegPose[2]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightFoot]), Rotation(tar[RetargetBoneType.RightFoot]));
                rightLegPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightToe]), Rotation(tar[RetargetBoneType.RightToe]));
                necksPose[0]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.Neck]), Rotation(tar[RetargetBoneType.Neck]));
                necksPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Head]), Rotation(tar[RetargetBoneType.Head]));
                leftArmPose[0]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftShoulder]), Rotation(tar[RetargetBoneType.LeftShoulder]));
                leftArmPose[1]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftUpperarm]), Rotation(tar[RetargetBoneType.LeftUpperarm]));
                leftArmPose[2]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftForearm]), Rotation(tar[RetargetBoneType.LeftForearm]));
                leftArmPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftHand]), Rotation(tar[RetargetBoneType.LeftHand]));
                rightArmPose[0]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightShoulder]), Rotation(tar[RetargetBoneType.RightShoulder]));
                rightArmPose[1]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightUpperarm]), Rotation(tar[RetargetBoneType.RightUpperarm]));
                rightArmPose[2]= new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightForearm]), Rotation(tar[RetargetBoneType.RightForearm]));
                rightArmPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightHand]), Rotation(tar[RetargetBoneType.RightHand]));
                if (hasFinger)
                {
                    leftThumbsPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb1]), Rotation(tar[RetargetBoneType.LeftThumb1]));
                    leftThumbsPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb2]), Rotation(tar[RetargetBoneType.LeftThumb2]));
                    leftThumbsPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb3]), Rotation(tar[RetargetBoneType.LeftThumb3]));
                    leftThumbsPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb4]), Rotation(tar[RetargetBoneType.LeftThumb4]));

                    leftIndexsPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex1]), Rotation(tar[RetargetBoneType.LeftIndex1]));
                    leftIndexsPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex2]), Rotation(tar[RetargetBoneType.LeftIndex2]));
                    leftIndexsPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex3]), Rotation(tar[RetargetBoneType.LeftIndex3]));
                    leftIndexsPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex4]), Rotation(tar[RetargetBoneType.LeftIndex4]));

                    leftMiddlesPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle1]), Rotation(tar[RetargetBoneType.LeftMiddle1]));
                    leftMiddlesPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle2]), Rotation(tar[RetargetBoneType.LeftMiddle2]));
                    leftMiddlesPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle3]), Rotation(tar[RetargetBoneType.LeftMiddle3]));
                    leftMiddlesPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle4]), Rotation(tar[RetargetBoneType.LeftMiddle4]));

                    leftRingsPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing1]), Rotation(tar[RetargetBoneType.LeftRing1]));
                    leftRingsPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing2]), Rotation(tar[RetargetBoneType.LeftRing2]));
                    leftRingsPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing3]), Rotation(tar[RetargetBoneType.LeftRing3]));
                    leftRingsPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing4]), Rotation(tar[RetargetBoneType.LeftRing4]));

                    leftPinkysPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky1]), Rotation(tar[RetargetBoneType.LeftPinky1]));
                    leftPinkysPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky2]), Rotation(tar[RetargetBoneType.LeftPinky2]));
                    leftPinkysPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky3]), Rotation(tar[RetargetBoneType.LeftPinky3]));
                    leftPinkysPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky4]), Rotation(tar[RetargetBoneType.LeftPinky4]));

                    rightThumbsPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb1]), Rotation(tar[RetargetBoneType.RightThumb1]));
                    rightThumbsPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb2]), Rotation(tar[RetargetBoneType.RightThumb2]));
                    rightThumbsPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb3]), Rotation(tar[RetargetBoneType.RightThumb3]));
                    rightThumbsPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb4]), Rotation(tar[RetargetBoneType.RightThumb4]));

                    rightIndexsPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex1]), Rotation(tar[RetargetBoneType.RightIndex1]));
                    rightIndexsPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex2]), Rotation(tar[RetargetBoneType.RightIndex2]));
                    rightIndexsPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex3]), Rotation(tar[RetargetBoneType.RightIndex3]));
                    rightIndexsPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex4]), Rotation(tar[RetargetBoneType.RightIndex4]));

                    rightMiddlesPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle1]), Rotation(tar[RetargetBoneType.RightMiddle1]));
                    rightMiddlesPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle2]), Rotation(tar[RetargetBoneType.RightMiddle2]));
                    rightMiddlesPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle3]), Rotation(tar[RetargetBoneType.RightMiddle3]));
                    rightMiddlesPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle4]), Rotation(tar[RetargetBoneType.RightMiddle4]));

                    rightRingsPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing1]), Rotation(tar[RetargetBoneType.RightRing1]));
                    rightRingsPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing2]), Rotation(tar[RetargetBoneType.RightRing2]));
                    rightRingsPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing3]), Rotation(tar[RetargetBoneType.RightRing3]));
                    rightRingsPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing4]), Rotation(tar[RetargetBoneType.RightRing4]));

                    rightPinkysPose[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky1]), Rotation(tar[RetargetBoneType.RightPinky1]));
                    rightPinkysPose[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky2]), Rotation(tar[RetargetBoneType.RightPinky2]));
                    rightPinkysPose[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky3]), Rotation(tar[RetargetBoneType.RightPinky3]));
                    rightPinkysPose[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky4]), Rotation(tar[RetargetBoneType.RightPinky4]));
                }

                root = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Root]), Rotation(tar[RetargetBoneType.Root]));
                spines = new CompatibleVirtualBone[4];
                leftLeg = new CompatibleVirtualBone[4];
                rightLeg = new CompatibleVirtualBone[4];
                necks = new CompatibleVirtualBone[2];
                leftArm = new CompatibleVirtualBone[4];
                rightArm = new CompatibleVirtualBone[4];

                leftThumbs = new CompatibleVirtualBone[4];
                leftIndexs = new CompatibleVirtualBone[4];
                leftMiddles = new CompatibleVirtualBone[4];
                leftRings = new CompatibleVirtualBone[4];
                leftPinkys = new CompatibleVirtualBone[4];

                rightThumbs = new CompatibleVirtualBone[4];
                rightIndexs = new CompatibleVirtualBone[4];
                rightMiddles = new CompatibleVirtualBone[4];
                rightRings = new CompatibleVirtualBone[4];
                rightPinkys = new CompatibleVirtualBone[4];
                spines[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Hip]), Rotation(tar[RetargetBoneType.Hip]));
                spines[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Spine]), Rotation(tar[RetargetBoneType.Spine]));
                spines[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Spine1]), Rotation(tar[RetargetBoneType.Spine1]));
                spines[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Chest]), Rotation(tar[RetargetBoneType.Chest]));
                leftLeg[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThigh]), Rotation(tar[RetargetBoneType.LeftThigh]));
                leftLeg[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftCalf]), Rotation(tar[RetargetBoneType.LeftCalf]));
                leftLeg[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftFoot]), Rotation(tar[RetargetBoneType.LeftFoot]));
                leftLeg[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftToe]), Rotation(tar[RetargetBoneType.LeftToe]));
                rightLeg[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThigh]), Rotation(tar[RetargetBoneType.RightThigh]));
                rightLeg[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightCalf]), Rotation(tar[RetargetBoneType.RightCalf]));
                rightLeg[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightFoot]), Rotation(tar[RetargetBoneType.RightFoot]));
                rightLeg[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightToe]), Rotation(tar[RetargetBoneType.RightToe]));
                necks[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Neck]), Rotation(tar[RetargetBoneType.Neck]));
                necks[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.Head]), Rotation(tar[RetargetBoneType.Head]));
                leftArm[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftShoulder]), Rotation(tar[RetargetBoneType.LeftShoulder]));
                leftArm[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftUpperarm]), Rotation(tar[RetargetBoneType.LeftUpperarm]));
                leftArm[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftForearm]), Rotation(tar[RetargetBoneType.LeftForearm]));
                leftArm[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftHand]), Rotation(tar[RetargetBoneType.LeftHand]));
                rightArm[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightShoulder]), Rotation(tar[RetargetBoneType.RightShoulder]));
                rightArm[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightUpperarm]), Rotation(tar[RetargetBoneType.RightUpperarm]));
                rightArm[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightForearm]), Rotation(tar[RetargetBoneType.RightForearm]));
                rightArm[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightHand]), Rotation(tar[RetargetBoneType.RightHand]));
                if (hasFinger)
                {
                    leftThumbs[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb1]), Rotation(tar[RetargetBoneType.LeftThumb1]));
                    leftThumbs[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb2]), Rotation(tar[RetargetBoneType.LeftThumb2]));
                    leftThumbs[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb3]), Rotation(tar[RetargetBoneType.LeftThumb3]));
                    leftThumbs[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftThumb4]), Rotation(tar[RetargetBoneType.LeftThumb4]));

                    leftIndexs[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex1]), Rotation(tar[RetargetBoneType.LeftIndex1]));
                    leftIndexs[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex2]), Rotation(tar[RetargetBoneType.LeftIndex2]));
                    leftIndexs[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex3]), Rotation(tar[RetargetBoneType.LeftIndex3]));
                    leftIndexs[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftIndex4]), Rotation(tar[RetargetBoneType.LeftIndex4]));

                    leftMiddles[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle1]), Rotation(tar[RetargetBoneType.LeftMiddle1]));
                    leftMiddles[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle2]), Rotation(tar[RetargetBoneType.LeftMiddle2]));
                    leftMiddles[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle3]), Rotation(tar[RetargetBoneType.LeftMiddle3]));
                    leftMiddles[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftMiddle4]), Rotation(tar[RetargetBoneType.LeftMiddle4]));

                    leftRings[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing1]), Rotation(tar[RetargetBoneType.LeftRing1]));
                    leftRings[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing2]), Rotation(tar[RetargetBoneType.LeftRing2]));
                    leftRings[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing3]), Rotation(tar[RetargetBoneType.LeftRing3]));
                    leftRings[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftRing4]), Rotation(tar[RetargetBoneType.LeftRing4]));

                    leftPinkys[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky1]), Rotation(tar[RetargetBoneType.LeftPinky1]));
                    leftPinkys[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky2]), Rotation(tar[RetargetBoneType.LeftPinky2]));
                    leftPinkys[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky3]), Rotation(tar[RetargetBoneType.LeftPinky3]));
                    leftPinkys[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.LeftPinky4]), Rotation(tar[RetargetBoneType.LeftPinky4]));

                    rightThumbs[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb1]), Rotation(tar[RetargetBoneType.RightThumb1]));
                    rightThumbs[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb2]), Rotation(tar[RetargetBoneType.RightThumb2]));
                    rightThumbs[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb3]), Rotation(tar[RetargetBoneType.RightThumb3]));
                    rightThumbs[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightThumb4]), Rotation(tar[RetargetBoneType.RightThumb4]));

                    rightIndexs[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex1]), Rotation(tar[RetargetBoneType.RightIndex1]));
                    rightIndexs[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex2]), Rotation(tar[RetargetBoneType.RightIndex2]));
                    rightIndexs[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex3]), Rotation(tar[RetargetBoneType.RightIndex3]));
                    rightIndexs[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightIndex4]), Rotation(tar[RetargetBoneType.RightIndex4]));

                    rightMiddles[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle1]), Rotation(tar[RetargetBoneType.RightMiddle1]));
                    rightMiddles[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle2]), Rotation(tar[RetargetBoneType.RightMiddle2]));
                    rightMiddles[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle3]), Rotation(tar[RetargetBoneType.RightMiddle3]));
                    rightMiddles[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightMiddle4]), Rotation(tar[RetargetBoneType.RightMiddle4]));

                    rightRings[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing1]), Rotation(tar[RetargetBoneType.RightRing1]));
                    rightRings[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing2]), Rotation(tar[RetargetBoneType.RightRing2]));
                    rightRings[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing3]), Rotation(tar[RetargetBoneType.RightRing3]));
                    rightRings[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightRing4]), Rotation(tar[RetargetBoneType.RightRing4]));

                    rightPinkys[0] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky1]), Rotation(tar[RetargetBoneType.RightPinky1]));
                    rightPinkys[1] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky2]), Rotation(tar[RetargetBoneType.RightPinky2]));
                    rightPinkys[2] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky3]), Rotation(tar[RetargetBoneType.RightPinky3]));
                    rightPinkys[3] = new CompatibleVirtualBone(Position(tar[RetargetBoneType.RightPinky4]), Rotation(tar[RetargetBoneType.RightPinky4]));
                }
            }

            ReadBeforeSolve();
        }

        private void ReadBeforeSolve()
        {
            root.Read(rootPose.solverPosition, rootPose.solverRotation);
            spines[0].Read(spinesPose[0].solverPosition, spinesPose[0].solverRotation);
            spines[1].Read(spinesPose[1].solverPosition, spinesPose[1].solverRotation);
            spines[2].Read(spinesPose[2].solverPosition, spinesPose[2].solverRotation);
            spines[3].Read(spinesPose[3].solverPosition, spinesPose[3].solverRotation);
            leftLeg[0].Read(leftLegPose[0].solverPosition, leftLegPose[0].solverRotation);
            leftLeg[1].Read(leftLegPose[1].solverPosition, leftLegPose[1].solverRotation);
            leftLeg[2].Read(leftLegPose[2].solverPosition, leftLegPose[2].solverRotation);
            leftLeg[3].Read(leftLegPose[3].solverPosition, leftLegPose[3].solverRotation);
            rightLeg[0].Read(rightLegPose[0].solverPosition, rightLegPose[0].solverRotation);
            rightLeg[1].Read(rightLegPose[1].solverPosition, rightLegPose[1].solverRotation);
            rightLeg[2].Read(rightLegPose[2].solverPosition, rightLegPose[2].solverRotation);
            rightLeg[3].Read(rightLegPose[3].solverPosition, rightLegPose[3].solverRotation);
            necks[0].Read(necksPose[0].solverPosition, necksPose[0].solverRotation);
            necks[1].Read(necksPose[1].solverPosition, necksPose[1].solverRotation);
            leftArm[0].Read(leftArmPose[0].solverPosition, leftArmPose[0].solverRotation);
            leftArm[1].Read(leftArmPose[1].solverPosition, leftArmPose[1].solverRotation);
            leftArm[2].Read(leftArmPose[2].solverPosition, leftArmPose[2].solverRotation);
            leftArm[3].Read(leftArmPose[3].solverPosition, leftArmPose[3].solverRotation);
            rightArm[0].Read(rightArmPose[0].solverPosition, rightArmPose[0].solverRotation);
            rightArm[1].Read(rightArmPose[1].solverPosition, rightArmPose[1].solverRotation);
            rightArm[2].Read(rightArmPose[2].solverPosition, rightArmPose[2].solverRotation);
            rightArm[3].Read(rightArmPose[3].solverPosition, rightArmPose[3].solverRotation);

            if (hasFinger)
            {
                leftThumbs[0].Read(leftThumbsPose[0].solverPosition, leftThumbsPose[0].solverRotation);
                leftThumbs[1].Read(leftThumbsPose[1].solverPosition, leftThumbsPose[1].solverRotation);
                leftThumbs[2].Read(leftThumbsPose[2].solverPosition, leftThumbsPose[2].solverRotation);
                leftThumbs[3].Read(leftThumbsPose[3].solverPosition, leftThumbsPose[3].solverRotation);

                leftIndexs[0].Read(leftIndexsPose[0].solverPosition, leftIndexsPose[0].solverRotation);
                leftIndexs[1].Read(leftIndexsPose[1].solverPosition, leftIndexsPose[1].solverRotation);
                leftIndexs[2].Read(leftIndexsPose[2].solverPosition, leftIndexsPose[2].solverRotation);
                leftIndexs[3].Read(leftIndexsPose[3].solverPosition, leftIndexsPose[3].solverRotation);

                leftMiddles[0].Read(leftMiddlesPose[0].solverPosition, leftMiddlesPose[0].solverRotation);
                leftMiddles[1].Read(leftMiddlesPose[1].solverPosition, leftMiddlesPose[1].solverRotation);
                leftMiddles[2].Read(leftMiddlesPose[2].solverPosition, leftMiddlesPose[2].solverRotation);
                leftMiddles[3].Read(leftMiddlesPose[3].solverPosition, leftMiddlesPose[3].solverRotation);

                leftRings[0].Read(leftRingsPose[0].solverPosition, leftRingsPose[0].solverRotation);
                leftRings[1].Read(leftRingsPose[1].solverPosition, leftRingsPose[1].solverRotation);
                leftRings[2].Read(leftRingsPose[2].solverPosition, leftRingsPose[2].solverRotation);
                leftRings[3].Read(leftRingsPose[3].solverPosition, leftRingsPose[3].solverRotation);

                leftPinkys[0].Read(leftPinkysPose[0].solverPosition, leftPinkysPose[0].solverRotation);
                leftPinkys[1].Read(leftPinkysPose[1].solverPosition, leftPinkysPose[1].solverRotation);
                leftPinkys[2].Read(leftPinkysPose[2].solverPosition, leftPinkysPose[2].solverRotation);
                leftPinkys[3].Read(leftPinkysPose[3].solverPosition, leftPinkysPose[3].solverRotation);
                //
                rightThumbs[0].Read(rightThumbsPose[0].solverPosition, rightThumbsPose[0].solverRotation);
                rightThumbs[1].Read(rightThumbsPose[1].solverPosition, rightThumbsPose[1].solverRotation);
                rightThumbs[2].Read(rightThumbsPose[2].solverPosition, rightThumbsPose[2].solverRotation);
                rightThumbs[3].Read(rightThumbsPose[3].solverPosition, rightThumbsPose[3].solverRotation);

                rightIndexs[0].Read(rightIndexsPose[0].solverPosition, rightIndexsPose[0].solverRotation);
                rightIndexs[1].Read(rightIndexsPose[1].solverPosition, rightIndexsPose[1].solverRotation);
                rightIndexs[2].Read(rightIndexsPose[2].solverPosition, rightIndexsPose[2].solverRotation);
                rightIndexs[3].Read(rightIndexsPose[3].solverPosition, rightIndexsPose[3].solverRotation);

                rightMiddles[0].Read(rightMiddlesPose[0].solverPosition, rightMiddlesPose[0].solverRotation);
                rightMiddles[1].Read(rightMiddlesPose[1].solverPosition, rightMiddlesPose[1].solverRotation);
                rightMiddles[2].Read(rightMiddlesPose[2].solverPosition, rightMiddlesPose[2].solverRotation);
                rightMiddles[3].Read(rightMiddlesPose[3].solverPosition, rightMiddlesPose[3].solverRotation);

                rightRings[0].Read(rightRingsPose[0].solverPosition, rightRingsPose[0].solverRotation);
                rightRings[1].Read(rightRingsPose[1].solverPosition, rightRingsPose[1].solverRotation);
                rightRings[2].Read(rightRingsPose[2].solverPosition, rightRingsPose[2].solverRotation);
                rightRings[3].Read(rightRingsPose[3].solverPosition, rightRingsPose[3].solverRotation);

                rightPinkys[0].Read(rightPinkysPose[0].solverPosition, rightPinkysPose[0].solverRotation);
                rightPinkys[1].Read(rightPinkysPose[1].solverPosition, rightPinkysPose[1].solverRotation);
                rightPinkys[2].Read(rightPinkysPose[2].solverPosition, rightPinkysPose[2].solverRotation);
                rightPinkys[3].Read(rightPinkysPose[3].solverPosition, rightPinkysPose[3].solverRotation);
            }
        }

        private void RecordLocalRotations(Dictionary<RetargetBoneType, Joint_t> srcTPose, Dictionary<RetargetBoneType, Joint_t> tarTPose)
        {
            foreach (var key in bodyTypes)
            {
                localRotationDictionary.Add(key, Quaternion.Inverse(Rotation(srcTPose[key])) * Rotation(tarTPose[key]));
            }

            if (hasRotabone)
            {
                repositionLeftArmRots[0] = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.LeftForearm]));
                repositionLeftArmRots[1] = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.LeftRotaBone]));
                repositionLeftArmRots[2] = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.LeftHand]));
                repositionRightArmRots[0] = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.RightForearm]));
                repositionRightArmRots[1] = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.RightRotaBone]));
                repositionRightArmRots[2] = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.RightHand]));
                leftRotaboneRelativeToForearm = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.LeftForearm])) * Position(tarTPose[RetargetBoneType.LeftRotaBone]) -
                    Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.LeftForearm])) * Position(tarTPose[RetargetBoneType.LeftForearm]);
                rightRotaboneRelativeToForearm = Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.RightForearm])) * Position(tarTPose[RetargetBoneType.RightRotaBone]) -
                    Quaternion.Inverse(Rotation(tarTPose[RetargetBoneType.RightForearm])) * Position(tarTPose[RetargetBoneType.RightForearm]);

            }

            if (hasFinger)
            {
                foreach (var key in fingerTypes)
                {
                    localRotationDictionary.Add(key, Quaternion.Inverse(Rotation(srcTPose[key])) * Rotation(tarTPose[key]));
                }
            }
        }

        private bool Check(Dictionary<RetargetBoneType, Joint_t> pose, ref bool hasFingers, ref bool hasRotabones)
        {
            if (pose == null || pose.Count <= 0)
            {
                hasFinger = false;
                hasRotabones = false;
                return false;
            }

            foreach (var key in bodyTypes)
            {
                if (!pose.ContainsKey(key))
                {
                    hasFingers = false;
                    hasRotabones = false;
                    return false;
                }
            }

            hasFingers = true;
            foreach (var key in fingerTypes)
            {
                if (!pose.ContainsKey(key)) hasFingers = false;
            }

            hasRotabones = true;
            foreach (var key in rotaboneTypes)
            {
                if (!pose.ContainsKey(key)) hasRotabones = false;
            }
            return true;
        }

        public static Vector3 Position(Joint_t j)
        {
            return new Vector3(j.px, j.py, j.pz);
        }

        public static Quaternion Rotation(Joint_t j)
        {
            return new Quaternion(j.qx, j.qy, j.qz, j.qw);
        }

        public void Get(CompatibleVirtualBone[] bones, int begin, ref Vector3[] positions, ref Quaternion[] rotations)
        {
            positions = new Vector3[3];
            rotations = new Quaternion[3];

            for (int i = 0; i < 3; ++i)
            {
                int index = i + begin;
                positions[i] = bones[index].solverPosition;
                rotations[i] = bones[index].solverRotation;
            }
        }

        public Vector3[] CalNormlizedHandTarget(Dictionary<RetargetBoneType, Joint_t> frame)
        {
            Vector3 tarLeftHandForearmDir = leftArm[3].solverPosition - leftArm[2].solverPosition;
            Vector3 tarLeftForearmUpperarmDir = leftArm[2].solverPosition - leftArm[1].solverPosition;
            Vector3 tarLeftUpperarmShoulderDir = leftArm[1].solverPosition - leftArm[0].solverPosition;
            Vector3 tarLeftShoulderChestDir = leftArm[0].solverPosition - spines[3].solverPosition;
            Vector3 tarRightHandForearmDir = rightArm[3].solverPosition - rightArm[2].solverPosition;
            Vector3 tarRightForearmUpperarmDir = rightArm[2].solverPosition - rightArm[1].solverPosition;
            Vector3 tarRightUpperarmShoulderDir = rightArm[1].solverPosition - rightArm[0].solverPosition;
            Vector3 tarRightShoulderChestDir = rightArm[0].solverPosition - spines[3].solverPosition;
            Vector3 tarSpineHipDir = spines[1].solverPosition - spines[0].solverPosition;
            Vector3 tarSpine1SpineDir = spines[2].solverPosition - spines[1].solverPosition;
            Vector3 tarChestSpine1Dir = spines[3].solverPosition - spines[2].solverPosition;
            Vector3 tarNeckHeadDir = necks[0].solverPosition - necks[1].solverPosition;
            Vector3 tarChestNeckDir = spines[3].solverPosition - necks[0].solverPosition;

            Vector3 srcLeftHandForearmDir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.LeftForearm]);
            Vector3 srcLeftForearmUpperarmDir = Position(frame[RetargetBoneType.LeftForearm]) - Position(frame[RetargetBoneType.LeftUpperarm]);
            Vector3 srcLeftUpperarmShoulderDir = Position(frame[RetargetBoneType.LeftUpperarm]) - Position(frame[RetargetBoneType.LeftShoulder]);
            Vector3 srcLeftShoulderChestDir = Position(frame[RetargetBoneType.LeftShoulder]) - Position(frame[RetargetBoneType.Chest]);
            Vector3 srcRightHandForearmDir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.RightForearm]);
            Vector3 srcRightForearmUpperarmDir = Position(frame[RetargetBoneType.RightForearm]) - Position(frame[RetargetBoneType.RightUpperarm]);
            Vector3 srcRightUpperarmShoulderDir = Position(frame[RetargetBoneType.RightUpperarm]) - Position(frame[RetargetBoneType.RightShoulder]);
            Vector3 srcRightShoulderChestDir = Position(frame[RetargetBoneType.RightShoulder]) - Position(frame[RetargetBoneType.Chest]);
            Vector3 srcSpineHipDir = Position(frame[RetargetBoneType.Spine]) - Position(frame[RetargetBoneType.Hip]);
            Vector3 srcSpine1SpineDir = Position(frame[RetargetBoneType.Spine1]) - Position(frame[RetargetBoneType.Spine]);
            Vector3 srcChestSpine1Dir = Position(frame[RetargetBoneType.Chest]) - Position(frame[RetargetBoneType.Spine1]);
            Vector3 srcNeckHeadDir = Position(frame[RetargetBoneType.Neck]) - Position(frame[RetargetBoneType.Head]);
            Vector3 srcChestNeckDir = Position(frame[RetargetBoneType.Chest]) - Position(frame[RetargetBoneType.Neck]);

            Vector3 targetPoint = Position(frame[RetargetBoneType.LeftHand]);
            Vector3 basePoint = Position(frame[RetargetBoneType.Chest]);
            Vector3 dir = targetPoint - basePoint;
            float weightLeftHandChest = 1 / dir.sqrMagnitude;
            float t = srcLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                srcLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                srcLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                srcLeftHandForearmDir.magnitude * AbsCosWeight(srcLeftHandForearmDir, dir);
            Vector3 egocentricLeftHandChestDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.LeftHand]);
            basePoint = Position(frame[RetargetBoneType.Neck]);
            dir = targetPoint - basePoint;
            float weightLeftHandNeck = 1 / dir.sqrMagnitude;
            t = srcLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                srcLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                srcLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                srcLeftHandForearmDir.magnitude * AbsCosWeight(srcLeftHandForearmDir, dir) +
                srcChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir);
            Vector3 egocentricLeftHandNeckDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.LeftHand]);
            basePoint = Position(frame[RetargetBoneType.Head]);
            dir = targetPoint - basePoint;
            float weightLeftHandHead = 1 / dir.sqrMagnitude;
            t = srcLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                srcLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                srcLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                srcLeftHandForearmDir.magnitude * AbsCosWeight(srcLeftHandForearmDir, dir) +
                srcChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir) +
                srcNeckHeadDir.magnitude * AbsCosWeight(srcNeckHeadDir, dir);
            Vector3 egocentricLeftHandHeadDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.LeftHand]);
            basePoint = Position(frame[RetargetBoneType.Spine1]);
            dir = targetPoint - basePoint;
            float weightLeftHandSpine1 = 1 / dir.sqrMagnitude;
            t = srcLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                srcLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                srcLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                srcLeftHandForearmDir.magnitude * AbsCosWeight(srcLeftHandForearmDir, dir) +
                srcChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir);
            Vector3 egocentricLeftHandSpine1Dir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.LeftHand]);
            basePoint = Position(frame[RetargetBoneType.Spine]);
            dir = targetPoint - basePoint;
            float weightLeftHandSpine = 1 / dir.sqrMagnitude;
            t = srcLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                srcLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                srcLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                srcLeftHandForearmDir.magnitude * AbsCosWeight(srcLeftHandForearmDir, dir) +
                srcChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                srcSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir);
            Vector3 egocentricLeftHandSpineDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.LeftHand]);
            basePoint = Position(frame[RetargetBoneType.Hip]);
            dir = targetPoint - basePoint;
            float weightLeftHandHip = 1 / dir.sqrMagnitude;
            t = srcLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                srcLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                srcLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                srcLeftHandForearmDir.magnitude * AbsCosWeight(srcLeftHandForearmDir, dir) +
                srcChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                srcSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir) +
                srcSpineHipDir.magnitude * AbsCosWeight(srcSpineHipDir, dir);
            Vector3 egocentricLeftHandHipDir = dir / t;

            float sum = weightLeftHandChest + weightLeftHandHead  + weightLeftHandHip;
            weightLeftHandChest /= sum;
            weightLeftHandHead /= sum;
            weightLeftHandHip /= sum;
            //
            targetPoint = Position(frame[RetargetBoneType.RightHand]);
            basePoint = Position(frame[RetargetBoneType.Chest]);
            dir = targetPoint - basePoint;
            float weightRightHandChest = 1 / dir.sqrMagnitude;
            t = srcRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                srcRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                srcRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                srcRightHandForearmDir.magnitude * AbsCosWeight(srcRightHandForearmDir, dir);
            Vector3 egocentricRightHandChestDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.RightHand]);
            basePoint = Position(frame[RetargetBoneType.Neck]);
            dir = targetPoint - basePoint;
            float weightRightHandNeck = 1 / dir.sqrMagnitude;
            t = srcRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                srcRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                srcRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                srcRightHandForearmDir.magnitude * AbsCosWeight(srcRightHandForearmDir, dir) +
                srcChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir);
            Vector3 egocentricRightHandNeckDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.RightHand]);
            basePoint = Position(frame[RetargetBoneType.Head]);
            dir = targetPoint - basePoint;
            float weightRightHandHead = 1 / dir.sqrMagnitude;
            t = srcRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                srcRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                srcRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                srcRightHandForearmDir.magnitude * AbsCosWeight(srcRightHandForearmDir, dir) +
                srcChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir) +
                srcNeckHeadDir.magnitude * AbsCosWeight(srcNeckHeadDir, dir);
            Vector3 egocentricRightHandHeadDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.RightHand]);
            basePoint = Position(frame[RetargetBoneType.Spine1]);
            dir = targetPoint - basePoint;
            float weightRightHandSpine1 = 1 / dir.sqrMagnitude;
            t = srcRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                srcRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                srcRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                srcRightHandForearmDir.magnitude * AbsCosWeight(srcRightHandForearmDir, dir) +
                srcChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir);
            Vector3 egocentricRightHandSpine1Dir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.RightHand]);
            basePoint = Position(frame[RetargetBoneType.Spine]);
            dir = targetPoint - basePoint;
            float weightRightHandSpine = 1 / dir.sqrMagnitude;
            t = srcRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                srcRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                srcRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                srcRightHandForearmDir.magnitude * AbsCosWeight(srcRightHandForearmDir, dir) +
                srcChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                srcSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir);
            Vector3 egocentricRightHandSpineDir = dir / t;

            targetPoint = Position(frame[RetargetBoneType.RightHand]);
            basePoint = Position(frame[RetargetBoneType.Hip]);
            dir = targetPoint - basePoint;
            float weightRightHandHip = 1 / dir.sqrMagnitude;
            t = srcRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                srcRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                srcRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                srcRightHandForearmDir.magnitude * AbsCosWeight(srcRightHandForearmDir, dir) +
                srcChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                srcSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir) +
                srcSpineHipDir.magnitude * AbsCosWeight(srcSpineHipDir, dir);
            Vector3 egocentricRightHandHipDir = dir / t;

            sum = weightRightHandChest + weightRightHandHead  + weightRightHandHip;
            weightRightHandChest /= sum;
            weightRightHandHead /= sum;
            weightRightHandHip /= sum;
            /////////////////////////////////////////////////
            dir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.Chest]);
            float tatTChest = tarLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                tarLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                tarLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                tarLeftHandForearmDir.magnitude * AbsCosWeight(tarLeftHandForearmDir, dir);

            dir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.Neck]);
            float tatTNeck = tarLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                tarLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                tarLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                tarLeftHandForearmDir.magnitude * AbsCosWeight(tarLeftHandForearmDir, dir) +
                tarChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir);

            dir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.Head]);
            float tatTHead = tarLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                tarLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                tarLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                tarLeftHandForearmDir.magnitude * AbsCosWeight(tarLeftHandForearmDir, dir) +
                tarChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir) +
                tarNeckHeadDir.magnitude * AbsCosWeight(srcNeckHeadDir, dir);

            dir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.Spine1]);
            float tatTSpine1 = tarLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                tarLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                tarLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                tarLeftHandForearmDir.magnitude * AbsCosWeight(tarLeftHandForearmDir, dir) +
                tarChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir);

            dir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.Spine]);
            float tatTSpine = tarLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                tarLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                tarLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                tarLeftHandForearmDir.magnitude * AbsCosWeight(tarLeftHandForearmDir, dir) +
                tarChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                tarSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir);

            dir = Position(frame[RetargetBoneType.LeftHand]) - Position(frame[RetargetBoneType.Hip]);
            float tatTHip = tarLeftShoulderChestDir.magnitude * AbsCosWeight(srcLeftShoulderChestDir, dir) +
                tarLeftUpperarmShoulderDir.magnitude * AbsCosWeight(srcLeftUpperarmShoulderDir, dir) +
                tarLeftForearmUpperarmDir.magnitude * AbsCosWeight(srcLeftForearmUpperarmDir, dir) +
                tarLeftHandForearmDir.magnitude * AbsCosWeight(tarLeftHandForearmDir, dir) +
                tarChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                tarSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir) +
                tarSpineHipDir.magnitude * AbsCosWeight(srcSpineHipDir, dir);
            //
            dir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.Chest]);
            float tatTRChest = tarRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                tarRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                tarRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                tarRightHandForearmDir.magnitude * AbsCosWeight(tarRightHandForearmDir, dir);

            dir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.Neck]);
            float tatTRNeck = tarRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                tarRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                tarRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                tarRightHandForearmDir.magnitude * AbsCosWeight(tarRightHandForearmDir, dir) +
                tarChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir);

            dir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.Head]);
            float tatTRHead = tarRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                tarRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                tarRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                tarRightHandForearmDir.magnitude * AbsCosWeight(tarRightHandForearmDir, dir) +
                tarChestNeckDir.magnitude * AbsCosWeight(srcChestNeckDir, dir) +
                tarNeckHeadDir.magnitude * AbsCosWeight(srcNeckHeadDir, dir);

            dir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.Spine1]);
            float tatTRSpine1 = tarRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                tarRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                tarRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                tarRightHandForearmDir.magnitude * AbsCosWeight(tarRightHandForearmDir, dir) +
                tarChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir);

            dir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.Spine]);
            float tatTRSpine = tarRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                tarRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                tarRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                tarRightHandForearmDir.magnitude * AbsCosWeight(tarRightHandForearmDir, dir) +
                tarChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                tarSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir);

            dir = Position(frame[RetargetBoneType.RightHand]) - Position(frame[RetargetBoneType.Hip]);
            float tatTRHip = tarRightShoulderChestDir.magnitude * AbsCosWeight(srcRightShoulderChestDir, dir) +
                tarRightUpperarmShoulderDir.magnitude * AbsCosWeight(srcRightUpperarmShoulderDir, dir) +
                tarRightForearmUpperarmDir.magnitude * AbsCosWeight(srcRightForearmUpperarmDir, dir) +
                tarRightHandForearmDir.magnitude * AbsCosWeight(tarRightHandForearmDir, dir) +
                tarChestSpine1Dir.magnitude * AbsCosWeight(srcChestSpine1Dir, dir) +
                tarSpine1SpineDir.magnitude * AbsCosWeight(srcSpine1SpineDir, dir) +
                tarSpineHipDir.magnitude * AbsCosWeight(srcSpineHipDir, dir);

            Vector3[] hands = new Vector3[2];
            hands[0] = weightLeftHandChest * (spines[3].solverPosition + egocentricLeftHandChestDir * tatTChest) +
                weightLeftHandHead * (necks[1].solverPosition + egocentricLeftHandHeadDir * tatTHead) +
                weightLeftHandHip * (spines[0].solverPosition + egocentricLeftHandHipDir * tatTHip);
            hands[1] = weightRightHandChest * (spines[3].solverPosition + egocentricRightHandChestDir * tatTRChest) +
                weightRightHandHead * (necks[1].solverPosition + egocentricRightHandHeadDir * tatTRHead) +
                weightRightHandHip * (spines[0].solverPosition + egocentricRightHandHipDir * tatTRHip);

            //hands[0] =  (spines[3].solverPosition + egocentricLeftHandChestDir * tatTChest);
            //hands[1] =  (spines[3].solverPosition + egocentricRightHandChestDir * tatTRChest);
            return hands;
        }

        private float AbsCosWeight(Vector3 direction, Vector3 sDir)
        {
           return Vector3.Dot(direction / direction.magnitude, sDir / sDir.magnitude);
        }

        private void ApplyHeadOffset()
        {
            necks[1].solverRotation = headOffset * necks[1].solverRotation;
        }

        #endregion
    }
}
