using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics;
namespace WindowsFormsApplication1
{
    class MySQLConnector
    {


        private MySqlConnection conn;
        public MySQLConnector()
        {
            try
            {
                Init();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("You have failed me mysql\n", ex.ToString());
            }
        }
        public void Init(){
            string server;
            string database;
            string uid;
            string password;
          //  Console.Write("Success");
            server = "localhost";
            database = "yelpdb";
            uid = "root";
            password = "Mchuckb23";
            string cs = "SERVER=" + server + ";"+ "DATABASE=" + database + ";" +  "UID=" + uid + ";" + "PWD=" + password + ";";
            //Console.Write(cs);
            conn = new MySqlConnection(cs); 
          
            }
        private bool OpenConnection(){
            Console.Write("Hello");
            try{
                conn.Open();
                Debug.WriteLine("Success");
                return true;
            }catch(MySqlException ex){
                Debug.WriteLine("You have failed me mysql\n", ex.ToString());
                if(ex.Number == 0 ){
                    return false;
                }
                else if(ex.Number == 1045){ //invalid username/password
                    return false;
                }
            }
            return false;
        }
        private bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }
        public List<String> SQLSel(string query, string column)
        {
            List<string> queryResult = new List<string>();
            Debug.WriteLine("Connection OPen");
            if (this.OpenConnection()==true)
            {
                Debug.WriteLine("Connection OPen");
                MySqlCommand cmd = new MySqlCommand(query,conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                    queryResult.Add(reader.GetString(column));

                reader.Close();
                this.CloseConnection();
            }
            return queryResult;
        }

    }
}
