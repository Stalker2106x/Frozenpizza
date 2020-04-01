using FrozenPizza.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenPizza
{
    public class Camera
    {
        //Camera settings
        float _zoom, _rotation;
        Vector2 _origin;
        Matrix _transform;

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0f;
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        public Vector2 Pos
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public void Move(Vector2 amount)
        {
            _origin += amount;
        }

        //Returns real viewport
        public Rectangle getViewport()
        {
            return (new Rectangle((int)_origin.X - Options.Config.Width / 2, (int)_origin.Y - Options.Config.Height / 2,
                                     Options.Config.Width, Options.Config.Height));
        }

        //returns viewport relative origin
		public Vector2 getViewportCenter()
		{
			return (new Vector2(Options.Config.Width / 2, Options.Config.Height / 2));
		}

        //Returns the player view
        public Matrix getTransformation()
        {
            _transform = Matrix.CreateTranslation(new Vector3(-_origin.X, -_origin.Y, 0)) *
                         Matrix.CreateRotationZ(_rotation) *
                         Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
                         Matrix.CreateTranslation(new Vector3(Options.Config.Width * 0.5f, Options.Config.Height * 0.5f, 0));
            return (_transform);
        }
    }
}
