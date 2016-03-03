using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRL.Commons.Tree
{
    public class GTree<T>
    {
        #region Private Variables

        private GNode<T> root;
        private GNode<T> current;

        #endregion

        #region Public Methods

        public void Insert(GNode<T> newNode, bool next = false)
        {
            if (IsEmpty())
            {
                root = newNode;
                newNode.Parent = null;
                current = root;
            }
            else
            {
                current.Childs.Add(newNode);
                newNode.Parent = current;

                if (next)
                    current = newNode;
            }
        }

        public void Select(GNode<T> node)
        {
            if (!IsEmpty())
            {
                GNode<T> sNode = depthSearch(node.ID, root);
                if (sNode != null)
                {
                    current = sNode;
                }
            }
        }

        public List<GNode<T>> GetCurrentChilds()
        {
            if (!IsEmpty())
                return current.Childs;
            return null;
        }

        public GNode<T> GotoParentNode()
        {
            if (current.Parent != null)
            {
                current = current.Parent;
                return current;
            }

            return null;
        }

        public GNode<T> GetRootNode()
        {
            return root;
        }

        public GNode<T> GetCurrentNode()
        {
            return current;
        }

        public bool IsEmpty()
        {
            return (root == null);
        }

        public List<GNode<T>> ToList()
        {
            if (root == null)
                return null;

            return depthTraversal(root);
        }

        #endregion

        #region Private Methods

        private List<GNode<T>> depthTraversal(GNode<T> parent)
        {
            List<GNode<T>> nList = new List<GNode<T>>();
            foreach (var item in parent.Childs)
                nList.AddRange(depthTraversal(item));
            nList.Add(parent);

            return nList;
        }

        private GNode<T> depthSearch(long id, GNode<T> parent)
        {
            if (parent.ID == id)
                return parent;

            foreach (var item in parent.Childs)
            {
                GNode<T> sNode = depthSearch(id, item);
                if (sNode != null)
                    return sNode;
            }

            return null;
        }

        #endregion
    }
}
