/*WSU EECS CptS 451*/
/*Instructor: Sakire Arslan Ay*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace parse_yelp
{
    class Parser
    {
        //initialize the input/output data directory. Currently set to execution folder. 
        public static String dataDir = ".\\";
        static void Main(string[] args)
        {
            JSONParser my_parser =  new JSONParser();

            //Parse yelp_business.json 
            my_parser.parseJSONFile(dataDir + "yelp_business.json", dataDir + "business.sql");

            //Parse yelp_review.json 
          // my_parser.parseJSONFile(dataDir+"yelp_review.json",dataDir+"review.sql");

           //my_parser.parseJSONFile(dataDir + "yelp_user.json", dataDir + "users.sql");
            //my_parser.parseJSONFile(dataDir + "yelp_checkin.json", dataDir + "checkin.sql");

        }
    }
}

