using System.Data.SqlClient;

namespace MRL.Utils
{
    public abstract class DBHandler
    {
        //private string ServerIP = ".\\sql2008";
        private string ServerIP = "lpc:.\\sql2008";
        private string ServerUserName = "sa";
        private string ServerPass = "123";

        protected SqlConnection connection = null;
        
        protected DBHandler()
        {
            string conString = "Data Source=" + ServerIP + ";Initial Catalog=MRLVR;User ID=" + ServerUserName + ";Password=" + ServerPass;
            connection = new SqlConnection(conString);
        }
    }
}
