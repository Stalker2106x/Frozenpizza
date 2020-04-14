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

    //Reference to graphicsdevice
    GraphicsDevice _graphics;

    public Camera(GraphicsDevice graphicsDevice)
    {
      _graphics = graphicsDevice;
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
    public Rectangle getWorldViewport()
    {
      return (new Rectangle((int)_origin.X - _graphics.Viewport.Width / 2, (int)_origin.Y - _graphics.Viewport.Height / 2,
                              _graphics.Viewport.Width, _graphics.Viewport.Height));
    }
    public Rectangle getGridViewport()
    {
      var wViewPort = getWorldViewport();
      return (new Rectangle(GameMain.map.WorldToGrid(new Vector2(wViewPort.X, wViewPort.Y)),
                              new Point(_graphics.Viewport.Width / GameMain.map.tileSize.X, _graphics.Viewport.Height / GameMain.map.tileSize.Y)));
    }

    //returns viewport relative origin
    public Vector2 getViewportCenter()
    {
      return (new Vector2(_graphics.Viewport.Width / 2, _graphics.Viewport.Height / 2));
    }

    //Returns the player view
    public Matrix getTransformation()
    {
      _transform = Matrix.CreateTranslation(new Vector3(-_origin.X, -_origin.Y, 0)) *
                   Matrix.CreateRotationZ(_rotation) *
                   Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(_graphics.Viewport.Width * 0.5f, _graphics.Viewport.Height * 0.5f, 0));
      return (_transform);
    }
  }
}
