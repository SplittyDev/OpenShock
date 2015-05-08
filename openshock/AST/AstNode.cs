using System;
using System.Collections.Generic;

namespace openshock
{
	public class AstNode
	{
		public List<AstNode> children;
		public AstNodeType type;

		public AstNode () {
			children = new List<AstNode> ();
		}

		public static AstNode Create (AstNodeType type, AstNode[] children) {
			var node = new AstNode ();
			node.type = type;
			node.children = new List<AstNode> (children);
			return node;
		}

		public static AstNode Create (AstNodeType type, List<AstNode> children) {
			var node = new AstNode ();
			node.type = type;
			node.children = children;
			return node;
		}
	}
}

