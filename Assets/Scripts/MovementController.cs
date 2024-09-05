using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private float offsetY = 0.125f;
    [SerializeField] private float heightFromGround = 0.5f;
    [SerializeField] private float ikSpeed = 20.0f;
    [SerializeField] private float pelvisSpeed = 5.0f;
    [SerializeField] private string leftFootIKCurve = "LeftFootIKCurve";
    [SerializeField] private string rightFootIKCurve = "RightFootIKCurve";

    [Header("Character movement stats")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    [Header("Gravity handling")]
    [SerializeField] private float _gravityForce = 20;
    private float _currentAttractionCharacter = 0;

    private CharacterController _characterController;
    private Animator _animator;
    private Transform leftFoot, rightFoot;
    private Vector3 leftFootPos, rightFootPos, lastLeftFootPos, lastRightFootPos;
    private Quaternion leftFootRotation, rightFootRotation;
    private float leftFootIKWeight, rightFootIKWeight;
    private float lastPelvisPosY = Mathf.Infinity;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
    }

    private void Update()
    {
        GravityHandling();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        UpdateIK();
        AdjustFeetPositionAndRotation();
        AdjustPelvisHeight();
    }

    public void MoveCharacter(Vector3 moveDirection)
    {
        moveDirection = transform.forward * moveDirection.z + transform.right * moveDirection.x;
        moveDirection *= _moveSpeed;
        moveDirection.y = _currentAttractionCharacter;
        _characterController.Move(moveDirection * Time.deltaTime);
    }

    private void GravityHandling()
    {
        if (!_characterController.isGrounded)
        {
            _currentAttractionCharacter -= _gravityForce * Time.deltaTime;
        }
        else
        {
            _currentAttractionCharacter = 0;
        }
    }

    private void UpdateIK()
    {
        RaycastHit leftHit, rightHit;

        Vector3 leftPos = leftFoot.TransformPoint(Vector3.zero);
        Vector3 rightPos = rightFoot.TransformPoint(Vector3.zero);

        if (Physics.Raycast(leftPos + Vector3.up * heightFromGround, Vector3.down, out leftHit, 1))
        {
            // if the raycasthit is HIGHER than the foot pos in the last frame, the new pos needs to be
            // HIGHER NOW without interpolation - the foot should not vanish in a stone when the stone rises!
            // if the raycasthit is LOWER the foot can get down over time
            leftFootPos = leftHit.point;
            if (leftHit.point.y < lastLeftFootPos.y)
            {
                leftFootPos.y = Mathf.Lerp(lastLeftFootPos.y, leftFootPos.y, Time.deltaTime * ikSpeed);
            }
            var rotation = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
            // getting the y rotation from the animation looks more natural
            leftFootRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        }

        if (Physics.Raycast(rightPos + Vector3.up * heightFromGround, Vector3.down, out rightHit, 1))
        {
            rightFootPos = rightHit.point;
            if (rightHit.point.y < lastRightFootPos.y)
            {
                rightFootPos.y = Mathf.Lerp(lastRightFootPos.y, rightFootPos.y, Time.deltaTime * ikSpeed);
            }
            var rotation = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
            rightFootRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        }

        lastLeftFootPos = leftFootPos;
        lastRightFootPos = rightFootPos;
    }

    private void AdjustFeetPositionAndRotation()
    {
        leftFootIKWeight = _animator.GetFloat(leftFootIKCurve);
        rightFootIKWeight = _animator.GetFloat(rightFootIKCurve);

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootIKWeight);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootIKWeight);

        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos + new Vector3(0, offsetY, 0));
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos + new Vector3(0, offsetY, 0));

        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootIKWeight);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, leftFootIKWeight);

        _animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);
        _animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRotation);
    }

    private void AdjustPelvisHeight()
    {
        if (lastPelvisPosY == Mathf.Infinity)
        {
            lastPelvisPosY = _animator.bodyPosition.y;
        }

        float leftOffsetPos = leftFootPos.y - transform.position.y;
        float rightOffsetPos = rightFootPos.y - transform.position.y;

        float totalOffset = (leftOffsetPos < rightOffsetPos) ? leftOffsetPos : rightOffsetPos;

        Vector3 newPelvisPos = _animator.bodyPosition + Vector3.up * totalOffset;
        newPelvisPos.y = Mathf.Lerp(lastPelvisPosY, newPelvisPos.y, pelvisSpeed * Time.deltaTime);
        _animator.bodyPosition = newPelvisPos;
        lastPelvisPosY = _animator.bodyPosition.y;
    }
}
