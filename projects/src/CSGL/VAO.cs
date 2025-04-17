using System;
using OpenTK.Graphics.OpenGL;
using Logging;
using ContentPipeline.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSGL.Engine.OpenGL
{
	public class VAO : IDisposable
	{
		public int ID;
		private bool initialized;
		public VAO() 
		{
			this.ID = GL.GenVertexArray();
			Log.GL($"Generated VAO: {this.ID}");
			initialized = true;
		}

		public void BindVBO(VBO VBO)
		{
			GL.BindVertexArray(this.ID);
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO.ID);

			// Positional Data (vec3): Uniform Layout 0
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);

			// Vertex Normals (vec3): Uniform Layout 1
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 12 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(1);

			// Tangent Data (vec3): Uniform Layout 2
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 12 * sizeof(float), 6 * sizeof(float));
			GL.EnableVertexAttribArray(2);

			// TexCoord (vec2): Uniform Layout 3
			GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, 12 * sizeof(float), 9 * sizeof(float));
			GL.EnableVertexAttribArray(3);

		}

		public void LinkAttrib(VBO VBO, int layout, int numComponents, VertexAttribPointerType type, int stride, int offset)
		{
			VBO.Bind();
			Log.GL($"VAO:{this.ID} bind VBO:{VBO.ID} (Layout: {layout}, {numComponents}, {type.ToString()}) stride: {stride}, offset: {offset}");
			
			GL.VertexAttribPointer(layout, numComponents, type, false, stride * sizeof(float), offset * sizeof(float));
			GL.EnableVertexAttribArray(layout);
			ErrorCode error = GL.GetError();

			if (error != ErrorCode.NoError)
			{
				Log.GL($"Error linking attribute: {this.ToString()}");
			}

			VBO.Unbind();
		}

		public void Bind()
		{
			GL.BindVertexArray(this.ID);
		}

		public void Unbind()
		{
			GL.BindVertexArray(0);
		}

		public void Dispose()
		{
			if (!this.initialized)
				return;

			GL.DeleteVertexArray(this.ID);
		}
	}
}
