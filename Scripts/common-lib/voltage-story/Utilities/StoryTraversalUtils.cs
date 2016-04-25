using System;

using Newtonsoft.Json.Linq;

namespace Voltage.Story.Utilities
{
    using Voltage.Common.Logging;
    using Voltage.Story.Exceptions;

	public class StoryTraversalUtils
	{
		private const string nodeAssemblyPrefix = "Voltage.Story.Models.Nodes.";
        private const string nodeClassSuffix = "Node";

		public static void TraverseJson(string json, string property, Action<Type,JToken> action)
		{
			if (!string.IsNullOrEmpty(json) && !string.IsNullOrEmpty(property))
			{
				JObject jsonObj = JObject.Parse(json);
				JToken children;
				if(jsonObj.TryGetValue(property, out children))
				{
					JArray jsonArray = (JArray)children;
                    for (int i = 0; i < jsonArray.Count; ++i)
                    {
                        JToken node = jsonArray[i];
                        if (action != null)
                        {
                            Type nodeType = null;
                            nodeType = GetNodeType(node["_class"].ToString());

                            try
                            {
                                action(nodeType, node);
                            }
                            catch (Exception e)
                            {
                                throw new NodeConstructionError(i, node.ToString(), e.Message);
                            }
                        }
                    }
				}
				else
				{
					Console.WriteLine();
//					throw new KeyDoesNotExistException (string.Format("No Key for '{0}'", property));
				}
			}
			else
			{
				throw new ArgumentNullException("");
			}
		}

        private static Type GetNodeType(string classname)
        {
            string type = nodeAssemblyPrefix + classname;
            if (!type.EndsWith(nodeClassSuffix))
            {
                type = type + nodeClassSuffix;
            }

            Type resolvedType = Type.GetType(type);
            if (resolvedType == null)
            {
                throw new Exception(string.Format("No node type found for class: {0}. Searched for class name: {1}", classname, type));
            }

            return resolvedType;
        }
	}
}

