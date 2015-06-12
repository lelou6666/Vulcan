using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class Quiz : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection("Server=FSTROYSQL1; Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=vulcancms;Password=Vulcan123");
    string Fname;
    string Lname;
    string Title;
    string EmailId;
    string Password;
    string Experience;
    string City;
    string State;
    string Zip;
    protected void Page_Load(object sender, EventArgs e)
    {
		string PhaseId = Request.QueryString["phase"];
		
        if (!IsPostBack)
        {
            Session["counter"] = "1";
            Session["TopQuestionId"] = null;
            
            int id = Convert.ToInt32(PhaseId);
            GetQuestionId(id);
            //int id = 1;
            int QuestionCount = GetQuestionCount(id);
            if (Convert.ToInt32(Session["counter"].ToString()) <= QuestionCount)
            {
                pnlQuestion.Visible = true;
                GetQuestion(Convert.ToInt32(Session["counter"].ToString()), id);
            }
        }
    }
    protected void btnNextQuestion_Click(object sender, EventArgs e)
    {        
        if (rblChoice.SelectedIndex >= 0)
        {
            lblError.Text = "";
            lblAnswer.Text = "";
            string UserId =  Session["UserId"].ToString();
			int phase =  Convert.ToInt32(Request.QueryString["phase"]);
            GetUserDetails(Convert.ToInt32(UserId));
            if (Fname != null && Lname != null && Title != null && EmailId != null && Experience != null && City != null && State != null && Zip != null)
            {
                string Question = lblQuestion1.Text;
                Question = Question.Replace("'", "''");
                string SelecttedAnswer = rblChoice.SelectedItem.Text;
                SelecttedAnswer = SelecttedAnswer.Replace("'", "''");
                string CorrectAnswer = hfCorrectAnswer.Value;
                CorrectAnswer = CorrectAnswer.Replace("'", "''");
				
                string TesiInsertQry = "insert into  TestResult (UserId, Firstname, LastName, Title, Email, Password, Experience, City, State, Zip, PhaseId, QuestionId, QuestionName, UserAnswer, CorrectAnswer, CreatedDate ) values (";
                TesiInsertQry += Convert.ToInt32(Session["UserId"].ToString()) + ",'" + Fname + "','" + Lname + "','" + Title + "','" + EmailId + "','" + Password + "','" + Experience + "','" + City + "','" + State + "','" + Zip + "', ";
                TesiInsertQry += phase + "," + Convert.ToInt32(hfQuestionId.Value) + ",'" + Question + "','" + SelecttedAnswer + "','" + CorrectAnswer + "','" + DateTime.Now.ToString() + "')";
                
                con.Open();
                SqlCommand cmd = new SqlCommand(TesiInsertQry, con);
                cmd.ExecuteNonQuery();
                con.Close();

                string value = Session["counter"].ToString();
                int count = Convert.ToInt32(value);
                count = count + 1;

                Session["counter"] = Convert.ToString(count);
                string PhaseId = Request.QueryString["phase"];
                int id = Convert.ToInt32(PhaseId);
                //int id = 1;
                int QuestionCount = GetQuestionCount(id);
                if (Convert.ToInt32(Session["counter"].ToString()) <= QuestionCount)
                {
                    GetQuestion(Convert.ToInt32(Session["counter"].ToString()), id);
                }
                else
                {                   
                    Response.Redirect("Result.aspx?Pid=" + id + "&Uid=" + UserId);
                }
            }            
        }
        else
        {
            pnlQuestion.Visible = true;
            lblError.Text = "Please Select an Answer";
        }       
    }
    protected void btnReTest_Click(object sender, EventArgs e)
    {
        string PhaseId = Request.QueryString["phase"];
        Response.Redirect("Quiz.aspx?phase=" + PhaseId);
    }
    protected void btnChkAnswer_Click(object sender, EventArgs e)
    {
        if (rblChoice.SelectedIndex >= 0)
        {
			lblError.Text = "";
			
            if (hfCorrectAnswer.Value == rblChoice.SelectedItem.Value)
            {
                lblUserAnswer.Text = rblChoice.SelectedItem.Text;
                lblCorrectAnswer.Text = hfCorrectAnswer.Value;
				lblReason.Text = hfReason.Value;
                Page page = HttpContext.Current.CurrentHandler as Page;
                if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("alert"))
                {
                    page.ClientScript.RegisterClientScriptBlock(page.GetType(), "alert", "showResult();", true /* addScriptTags */);
                }
            }
            else
            {
                lblUserAnswer.Text = rblChoice.SelectedItem.Text;
                lblCorrectAnswer.Text = hfCorrectAnswer.Value;
				lblReason.Text = hfReason.Value;
                Page page = HttpContext.Current.CurrentHandler as Page;
                if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("alert"))
                {
                    page.ClientScript.RegisterClientScriptBlock(page.GetType(), "alert", "showResult();", true /* addScriptTags */);
                }
            }
			
			rblChoice.Enabled = false;
        }
        else
        {
            pnlQuestion.Visible = true;
            lblError.Text = "Please Select an Answer";
        }       
    }  
    public void GetQuestion(int id, int Phase)
    {        
		rblChoice.Enabled = true;
	
        int TqId =0;
        if( Session["TopQuestionId"] != null)
        {
            TqId = Convert.ToInt32(Session["TopQuestionId"].ToString());
        }
        string qryQuestion = "select * from Questions where PhaseId=" + Phase + "and QuestionId=" + TqId;        
        TqId = TqId + 1;
        Session["TopQuestionId"] = TqId.ToString();
        con.Open();
        SqlDataAdapter sda = new SqlDataAdapter(qryQuestion, con);
        DataSet dsQuestion = new DataSet();
        sda.Fill(dsQuestion, "Question");        
        if (dsQuestion.Tables[0].Rows.Count > 0)
        {
            DataRow dtrQuestion;
            int i = 0;
            while (i < dsQuestion.Tables[0].Rows.Count)
            {
                dtrQuestion = dsQuestion.Tables[0].Rows[i];
                lblQuestion1.Text = dtrQuestion["QuestionName"].ToString();
				
				if(Phase == 1) {
					lblQuestionNum.Text = "Question " + Session["counter"].ToString() + " of 15";
				}
				else
				{
					lblQuestionNum.Text = "Question " + Session["counter"].ToString() + " of 15";
				}
				
                hfQuestionId.Value = dtrQuestion["QuestionId"].ToString();
                GetAnswer(Convert.ToInt32(dtrQuestion["QuestionId"]));
                GetCorrectAnswer(Convert.ToInt32(dtrQuestion["QuestionId"]));
                i++;
            }
        }
        con.Close();
		
		if(Phase == 1)
		{
			if(Convert.ToInt32(Session["counter"].ToString()) < 11){
				iRange.ImageUrl = "Images/endurance.jpg";
				lblHeading.Text = "Restaurant Ranges";
			}
			else
			{
				lblHeading.Text = "Heavy Duty";
				iRange.ImageUrl = "Images/range.jpg";
			}
		}
		else
		{
			iRange.ImageUrl = "Images/range.jpg";
			lblHeading.Text = "Restaurant Ranges";
		}
    }
    public void GetAnswer(int QuestionId)
    {
        string qryAns = "select * from answer where QuestionId="+QuestionId;        
        SqlDataAdapter sdaAnswer = new SqlDataAdapter(qryAns, con);
        DataSet dsAnswer = new DataSet();
        sdaAnswer.Fill(dsAnswer, "Answer");
        rblChoice.ClearSelection();
        rblChoice.Items.Clear();        
        ListItemCollection itemsCollection = rblChoice.Items;
        if (dsAnswer.Tables[0].Rows.Count > 0)
        {
            DataRow dtrAnswer;
            int j = 0;
            while (j < dsAnswer.Tables[0].Rows.Count)
            {
                dtrAnswer = dsAnswer.Tables[0].Rows[j];
                if (dtrAnswer["AnswerText"].ToString() != "")
                {
                    //itemsCollection.Add(new ListItem(dtrAnswer["AnswerText"].ToString(), dtrAnswer["Answer"].ToString()));
                    itemsCollection.Add(new ListItem(dtrAnswer["AnswerText"].ToString(), (j+1).ToString()));
                }
                j++;
            }
        }         
    }
    public void GetCorrectAnswer(int QuestionId)
    {
        string qryCorrectAnswer = "select * from answer where QuestionId=" + QuestionId +" and Answer='True'";
        SqlDataAdapter sdaCorrectAnswer = new SqlDataAdapter(qryCorrectAnswer, con);
        DataSet dsCorrectAnswer = new DataSet();
        sdaCorrectAnswer.Fill(dsCorrectAnswer, "Answer");
        if (dsCorrectAnswer.Tables[0].Rows.Count > 0 && dsCorrectAnswer.Tables[0].Rows.Count == 1)
        {
            DataRow dtrCorrectAnswer;
			
            int j = 0;
            while (j < dsCorrectAnswer.Tables[0].Rows.Count)
            {
                dtrCorrectAnswer = dsCorrectAnswer.Tables[0].Rows[j];
                hfCorrectAnswer.Value = dtrCorrectAnswer["AnswerText"].ToString();
				hfReason.Value = dtrCorrectAnswer["AnswerExpanded"].ToString();
                j++;
            }
        }
    }
    public int GetQuestionCount(int PhaseId)
    {
        string qryQuestionCount = "select * from questions where PhaseId=" + PhaseId;
        SqlDataAdapter sdaCount = new SqlDataAdapter(qryQuestionCount, con);
        DataSet dsCount = new DataSet();
        sdaCount.Fill(dsCount, "Count");
        if (dsCount.Tables[0].Rows.Count > 0)
        {
            return dsCount.Tables[0].Rows.Count;
        }
        else
        {
            return 0;
        }
    }
    public void GetUserDetails(int UserId)
    {
        string qryUserDetails = "Select * from users where Userid=" + UserId;
        SqlDataAdapter sdaUser = new SqlDataAdapter(qryUserDetails, con);
        DataSet dsUser = new DataSet();
        sdaUser.Fill(dsUser, "UDetail");
        if (dsUser.Tables[0].Rows.Count > 0 && dsUser.Tables[0].Rows.Count == 1)
        {
            DataRow dtrUser;
            int i = 0;
            while(i<dsUser.Tables[0].Rows.Count)
            {
                dtrUser = dsUser.Tables[0].Rows[i];
                if (dtrUser["FirstName"] != null)
                {
                    Fname = dtrUser["FirstName"].ToString();
                }
                if (dtrUser["LastName"] != null)
                {
                    Lname = dtrUser["LastName"].ToString();
                }               
                if (dtrUser["Title"] != null)
                {
                    Title = dtrUser["Title"].ToString();
                }
                if (dtrUser["Email"] != null)
                {
                    EmailId = dtrUser["Email"].ToString();
                }
                if (dtrUser["Password"] != null)
                {
                    Password = dtrUser["Password"].ToString();
                }
                if (dtrUser["Experience"] != null)
                {
                    Experience = dtrUser["Experience"].ToString();
                }
                if (dtrUser["City"] != null)
                {
                    City = dtrUser["City"].ToString();
                }
                if (dtrUser["State"] != null)
                {
                    State = dtrUser["State"].ToString();
                }
                if (dtrUser["Zip"] != null)
                {
                    Zip = dtrUser["Zip"].ToString();
                }
                i++;
            }
        }
    }
    public void GetQuestionId(int PhaseId)
    {
        string qryQuestionCount = "select top(1) * from questions where PhaseId=" + PhaseId;
        SqlDataAdapter sdaCount = new SqlDataAdapter(qryQuestionCount, con);
        DataSet dsCount = new DataSet();
        sdaCount.Fill(dsCount, "Count");
        if (dsCount.Tables[0].Rows.Count > 0)
        {
            DataRow dtrAnswer;
            int j = 0;
            while (j < dsCount.Tables[0].Rows.Count)
            {
                dtrAnswer = dsCount.Tables[0].Rows[j];
                if (dtrAnswer["QuestionId"].ToString() != "")
                {
                    Session["TopQuestionId"] = dtrAnswer["QuestionId"].ToString();
                }               
                j++;
            }
        }      
    }
}