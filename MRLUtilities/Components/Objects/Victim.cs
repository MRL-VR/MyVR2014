
namespace MRL.Components.Tools.Objects
{
    public class Victim
    {
        private string _name;
        private int _id;
        private string _status;
        private double _probability;
        private string _position;
        public Victim(Victim victim)
        {
            this._id = victim._id;
            this._name = victim._name;
            this._position = victim.Position;
            this._status = victim._status;
            this._probability = victim._probability;

        }
        public Victim(int id, string name, string position, string status, int health)
        {
            this._id = id;
            this._name = name;
            this._status = status;
            this._probability = health;
            this._position = position;
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public double Probability
        {
            get { return _probability; }
            set { _probability = value; }
        }
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public string NameAndProbablity { get { return _name + "(" + _probability.ToString() + ")"; } }
        public override string ToString()
        {
            return _name;
        }

    }
}
