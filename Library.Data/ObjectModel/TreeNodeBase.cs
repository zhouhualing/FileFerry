using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Data.DataObjects
{
	/// <summary>
	/// ���ڵ�Ļ���
	/// </summary>
	/// <typeparam name="TNode">���ڵ������</typeparam>
	/// <typeparam name="TCollection">���ڵ�ļ�������</typeparam>
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
		/// ���ڵ�
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
		/// ��һ���ֵܽڵ�
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
		/// ǰһ���ֵܽڵ�
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
		/// �ӽڵ㼯��
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
		/// ��һ���ӽڵ�
		/// </summary>
		public TNode FirstChild
		{
			get
			{
				return this.Children.FirstNode;
			}
		}

		/// <summary>
		/// ���һ���ӽڵ�
		/// </summary>
		public TNode LastChild
		{
			get
			{
				return this.Children.LastNode;
			}
		}

		/// <summary>
		/// �����ӽڵ�
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
