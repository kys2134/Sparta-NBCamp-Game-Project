namespace Backend.Object.Character.Enemy.Node
{
    public class SetRotationToPlayer : ActionNode
    {
        public bool faceToPlayer = false;
        public float faceRotationLerpTime = 2f;
        protected override void Start()
        {
            if (agent.MovementController.IsFaceToPlayer())
            {
                return;
            }

            agent.MovementController.FaceToPlayer = faceToPlayer;

            if (!faceToPlayer)
            {
                return;
            }
            agent.MovementController.FaceLerpTime = faceRotationLerpTime;
        }
        protected override void Stop()
        {

        }
        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}
