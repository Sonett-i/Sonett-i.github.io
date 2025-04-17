using System;
using System.Text.Json;
using OpenTK.Mathematics;


#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace CSGL.Engine
{
	public class Transform : Component
	{
		public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public Transform()
		{
			this.position = Vector3.Zero;
			this.rotation = Quaternion.Identity;
			this.scale = Vector3.One;
		}

		// Matrix representing the transformation (TRS) of this transform
		public Matrix4 Transform_Matrix = Matrix4.Identity;

		// Public property for world position
		public Vector3 position
		{
			get
			{
				return WorldPosition;
			}

			set
			{
				WorldPosition = value;
			}
		}

		// Public property for world rotation
		public Quaternion rotation
		{
			get
			{
				return WorldRotation;
			}
			set
			{
				WorldRotation = value;
			}
		}

		public Vector3 scale
		{
			get 
			{
				return LocalScale;
			}
			set 
			{
				LocalScale = value;
			}
		}

		// Parent transform (dictates world position/rotation inheritance
		private Transform? _parent = null!;
		public Transform? Parent
		{
			get => _parent;
			set
			{
				if (_parent != null)
					_parent.Children.Remove(this);

				_parent = value;
				_parent.Children.Add(this);
			}
		}

		// Local transforms relative to parent (or world) if no parent present
		public Vector3 LocalPosition { get; set; } = Vector3.Zero;
		public Quaternion LocalRotation { get; set; } = Quaternion.Identity;
		public Vector3 LocalScale { get; set; } = Vector3.One;

		// List of child transforms that will be affected by this transform
		public List<Transform> Children { get; private set; } = new List<Transform>();

		// Forward Vector based on local rotation
		public Vector3 Forward
		{
			get
			{
				float radY = MathHelper.DegreesToRadians(LocalRotation.ToEulerAngles().Y);
				float radX = MathHelper.DegreesToRadians(LocalRotation.ToEulerAngles().X);

				return new Vector3(
					(float)(MathF.Cos(radY) * MathF.Cos(radX)),
					(float)(MathF.Sin(radX)),
					(float)(MathF.Sin(radY) * MathF.Cos(radX))
					);
			}
		}

		// Right vector is cross product of forward vector and local up vector
		public Vector3 Right
		{
			get
			{
				return Vector3.Cross(Forward, Vector3.UnitY).Normalized();
			}
		}

		// http://graphics.cs.cmu.edu/courses/15-466-f17/notes/hierarchy.html 
		// https://community.khronos.org/t/understanding-child-parent-transformation/74289

		public Vector3 WorldPosition
		{
			get
			{
				// Return local position if no parent present, otherwise transform this object's local transform based on parent's position and rotation
				if (Parent == null) return LocalPosition;
				return Parent.WorldPosition + Vector3.Transform(LocalPosition, Parent.rotation);
			}
			set
			{
				if (Parent == null)
				{
					LocalPosition = value;
				}
				else
				{
					LocalPosition = Vector3.Transform(value - Parent.WorldPosition, Quaternion.Invert(Parent.rotation));
				}
			}
		}

		public Quaternion WorldRotation
		{
			get
			{
				if (Parent == null)
					return LocalRotation;

				return Parent.WorldRotation * LocalRotation;
			}
			set
			{
				if (Parent == null)
				{
					LocalRotation = value;
				}
				else
				{
					LocalRotation = Quaternion.Invert(Parent.WorldRotation) * value;
				}
			}
		}

		public void UpdateTransforms()
		{
			this.Transform_Matrix = MathU.TRS(this);

			if (this.Parent != null)
				this.Transform_Matrix *= this.Parent.Transform_Matrix;

			foreach (Transform child in Children)
			{
				child.UpdateTransforms();
			}
		}

		public void RotateAround(Vector3 point, float angle, Vector3 axis)
		{
			Vector3 offset = LocalPosition - point;

			Quaternion rotation = Quaternion.FromAxisAngle(axis.Normalized(), MathHelper.DegreesToRadians(angle));

			offset = Vector3.Transform(offset, rotation);

			LocalPosition = point + offset;
			LocalRotation = rotation * LocalRotation;
		}

		public Matrix4 TRS()
		{
			Matrix4 translation = Matrix4.CreateTranslation(this.LocalPosition);
			Matrix4 rotation = Matrix4.CreateFromQuaternion(this.LocalRotation);
			Matrix4 scale = Matrix4.CreateScale(this.LocalScale);

			return scale * rotation * translation;
		}
		public override void Instance(Entity parent)
		{

		}
	}
}