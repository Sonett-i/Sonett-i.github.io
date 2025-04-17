using ContentPipeline;
using ContentPipeline.Components;
using CSGL.Engine.OpenGL;
using OpenTK.Graphics.OpenGL;
using Logging;
using SharedLibrary;

namespace CSGL.Engine
{
	public class Mesh : Component, IDisposable
	{
		private List<Vertex> Vertices = new List<Vertex>();
		private List<Texture> textures = new List<Texture>();
		public Shader Shader = null!;

		public VAO VAO = null!;
		public VBO VBO = null!;
		public EBO EBO = null!;

		float[] vertexBuffer = null!;
		uint[] indexBuffer = null!;

		BufferUsageHint hint;
		public Mesh() { }

		public Mesh(Vertex[] vertices, uint[] indices, List<Texture> textures, Shader shader, BufferUsageHint hint = BufferUsageHint.StaticDraw)
		{
			//this.vbo = new VBO(vertices);

			this.vertexBuffer = MeshData.Buffer(vertices);
			this.indexBuffer = indices;
			this.Shader = shader;

			this.hint = hint;
			this.textures = textures;

			this.VBO = new VBO(vertices);

			this.VAO = new VAO();

			VAO.Bind();

			this.VAO.LinkAttrib(VBO, 0, 3, VertexAttribPointerType.Float, Vertex.Stride, Vertex.PositionOffset);
			this.VAO.LinkAttrib(VBO, 1, 3, VertexAttribPointerType.Float, Vertex.Stride, Vertex.NormalOffset);
			this.VAO.LinkAttrib(VBO, 2, 3, VertexAttribPointerType.Float, Vertex.Stride, Vertex.TangentOffset);
			this.VAO.LinkAttrib(VBO, 3, 2, VertexAttribPointerType.Float, Vertex.Stride, Vertex.UVOffset);


			ErrorCode error = GL.GetError();
			if (error != ErrorCode.NoError)
			{
				Log.GL($"Error drawing {this.ToString()}");
			}

			this.EBO = new EBO(indices);


			VAO.Unbind();
		}

		public void Draw(Shader shader, Camera camera)
		{
			shader.Activate();
			VAO.Bind();

			EBO.Bind();
			for (int i = 0; i < textures.Count; i++)
			{
				string type = TextureDefinitions.TextureUniformTypes[textures[i].TextureType];

				textures[i].TexUnit(shader, ("material." + type), i);
				textures[i].Bind();
			}

			if (ParentEntity.Lit == true)
			{
				shader.SetUniform("light.position", SceneManager.ActiveScene.MainLight.transform.position);
				
				shader.SetUniform("light.ambient", SceneManager.ActiveScene.MainLight.ambient);
			}

			shader.SetUniform("light.colour", SceneManager.ActiveScene.MainLight.Colour);

			shader.SetUniform("camPos", Camera.main.transform.position);

			shader.SetUniform("model", ParentEntity.transform.Transform_Matrix);
			shader.SetUniform("view", camera.ViewMatrix);
			shader.SetUniform("projection", camera.ProjectionMatrix);
			shader.SetUniform("nearClip", camera.NearClip);
			shader.SetUniform("farClip", camera.FarClip);

			GL.DrawElements(PrimitiveType.Triangles, this.EBO.indexLength, DrawElementsType.UnsignedInt, 0);

			ErrorCode error = GL.GetError();

			if (error != ErrorCode.NoError)
			{
				Log.GL($"Error drawing {this.ToString()}");
			}
		}

		public void Draw()
		{
			Shader.Activate();

			this.VAO.Bind();
			this.EBO.Bind();

			this.Shader.Uniforms["model"].SetValue(ParentEntity.transform.Transform_Matrix);
			this.Shader.Uniforms["view"].SetValue(Camera.main.ViewMatrix);
			this.Shader.Uniforms["projection"].SetValue(Camera.main.ProjectionMatrix);

			this.Shader.SetUniform("camPos", Camera.main.transform.position);


			for (int i = 0; i < this.textures.Count; i++)
			{
				GL.ActiveTexture(TextureUnit.Texture0 + i);
				GL.BindTexture(TextureTarget.Texture2D, textures[i].ID);
			}
		}

		public void Dispose()
		{
			VAO.Dispose();
			VBO.Dispose();
			EBO.Dispose();
			Shader.Dispose();
			foreach (Texture tex in textures) 
			{
				tex.Dispose();
			}
		}

		public override string ToString()
		{
			string output = "VertexBuffer\n\tx\ty\tz\tx\ty\tz\tx\ty\tz\tu\tv";

			for (int i = 0; i < this.vertexBuffer.Length; i++)
			{
				if (i % Vertex.Stride == 0)
					output += "\n";

				output += "\t" + this.vertexBuffer[i];
			}

			output += "\nIndex Buffer\n";

			for (int i = 0; i < this.indexBuffer.Length; i++)
			{
				if (i % 3 == 0)
					output += "\n";
				output += "\t" + this.indexBuffer[i];
			}

			return output;
		}
	}
}