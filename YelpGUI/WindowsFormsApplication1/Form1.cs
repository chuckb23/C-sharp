using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        MySQLConnector mydb;
        public Form1()
        {
            //Debug.WriteLine("Success");
            InitializeComponent();
            mydb = new MySQLConnector();
            string qstr = "SELECT DISTINCT Category_Category_name as name FROM businesscategory WHERE category_type = 'main' ORDER BY name ";
            List<string> qresult = mydb.SQLSel(qstr, "name");
            for (int i = 0; i < qresult.Count(); i++)
            {
                checkedListBox1.Items.Add(qresult[i]);
            }
            listView1.View = View.Details;
            listView2.View = View.Details;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string check = checkedListBox1.SelectedItem.ToString();
            int idx = checkedListBox1.SelectedIndex;
          
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(checkedListBox1.CheckedItems.Count!=0)
              populateCategories();
            if (checkedListBox2.CheckedItems.Count != 0)
                populateAttributes();
            if (checkedListBox3.CheckedItems.Count != 0)
            {
                listView1.Items.Clear();
                if (comboBox1.SelectedItem != "" && comboBox3.Text != "" && comboBox4.Text != "")
                {
                    hours();
                }
                else
                {
                    findBusiness();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void populateCategories()
        {
            //checkedListBox2.Items.Clear();
            int count = 1;
            if (checkedListBox1.CheckedItems.Count > 0)
            {
                string qstr = "SELECT DISTINCT Category_Category_Name FROM businesscategory WHERE Business_business_id"
                + " IN (SELECT Business_business_id FROM businesscategory where Category_Category_Name IN (";
                foreach (string indexChecked in checkedListBox1.CheckedItems)
                {
                    if (count++ == checkedListBox1.CheckedItems.Count)
                    {
                        qstr += "'" + indexChecked + "'))ORDER BY Category_Category_Name;";
                        break;
                    }
                    qstr += "'" + indexChecked + "',";
                }
                List<string> qresult = mydb.SQLSel(qstr, "Category_Category_Name");
                Debug.WriteLine(qresult);
                for (int i = 0; i < qresult.Count(); i++)
                {
                    if(!checkedListBox2.Items.Contains(qresult[i]))
                        checkedListBox2.Items.Add(qresult[i]);
                }
            }
        }
        private void populateAttributes()
        {
          //checkedListBox3.Items.Clear();
           /* string qstr = "SELECT DISTINCT CONCAT(Attributes_key,'_', value) AS fullkey FROM businessattributes WHERE Business_business_id IN "
                +"(SELECT Business_business_id FROM businesscategory where Category_Category_Name IN "
            +"('Cafes','Mexican')) ORDER BY fullkey;";*/
            int count = 1;
            //INNER JOIN slow return
           string qstr = "SELECT DISTINCT CONCAT(businessattributes.Attributes_key,'_',value) as fullkey FROM businesscategory INNER JOIN businessattributes ON "
            + "businessattributes.business_business_id = businesscategory.business_business_id WHERE businesscategory.category_category_name IN(";
            foreach (string indexChecked in checkedListBox2.CheckedItems)
            {
                if (count++ == checkedListBox2.CheckedItems.Count)
                {
                    qstr += "'" + indexChecked + "')ORDER BY fullkey;";
                    break;
                }
                qstr += "'" + indexChecked + "',";
            }
            List<string> qresult = mydb.SQLSel(qstr, "fullkey");
            //List<string> qresult1 = mydb.SQLSel(qstr, "value");
            Debug.WriteLine(qresult);
            for (int i = 0; i < qresult.Count(); i++)
            {
               // checkedListBox3.Items.Add(qresult[i]);
              /*  if(qresult1[i].Equals("true"))
                    checkedListBox3.Items.Add(qresult[i]);
                else if(qresult1[i].Equals("{}"))
                    Console.Write("Hello");
                else*/
                if(!checkedListBox3.Items.Contains(qresult[i].Replace("\"","")))
                    checkedListBox3.Items.Add(qresult[i].Replace("\"", ""));
                    
            }

        }
        private void findBusiness()
        {
            listView1.Items.Clear();
            int count=1;int count2=1;
            string qstr = "SELECT name, city, state, stars,business_id FROM business, businessattributes WHERE business_id = business_business_id AND "
            + "CONCAT(attributes_key,'_',value) IN (";
            foreach (string indexChecked in checkedListBox3.CheckedItems)
            {
                if (count++ == checkedListBox3.CheckedItems.Count)
                {
                    qstr += "'" + indexChecked + "')";
                    break;
                }
                qstr+= "'"+indexChecked+"', ";
            }
            qstr += " AND business_business_id IN (SELECT DISTINCT businesscategory.business_business_id AS id FROM businesscategory "
            + "INNER JOIN businessattributes ON "
            + "businessattributes.business_business_id = businesscategory.business_business_id WHERE businesscategory.category_category_name "
            + "IN(";
            if (comboBox2.SelectedItem.ToString() != "All Attributes")
            {
                count = 1;
            }
            foreach (string catChecked in checkedListBox2.CheckedItems)
            {
                if (count2++ == checkedListBox2.CheckedItems.Count)
                {
                    qstr += "'" + catChecked + "')";
                    break;
                }
                qstr += "'" + catChecked + "', ";
            }
            qstr+=")GROUP BY business_id HAVING count(*)>=" + (count-1) +" ORDER BY name;";
            List<string> qresultName = mydb.SQLSel(qstr, "name");
            List<string> qresultCity = mydb.SQLSel(qstr, "city");
            List<string> qresultState = mydb.SQLSel(qstr, "state");
            List<string> qresultStars = mydb.SQLSel(qstr, "stars");
            List<string> qresultID = mydb.SQLSel(qstr, "business_id");
            //List<string> qresult1 = mydb.SQLSel(qstr, "value");
            Debug.WriteLine(qresultName);
           //listView1.Items[0].SubItems.Add("00");
            listView1.View = View.Details;
            for (int i = 0; i < qresultName.Count; i++)
            {
                listView1.Items.Add(
                     new ListViewItem(new[] { 
                            qresultName[i].Replace("\"", ""), qresultCity[i].Replace("\"", ""), qresultState[i],qresultStars[i],qresultID[i] }));
                            

                //listView1.Columns[0].Text = qresult[i].Replace("\"", "");
             // listView1.Items[i].SubItems.Add();

            }
    
           // listView1.View = View.Details;
        }
        private void getReviews()
        {
            string qstr = " ";
            //string name = listView1.SelectedItems.
            foreach (ListViewItem bus in listView1.SelectedItems)
            {
                // Debug.WriteLine(listView1.Items.ContainsKey(bus));
                qstr = "SELECT text, stars, review.votes_useful, dates, name  FROM review, users WHERE UserID = User_UserID AND Business_business_id = "
                    + "'" + bus.SubItems[4].Text + "'ORDER BY dates DESC;";
            }
            List<string> qresultReview = mydb.SQLSel(qstr, "text");
            List<string> qresultUseful = mydb.SQLSel(qstr, "Votes_useful");
            List<string> qresultDates = mydb.SQLSel(qstr, "dates");
            List<string> qresultStars = mydb.SQLSel(qstr, "stars");
            List<string> qresultID = mydb.SQLSel(qstr, "name");
            for (int i = 0; i < qresultReview.Count; i++)
            {
                listView2.Items.Add(
                     new ListViewItem(new[] { 
                            qresultDates[i].Replace("\"", ""), qresultStars[i], qresultReview[i],qresultID[i],qresultUseful[i]}));


                //listView1.Columns[0].Text = qresult[i].Replace("\"", "");
                // listView1.Items[i].SubItems.Add();

            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
        }
        private void listView1DoubleClick(object sender, EventArgs e)
        {
            button3.Visible = true;
            listView2.Visible = true;
            button4.Visible = false;
            listView2.Items.Clear();
            getReviews();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            listView2.Visible = false;
            button3.Visible = false;
            button4.Visible = true;
            
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void hours()
        {
            int count = 1; int count2 = 1;
            string qstr = "SELECT name, city, state, stars,business_id, open.open, open.closed FROM business, businessattributes, open WHERE open.Business_business_id = business_id AND DaysofWeek_day = "
            + "'" + comboBox1.SelectedItem + "' AND business_id = businessattributes.business_business_id AND "
            + "CONCAT(attributes_key,'_',value) IN (";
            foreach (string indexChecked in checkedListBox3.CheckedItems)
            {
                if (count++ == checkedListBox3.CheckedItems.Count)
                {
                    qstr += "'" + indexChecked + "')";
                    break;
                }
                qstr += "'" + indexChecked + "', ";
            }
            qstr += " AND businessattributes.business_business_id IN (SELECT DISTINCT businesscategory.business_business_id AS id FROM businesscategory "
            + "INNER JOIN businessattributes ON "
            + "businessattributes.business_business_id = businesscategory.business_business_id WHERE businesscategory.category_category_name "
            + "IN(";
            if (comboBox2.SelectedItem.ToString() != "All Attributes")
            {
                count = 1;
            }
            foreach (string catChecked in checkedListBox2.CheckedItems)
            {
                if (count2++ == checkedListBox2.CheckedItems.Count)
                {
                    qstr += "'" + catChecked + "')";
                    break;
                }
                qstr += "'" + catChecked + "', ";
            }
            qstr += ")GROUP BY business_id HAVING count(*)>=" + (count - 1) + " ORDER BY name;";
            List<string> qresultName = mydb.SQLSel(qstr, "name");
            List<string> qresultCity = mydb.SQLSel(qstr, "city");
            List<string> qresultState = mydb.SQLSel(qstr, "state");
            List<string> qresultStars = mydb.SQLSel(qstr, "stars");
            List<string> qresultID = mydb.SQLSel(qstr, "business_id");
            List<string> qresultOpen = mydb.SQLSel(qstr, "open");
            List<string> qresultClose = mydb.SQLSel(qstr, "closed");
            //List<string> qresult1 = mydb.SQLSel(qstr, "value");
            Debug.WriteLine(qresultName);
            //listView1.Items[0].SubItems.Add("00");
            listView1.View = View.Details;
           for (int i = 0; i < qresultName.Count; i++)
            {
               string opened =  qresultOpen[i].ToString().Replace(":", "");
               string closed = qresultClose[i].ToString().Replace(":", "");
               if (Convert.ToInt32(opened) < Convert.ToInt32(comboBox3.Text.ToString().Replace(":","")) && Convert.ToInt32(closed) > Convert.ToInt32(comboBox4.Text.ToString().Replace(":","")))
               {
                   Debug.WriteLine("Helllo");

                   listView1.Items.Add(
                        new ListViewItem(new[] { 
                            qresultName[i].Replace("\"", ""), qresultCity[i].Replace("\"", ""), qresultState[i],qresultStars[i],qresultID[i] }));
               }


                //listView1.Columns[0].Text = qresult[i].Replace("\"", "");
                // listView1.Items[i].SubItems.Add();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clear();
        }
        private void clear()
        {
            listView2.Items.Clear();
            listView1.Items.Clear();
            checkedListBox2.Items.Clear();
            checkedListBox3.Items.Clear();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
