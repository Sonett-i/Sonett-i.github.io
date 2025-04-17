using System;
using ContentPipeline.Components;
using OpenTK.Graphics.OpenGL;
using Logging;
using SharedLibrary;

namespace CSGL.Engine.OpenGL
{
	public class VBO : IDisposable
	{
		public int ID;
		public readonly float[] buffer = null!;
		
		private BufferUsageHint usageHint;

		public VBO(Vertex[] vertices, BufferUsageHint hint = BufferUsageHint.StaticDraw)
		{
			this.usageHint = hint;

			this.buffer = MeshData.Buffer(vertices);

			this.ID = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.ID);
			GL.BufferData(BufferTarget.ArrayBuffer, this.buffer.Length * sizeof(float), this.buffer, hint);

			Log.GL($"Generated VBO: {this.ID}");
		}

		public void Bind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.ID);
		}

		public void Unbind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(this.ID);
		}
	}
}
