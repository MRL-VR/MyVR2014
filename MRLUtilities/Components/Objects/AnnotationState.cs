
namespace MRL.Components.Tools.Objects
{
    public class AnnotationState
    {
        private int _id;
        private string _description = "No Description";
        public AnnotationState(int id)
        {
            this._id = id;
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
