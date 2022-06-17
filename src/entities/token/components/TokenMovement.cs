using Godot;

namespace Token
{
    class TokenMovement : NodeComponent<Token>
    {
        public TokenMovement(Token instance) : base(instance) { }

        public TextureRect MoveHandle { get => _instance.GetNode<TextureRect>("MoveHandle"); }
        public TextureRect RotateHandle { get => _instance.GetNode<TextureRect>("RotateHandle"); }

        public bool isMoveHandleHeld;
        public bool isRotateHandleHeld;

        public override void Process(float delta)
        {
            if (isMoveHandleHeld)
            {
                // RectGlobalPosition = GetGlobalMousePosition() - (MoveHandle.RectPosition - MoveHandle.RectPivotOffset).Rotated((RectPosition + RectPivotOffset).AngleToPoint(GetGlobalMousePosition()));
                // _instance.RectGlobalPosition = _instance.GetGlobalMousePosition() - (MoveHandle.RectPosition - MoveHandle.RectPivotOffset).Rotated((_instance.RectPosition + _instance.RectPivotOffset).AngleToPoint(_instance.GetGlobalMousePosition()));
                _instance.RectGlobalPosition = _instance.GetGlobalMousePosition() - MoveHandle.RectPosition - MoveHandle.RectPivotOffset;
            }

            if (isRotateHandleHeld)
            {
                var hangleAngle = Mathf.Rad2Deg((_instance.RectPosition + _instance.RectPivotOffset).AngleToPoint(RotateHandle.RectPosition + RotateHandle.RectPivotOffset * 2));
                _instance.RectRotation = Mathf.Rad2Deg((_instance.RectPosition + _instance.RectPivotOffset).AngleToPoint(_instance.GetGlobalMousePosition())) + 90 + hangleAngle;

                RotateHandle.RectRotation = -_instance.RectRotation;
                MoveHandle.RectRotation = -_instance.RectRotation;
                _instance.VisibilityToggle.RectRotation = -_instance.RectRotation;
                _instance.DebugLabel.DebugLabel.RectRotation = -_instance.RectRotation;
                _instance.DebugLabel.UpdateDebugLabel();
            }
        }

        public override void Draw()
        {
            return;

            var rectGlobalCenter = _instance.RectGlobalPosition + _instance.RectPivotOffset;
            var rectLocalCenter = _instance.RectPosition + _instance.RectPivotOffset;
            var handleGlobalCenter = rectGlobalCenter + MoveHandle.RectPosition + MoveHandle.RectPivotOffset - _instance.RectPivotOffset;

            _instance.DrawCircle(rectLocalCenter, 2, new Color(1, 0, 0));
            _instance.DrawCircle(handleGlobalCenter, 2, new Color(0, 1, 0));
            _instance.DrawCircle(_instance.GetLocalMousePosition(), 2, new Color(0, 0, 1));
            _instance.DrawLine((rectLocalCenter), _instance.GetLocalMousePosition(), new Color(1, 0, 0), 1, true);
            _instance.DrawLine(rectLocalCenter, handleGlobalCenter.Rotated((rectLocalCenter).AngleToPoint(_instance.GetGlobalMousePosition())), new Color(0, 1, 0), 1, true);
        }
    }
}