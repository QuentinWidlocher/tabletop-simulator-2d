using Godot;

namespace Token
{
    class TokenMovement : NodeComponent<Token>
    {
        public TokenMovement(Token instance) : base(instance) { }

        public TextureRect MoveHandle { get => _instance.GetNode<TextureRect>("MoveHandle"); }
        public TextureRect RotateHandle { get => _instance.GetNode<TextureRect>("Sprite/RotateHandle"); }

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
                // var hangleAngle = Mathf.Rad2Deg((_instance.RectPosition + _instance.RectPivotOffset).AngleToPoint(RotateHandle.RectPosition + RotateHandle.RectPivotOffset * 2));
                // _instance.Sprite.RectRotation = Mathf.Rad2Deg((_instance.RectPosition + _instance.RectPivotOffset).AngleToPoint(_instance.GetGlobalMousePosition())) + 90 + hangleAngle;

                // RotateHandle.RectRotation = -_instance.RectRotation;
                // MoveHandle.RectRotation = -_instance.RectRotation;
                // _instance.VisibilityToggle.RectRotation = -_instance.RectRotation;
                // _instance.DebugLabel.DebugLabel.RectRotation = -_instance.RectRotation;
                // _instance.DebugLabel.UpdateDebugLabel();

                var handleAngle = Mathf.Rad2Deg((_instance.RectGlobalPosition + _instance.RectPivotOffset).AngleToPoint(RotateHandle.RectPosition + RotateHandle.RectPivotOffset));
                _instance.Sprite.RectRotation = Mathf.Rad2Deg((_instance.RectGlobalPosition + _instance.RectPivotOffset).AngleToPoint(_instance.GetGlobalMousePosition())) - handleAngle;

                var shape = new RectangleShape2D();
                shape.Extents = _instance.Sprite.Texture.GetSize() / 2;
                _instance.CollisionShape2D.Shape = shape;
                _instance.RectSize = _instance.Sprite.GetGlobalRect().Size;

                _instance.DebugLabel.UpdateDebugLabel();
            }
        }

        public override void Draw()
        {
            if (_instance.Name == "Token3")
            {
                var spr = _instance.Sprite;
                var txt = spr.Texture;
                var pos = spr.RectPosition;
                var ngl = Mathf.Deg2Rad(spr.RectRotation);

                var localRedPoint = pos;
                var redPoint = pos;

                var localGreenPoint = redPoint + new Vector2(txt.GetSize().x, 0);
                var greenPoint = localGreenPoint.Rotated(ngl);

                var localBluePoint = greenPoint + new Vector2(0, txt.GetSize().y);
                var bluePoint = localBluePoint.Rotated(ngl);

                var localBlackPoint = bluePoint - new Vector2(txt.GetSize().x, 0);
                var blackPoint = localBlackPoint.Rotated(ngl);

                _instance.DrawCircle(redPoint, 5, new Color(1, 0, 0));
                _instance.DrawLine(redPoint, greenPoint, new Color(1, 0, 0), 5);
                _instance.DrawCircle(greenPoint, 5, new Color(0, 1, 0));
                _instance.DrawLine(greenPoint, bluePoint, new Color(0, 1, 0), 5);
                _instance.DrawCircle(bluePoint, 5, new Color(0, 0, 1));
                _instance.DrawLine(bluePoint, blackPoint, new Color(0, 0, 1), 5);
                _instance.DrawCircle(blackPoint, 5, new Color(0, 0, 0));
                _instance.DrawLine(blackPoint, redPoint, new Color(0, 0, 0), 5);
            }

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