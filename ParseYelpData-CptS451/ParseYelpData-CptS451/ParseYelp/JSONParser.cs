/*WSU EECS CptS 451*/
/*Instructor: Sakire Arslan Ay*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Json;
using System.IO;
using System.Text.RegularExpressions;



namespace parse_yelp
{
    class JSONParser
    {
        
        public JSONParser( )
        {
            
        }

        
        public void parseJSONFile(string jsonInput, string sqlOutput)
        {
            int counter;
            string line;
            System.IO.StreamReader jsonfile;
            System.IO.StreamWriter sqlscriptfile;
        
            try
            {
                ParseJSONObjects json2db = new ParseJSONObjects();
                Console.WriteLine("\nCreating: " + sqlOutput );
                Console.Write("Progress:");
                // Read the json data jsonfile.
                jsonfile = new System.IO.StreamReader(jsonInput);
                // Create the sql script file. The script file is formatted for MySQL. If using Miscroft SQL Server should change the format - see Appendix B in Milestone 2 description
                sqlscriptfile = new System.IO.StreamWriter(sqlOutput);
                counter = 0;
                sqlscriptfile.WriteLine(json2db.setup());
                while ((line = jsonfile.ReadLine()) != null)
                {
                    JsonObject my_jsonStr = (JsonObject)JsonObject.Parse(line);
                    string type = my_jsonStr["type"].ToString();
                    
                    switch (type)
                    {
                        case "\"review\"":
                                sqlscriptfile.WriteLine(json2db.ProcessReviews(my_jsonStr));                                         
                                break;
                        case "\"user\"":
                                sqlscriptfile.WriteLine(json2db.ProcessUsers(my_jsonStr));
                                //sqlscriptfile.WriteLine(json2db.ProcessUsersFriends(my_jsonStr));
                                //sqlscriptfile.WriteLine(json2db.ProcessUsersElite(my_jsonStr));
                               // sqlscriptfile.WriteLine(json2db.ProcessUsersCompliments(my_jsonStr));
                                break;
                        case "\"checkin\"": 
                                sqlscriptfile.WriteLine(json2db.ProcessCheckins(my_jsonStr));
                                break;
                        case "\"business\"":
                                
                                sqlscriptfile.Write(json2db.ProcessBusiness(my_jsonStr));
                                sqlscriptfile.Write(json2db.ProcessBusinessCategories(my_jsonStr));
                                sqlscriptfile.Write(json2db.ProcessBusinessAttributes(my_jsonStr));
                                //sqlscriptfile.Write(json2db.ProcessBusinessAttributeTuples(my_jsonStr,"Ambience"));
                               // sqlscriptfile.Write(json2db.ProcessBusinessAttributeTuples(my_jsonStr, "Music"));
                               // sqlscriptfile.Write(json2db.ProcessBusinessAttributeTuples(my_jsonStr, "Parking"));
                                //sqlscriptfile.Write(json2db.ProcessBusinessAttributeTuples(my_jsonStr, "Dietary Restrictions"));
                                //sqlscriptfile.Write(json2db.ProcessBusinessAttributeTuples(my_jsonStr, "Good For")); 
                                //sqlscriptfile.Write(json2db.ProcessBusinessNeighborhoods(my_jsonStr));
                                sqlscriptfile.Write(json2db.ProcessBusinessHours(my_jsonStr));  
                                break;
                        default: Console.WriteLine("Unknown type : " + type);
                            break;
                    }
                    if ((counter % 5000) == 0)
                        Console.Write("■");
                    counter++;
                }
                jsonfile.Close();
                sqlscriptfile.Close();
                
            }
            catch (Exception e)
            {
                Console.Write("Exception:");
                Console.WriteLine(e.Message);
            }
            // Suspend the screen.
            Console.WriteLine("\n"+sqlOutput+": created. \n\n Press a key to continue.");
            Console.ReadLine();
        
        }



 

       


 


    }

    
}
