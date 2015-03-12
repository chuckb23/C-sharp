using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Json;
using System.Text.RegularExpressions;

namespace parse_yelp
{
    
    class ParseJSONObjects
    {              
        Categories category;

        public ParseJSONObjects( )
        {
            category = new Categories();
            
        }
        public string setup(){
            String insertStr = "";
            String[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            foreach(var item in category.mainCategories){
                insertStr += "INSERT INTO Category(Category_Type, Category_Name) VALUES ('main','"
                    + item.ToString().Replace("\"", "") + "');";
            }
            foreach (var item in days)
            {
                insertStr += "INSERT IGNORE INTO DaysOfWeek(Day) VALUES ('"
                    + item.ToString().Replace("\"", "") + "');";
            }
            Console.Write(insertStr);
            return insertStr;

        }
        public void Close( )
        {
        }

        private int maxLength = 5000;
        private string cleanTextforSQL(string inStr)
        {
            String outStr = Encoding.GetEncoding("iso-8859-1").GetString(Encoding.UTF8.GetBytes(inStr));
            outStr = outStr.Replace("\"", "").Replace("'", " ").Replace(@"\n", " ").Replace(@"\u000a", " ").Replace("\\", " ").Replace("é", "e").Replace("ê", "e").Replace("Ã¼", "A").Replace("Ã", "A").Replace("¤", "").Replace("©", "c").Replace("¶","");
            outStr = Regex.Replace(outStr,@"[^\u0020-\u007E]", "?");
            
            //Only get he first maxLength chars. Set maxLength to the max length of your attribute.
            return outStr.Substring(0, Math.Min(outStr.Length, maxLength));
        }

        
        private string TruncateReviewText(string longText)
        {
            return longText.Substring(0, Math.Min(250,longText.Length))+"...";
        }

        /* The INSERT statement for review tuples*/
        public string ProcessReviews(JsonObject my_jsonStr)
        {

            return "INSERT IGNORE INTO Review (review_id, Votes-funny, Votes-useful, Votes-cool, stars, date, text, user_id, business_id) VALUES ("
                + "'" + cleanTextforSQL(my_jsonStr["review_id"].ToString()) + "' , "
                + my_jsonStr["votes"]["funny"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["votes"]["useful"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["votes"]["cool"].ToString().Replace("\"", "") + ", "
                + my_jsonStr["stars"].ToString().Replace("\"", "") + " , "
                + "'" + cleanTextforSQL(my_jsonStr["date"].ToString()) + "' , "
                + "'" + TruncateReviewText(cleanTextforSQL(my_jsonStr["text"].ToString())) + "' , "
                + "'" + cleanTextforSQL(my_jsonStr["user_id"].ToString()) + "' , "
                + "'" + cleanTextforSQL(my_jsonStr["business_id"].ToString()) + "'); ";
           
                
                
                
                
        }

            
        /* The INSERT statement for business tuples. Note that this function does not extract all data items in the business object.  */
        public string ProcessBusiness(JsonObject my_jsonStr)
        {
            return "INSERT INTO Business (business_id, name, full_address, city, state, latitude, longitude, review_count, stars, open) VALUES ("
                + "'" + my_jsonStr["business_id"].ToString().Replace("\"", "") + "' , "
                + "'" + cleanTextforSQL(my_jsonStr["name"].ToString()) + "' , "
                + "'" + cleanTextforSQL(my_jsonStr["full_address"].ToString()) + "' , "
                + "'" + cleanTextforSQL(my_jsonStr["city"].ToString()) + "' , "
                + "'" + my_jsonStr["state"].ToString().Replace("\"", "") + "' , "
                + my_jsonStr["latitude"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["longitude"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["review_count"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["stars"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["open"].ToString().Replace("\"", "") 
                // + my_jsonStr["attributes"].ToString().Replace("\"", "") + " "
                // + "" + calcAttrBitwise((JsonObject)my_jsonStr["attributes"]) + 
                + ");";
               /* + ProcessBusinessCategories(my_jsonStr)
                + ProcessBusinessAttributes(my_jsonStr)
                + ProcessBusinessAttributeTuples(my_jsonStr, "Ambience")
                + ProcessBusinessAttributeTuples(my_jsonStr, "Music")
                + ProcessBusinessAttributeTuples(my_jsonStr, "Parking")
                + ProcessBusinessAttributeTuples(my_jsonStr, "Dietary Restrictions")
                + ProcessBusinessAttributeTuples(my_jsonStr, "Good For")
                + ProcessBusinessNeighborhoods(my_jsonStr)*/ 
        }

        
        /* The INSERT statement for category tuples*/
        public string ProcessBusinessCategories(JsonObject my_jsonStr)
        {
            String mainCatstr = "";
            String insertString = "";
            JsonArray categories = (JsonArray)my_jsonStr["categories"];
            //append an INSERT statement to insertString for each category of the business 
            for (int i = 0; i < categories.Count; i++)
            {
                insertString = insertString + "INSERT IGNORE INTO BusinessCategory (Business_business_id, Category_Category_Name) VALUES ("
                                + "'" + my_jsonStr["business_id"].ToString().Replace("\"", "") + "' , "
                                + "'" + cleanTextforSQL(categories[i].ToString()) + "'"
                                + ");"
                                + "\n"; //append a new line
                if (category.cExists(cleanTextforSQL(categories[i].ToString())))
                {
                    
                }
                else
                {
                    mainCatstr += "INSERT IGNORE INTO Category(Category_Type, Category_name) VALUES ("
                        + "'sub', '" + cleanTextforSQL(categories[i].ToString()) + "'"
                        + ");";
                }
            }
                return mainCatstr + insertString;
            
        }


       /* public string ProcessBusinessAttributes(JsonObject my_jsonStr)
        {
            String insertString = "";
            int i = 1;
            foreach (var item in my_jsonStr["attributes"])
            {
                if (i++ == my_jsonStr["attributes"].Count)
                {
                    insertString = insertString + item.Value.ToString().Replace("\"", "") + " ; ";
                    break;
                }
                if (item.Key != "Parking"&&item.Key!="Ambience"&&item.Key!="Good For"&&item.Key!="Music"&&item.Key!="Dietary Restrictions")
                {
                    insertString = insertString + item.Value.ToString().Replace("\"", "") + " , ";  
                }
            }
           // Console.Write("\n" + my_jsonStr["attributes"].Count + " : " + i + "\n");
           // Console.Write(insertString);
           // ProcessBusinessAttributeTuples(my_jsonStr, "Parking");
            return insertString;
        }*/

        public string ProcessBusinessAttributes(JsonObject my_jsonStr)
      {
           String insertString = "";
           int i = 1;
           JsonObject attributes = (JsonObject)my_jsonStr["attributes"];
          // if (my_jsonStr["attributes"].ContainsKey(title)!= false)
          // {
              /* foreach (var item in attributes[title])
               {
                   if (i++ == attributes[title].Count)
                   {
                       insertString = insertString + item.Value.ToString().Replace("\"", "") +"; ";
                       break;
                   }
                   insertString = insertString + item.Value.ToString().Replace("\"", "") + " , ";
               }*/
               foreach (var item in attributes)
               {
                   if (item.Value.Count > 0)
                   {
                       foreach (var thing in item.Value)
                       {
                           insertString += "INSERT IGNORE INTO Attributes(name) VALUES('" + item.Key.ToString().Replace(" ", "") +"-" + thing.Key + "');"
                               +"INSERT IGNORE INTO BusinessAttributes(Business_business_id, Attributes_key, value) VALUES('"
                               + my_jsonStr["business_id"].ToString().Replace("\"", "") + "' , '"
                               + item.Key.ToString().Replace(" ","") + "-" + thing.Key + "', '"
                               + thing.Value + "');";
                       }
                   }
                   else
                   {
                       insertString +=  "INSERT IGNORE INTO Attributes(name) VALUES('" + item.Key.ToString().Replace(" ", "") + "');"
                           +"INSERT IGNORE INTO BusinessAttributes(Business_business_id, Attributes_key, value) VALUES('"
                           + my_jsonStr["business_id"].ToString().Replace("\"", "") + "' , '"
                           + item.Key.ToString().Replace(" ", "") +"', '" + item.Value + "');";
                   }
                 //  Console.Write(item.Key + ": " + item.Value + "\n" );
               }
          // }
               Console.Write(insertString + "\n");
           return insertString;
        }
        //cd C:/Program Files/MySQL/MySQL Server 5.6/bin
       //source C:/Users/chuckb23/Developement/ParseYelpData-CptS451/ParseYelpData-CptS451/ParseYelp/bin/Release/business.sql;
        public string ProcessBusinessHours(JsonObject my_jsonStr)
        {
            String[] days = {"Monday","Tuesday","Wednesday","Thursday", "Friday", "Saturday", "Sunday"};
            String insertStr = "";
            JsonObject hours = (JsonObject)my_jsonStr["hours"];
            for (int i = 0; i < days.Length; i++)
            {
                if (hours.ContainsKey(days[i]))
                {
                    insertStr += "INSERT IGNORE INTO Open(Business_business_id, DaysOfWeek_Day, Open, Closed) VALUES('"
                    + my_jsonStr["business_id"].ToString().Replace("\"", "") + "' , '"
                    + days[i].Replace("\"", "") +"', '"
                    + hours[days[i]]["open"].ToString().Replace("\"", "") +
                        "', '" + hours[days[i]]["close"].ToString().Replace("\"", "") + "'); ";
                }
            }
            Console.Write(insertStr);
            return insertStr;
            
        }

        public string ProcessBusinessNeighborhoods(JsonObject my_jsonStr)
        {
            String insertStr = "";
            JsonArray neighborhoods = (JsonArray)my_jsonStr["neighborhoods"];
            for (int i = 0; i < neighborhoods.Count; i++)
            {
                insertStr = insertStr + "'" + my_jsonStr["business_id"].ToString().Replace("\"", "") + "' , "
                    + "'" + cleanTextforSQL(neighborhoods[i].ToString()) + "'" + "; ";
            }
            insertStr = insertStr + "\n";
            Console.Write(insertStr);
            return insertStr;
        }
        
        //Process our main User attributes
        public string ProcessUsers(JsonObject my_jsonStr)
        {
            String insertStr = "";

            insertStr += "INSERT IGNORE INTO User (UserID, review-Count, avg-stars, fans, name, yelping-since, votes-funny,votes-useful,votes-cool) VALUES("
                +"'" + cleanTextforSQL(my_jsonStr["user_id"].ToString()) + "' , "
                + my_jsonStr["review_count"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["average_stars"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["fans"].ToString().Replace("\"", "") + " , "
                + "'" + cleanTextforSQL(my_jsonStr["name"].ToString()) + "' , "
                + "'" + cleanTextforSQL(my_jsonStr["yelping_since"].ToString()) + "' , "
                + my_jsonStr["votes"]["funny"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["votes"]["useful"].ToString().Replace("\"", "") + " , "
                + my_jsonStr["votes"]["cool"].ToString().Replace("\"", "") + ") "
                
               
                
                
               
                + ";";
            Console.Write(insertStr);
            return insertStr;
        //Add friends and add compliments and elite
        }

        //Get years elite if they exist
        public string ProcessUsersElite(JsonObject my_jsonStr)
        {
            String insertStr = "";
            JsonArray elite = (JsonArray)my_jsonStr["elite"];
            for (int i = 0; i < elite.Count; i++)
            {
                insertStr += "INSERT IGNORE INTO EliteUser (User_UserID, Elite_year) VALUES('"
                + my_jsonStr["user_id"].ToString().Replace("\"", "") + "' , "
                + elite[i].ToString().Replace("\"", "") + "); ";
            }
            Console.Write(insertStr);
            return insertStr;
        }

        //Get users friends if they have any
        public string ProcessUsersFriends(JsonObject my_jsonStr)
        {
            String insertStr = "";
            foreach (var item in my_jsonStr["friends"])
            {
                insertStr += "INSERT IGNORE INTO Friends (User_UserID, User_UserID1) VALUES('"
                + my_jsonStr["user_id"].ToString().Replace("\"", "") + "' , '"
                + cleanTextforSQL(item.Value.ToString()) + "'" + "); "; 
            }
            Console.Write(insertStr);
            return insertStr;
        }

        //Get users compliments if they have any
        public string ProcessUsersCompliments(JsonObject my_jsonStr)
        {
            String insertStr = "";
            int i = 1;
            foreach (var item in my_jsonStr["compliments"])
            {
               // if (i++ == my_jsonStr["compliments"].Count)
                //{
                    insertStr += "INSERT IGNORE INTO UserCompliments (User_UserID, Compliments_compliment_type, num_comp) VALUES ("
                    + "'" + my_jsonStr["user_id"].ToString().Replace("\"", "") + "' , '"
                    + item.Key.ToString().Replace("\"", "") + "' , " + item.Value.ToString().Replace("\"", "") + "; ";
                //    break;
              //  }
              //  insertStr+=item.Value.ToString().Replace("\"", "") + ", ";
            }
            Console.Write(insertStr);
            return insertStr;
        }
        
        public string ProcessCheckins(JsonObject my_jsonStr)
        {
            String insertStr = ""; String[] split;
            
            using (var enumerator = my_jsonStr["checkin_info"].GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    bool last;
                    var current = enumerator.Current;
                    do
                    {
                        
                        current = enumerator.Current;
                        split = current.Key.Split('-');
                        insertStr += "INSERT IGNORE INTO BusinessCheckin (Business_business_id, Checkin_dayofweek, Checkin_time, numberofcheck) VALUES ('"
                            + my_jsonStr["business_id"].ToString().Replace("\"", "") + "', "
                            + split[1].ToString().Replace("\"", "") + ", " + split[0].ToString().Replace("\"", "") + ", " + current.Value.ToString() + ");"; 
                        last = !enumerator.MoveNext();
                    } while (!last);
                    //var final = enumerator.Current;
                   // insertStr = insertStr +"'" + my_jsonStr["business_id"].ToString().Replace("\"", "'") + ";\n";
                   Console.Write(insertStr);
                    //return insertStr;

                }
            }
            return insertStr;
          
        }               

    }
}
