using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSGL.Engine
{
	public abstract class Component
	{
		public static Dictionary<string, Type> ComponentTypes = new Dictionary<string, Type>();
		public virtual void Start() { }
		public virtual void Update() { }
		public Entity ParentEntity = null!;

		// When importing from json to various components, we override this instance method to pass initializing variables into each component instance
		public virtual void Instance(Entity parent)
		{

		}
	}
}
