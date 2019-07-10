using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Data.DataObjects
{
	/// <summary>
	/// 树节点的基类
	/// </summary>
	/// <typeparam name="TNode">树节点的类型</typeparam>
	/// <typeparam name="TCollection">树节点的集合类型</typeparam>
	public abstract class TreeNodeBase<TNode, TCollection>
		where TNode : TreeNodeBase<TNode, TCollection>
		where TCollection : TreeNodeBaseCollection<TNode, TCollection>, new()
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public delegate bool TraverseNodeHandler(TreeNodeBase<TNode, TCollection> node, object context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public delegate bool BeforeTraverseNodeHandler(TreeNodeBase<TNode, TCollection> node, object context);

		private TNode parent = null;
		private TNode nextSibling = null;
		private TNode previousSibing = null;
		private TCollection children = new TCollection();

		/// <summary>
		/// 父节点
		/// </summary>
		public TNode Parent
		{
			get
			{
				return this.parent;
			}
			internal set
			{
				this.parent = value;
			}
		}

		/// <summary>
		/// 下一个兄弟节点
		/// </summary>
		public TNode NextSibling
		{
			get
			{
				return this.nextSibling;
			}
			internal set
			{
				this.nextSibling = value;
			}
		}

		/// <summary>
		/// 前一个兄弟节点
		/// </summary>
		public TNode PreviousSibing
		{
			get
			{
				return this.previousSibing;
			}
			internal set
			{
				this.previousSibing = value;
			}
		}

		/// <summary>
		/// 子节点集合
		/// </summary>
		public TCollection Children
		{
			get
			{
				if (this.children.parent == null)
					this.children.parent = (TNode)this;

				return this.children;
			}
		}

		/// <summary>
		/// 第一个子节点
		/// </summary>
		public TNode FirstChild
		{
			get
			{
				return this.Children.FirstNode;
			}
		}

		/// <summary>
		/// 最后一个子节点
		/// </summary>
		public TNode LastChild
		{
			get
			{
				return this.Children.LastNode;
			}
		}

		/// <summary>
		/// 遍历子节点
		/// </summary>
		/// /// <param name="beforeHandler"></param>
		/// <param name="handler"></param>
		/// <param name="context"></param>
		public void TraverseChildren(TraverseNodeHandler handler, BeforeTraverseNodeHandler beforeHandler, object context)
		{
			bool continued = true;

			if (beforeHandler != null)
				continued = beforeHandler(this, context);

			if (!continued)
				return;

			foreach (TNode node in children)
			{
				if (handler != null)
					continued = handler(node, context);

				if (continued)
					node.TraverseChildren(handler, beforeHandler, context);
				else
					break;
			}
		}
	}
}
