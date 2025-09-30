using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class EnemyMovementController : MovementController
    {
        [field: SerializeField] public GameObject Target { get; set; }
        [field: SerializeField] public float Distance { get; private set; }
        [field: SerializeField] public float StrafeSpeed { get; private set; }


        public bool FaceToPlayer;
        public float FaceLerpTime = 2f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            Distance = GetDistance();

            if (FaceToPlayer)
            {
                SetLerpRotation(FaceLerpTime);
            }
        }

        public void MoveToTarget(float speed, float speedFactor = 1)
        {
            transform.position += speedFactor * speed * Time.deltaTime * transform.forward;
        }

        public void MoveToTarget(Vector3 originPos, Vector3 targetPos, float speed)
        {
            Vector3 dir = GetDirection(originPos, targetPos);
            transform.position += speed * Time.deltaTime * dir;
        }

        private float GetDistance()
        {
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            return distance;
        }

        public void SetRotation()
        {
            Vector3 dir = GetDirection();
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        public void SetLerpRotation(float speed)
        {
            Vector3 dir = GetDirection();
            dir.y = 0f;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
            }
        }

        public void SetLerpRotation(Vector3 originPos, Vector3 targetPos, float speed)
        {
            Vector3 dir = GetDirection(originPos, targetPos);
            dir.y = 0f;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
            }
        }

        public Vector3 GetDirection()
        {
            return (Target.transform.position - transform.position).normalized;
        }

        public Vector3 GetDirection(Vector3 originPos, Vector3 targetPos)
        {
            return (targetPos - originPos).normalized;
        }

        public bool IsFaceToPlayer()
        {
            Vector3 dir = GetDirection();
            dir.y = 0f;
            // 두 벡터의 내적(dot product)을 사용하여 방향 유사도를 계산합니다. (1에 가까울수록 같은 방향)
            float similarity = Vector3.Dot(dir, transform.forward);
            if (similarity > 0.99f)
            {
                return true;
            }
            return false;
        }
    }
}
