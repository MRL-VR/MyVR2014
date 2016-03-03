using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRL.Commons.Tree
{
    public class GNode<T> : ICloneable
    {
        #region Public Variables

        public T Data;
        public List<GNode<T>> Childs { set; get; }
        public GNode<T> Parent { set; get; }

        #endregion

        #region Private Variables
        
        private long id;

        public long ID
        {
            get { return id; }
        }

        #endregion

        #region CTor

        public GNode()
        {
            long newId = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
                newId *= ((int)b + 1);
            
            id = newId;
            Childs = new List<GNode<T>>();
        }

        #endregion

        #region Public Methods

        public object Clone()
        {
            return new GNode<T>() { Childs = this.Childs.ToList(), Data = this.Data, Parent = Parent };
        }

        #endregion
    }
}
