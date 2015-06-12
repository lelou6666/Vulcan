using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;

public partial class Result : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection("Server=FSTROYSQL1; Initial Catalog=Vulcan_Product_Database;Persist Security Info=True;User ID=vulcancms;Password=Vulcan123");
    string UserName;
    string Date;
	string email = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        string PhaseId = Request.QueryString["Pid"];
        string UserId = Request.QueryString["Uid"];
        int Pid = Convert.ToInt32(PhaseId);
        int Uid = Convert.ToInt32(UserId);
        int QuestionCount = GetQuestionCount(Pid);
        int TotalResult = getTotalResult(Pid, Uid);
        GetUserName(Uid);
        lblPhase.Text = "Boelter Range Training: Phase " + PhaseId + " Complete";
		
		if(PhaseId == "2"){
			btnPhaseinside2.ImageUrl = "images/retake2.jpg";
		}
		else
		{
			btnPhaseinside2.ImageUrl = "images/phase2.jpg";
		}

        if (UserName != null)
        {
            lblUsername.Text = UserName;
        }
        if (Date != null)
        {
            DateTime dt = Convert.ToDateTime(Date);
            //var dtalone = dt.Date;
            //dateAndTime.ToString("dd/MM/yyyy")
            lblDate.Text = dt.ToString("MM/dd/yyyy");
        }
        lblResult.Text = "You successfully answered "+TotalResult+" of "+QuestionCount+" questions.";
		
		if (!IsPostBack)
        {
			Ektron.Cms.CommonApi mailformat = new Ektron.Cms.CommonApi();
			Ektron.Cms.Common.EkMailService mailclient = mailformat.EkMailRef;
			string message = "";
	
			message = "Vulcan Range Quiz " + lblPhase.Text + " by " + UserName + " (" + email + ") on " +  lblDate.Text + ". " + UserName
					   + " successfully answered " + TotalResult + " of " + QuestionCount + " questions.  Visit http://vulcanequipment.com/vulcan/learningcenter/admin.aspx to view and export detailed quiz results.";
		
			mailclient.MailFrom = "admin@vulcanequipment.com";
			mailclient.MailSubject = "Vulcan Range Quiz " + lblPhase.Text + " by " + UserName;
			mailclient.MailBodyText = message;
			//mailclient.MailTo = "renee.m.haas@gmail.com";
			mailclient.MailTo = "CHRIS.STERN@vulcanhart.com";
	        
			mailclient.SendMail();
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
    public int getTotalResult(int PhaseId, int UserId)
    {
        string Qryresultcount = "select * from testresult where PhaseId=" + PhaseId + " and Userid=" + UserId + " and UserAnswer=CorrectAnswer";
        SqlDataAdapter sdaResultCount = new SqlDataAdapter(Qryresultcount, con);
        DataSet dsResultCount = new DataSet();
        sdaResultCount.Fill(dsResultCount, "ResultCount");
        if (dsResultCount.Tables[0].Rows.Count > 0)
        {            
            return dsResultCount.Tables[0].Rows.Count;
        }
        else
        {
            return 0;
        }
    }
    public void GetUserName(int UserId)
    {
        string QryUserDetails = "select * from testresult where UserId=" + UserId;
        SqlDataAdapter sdaUserDetail = new SqlDataAdapter(QryUserDetails, con);
        DataSet dsUserDetail = new DataSet();
        sdaUserDetail.Fill(dsUserDetail, "UserDetail");
        if (dsUserDetail.Tables[0].Rows.Count > 0)
        {
            DataRow dtrUserDetail;
            int j = 0;
            while (j < dsUserDetail.Tables[0].Rows.Count)
            {
                dtrUserDetail = dsUserDetail.Tables[0].Rows[j];
                if (dtrUserDetail["FirstName"].ToString() != null)
                {
                    UserName = dtrUserDetail["FirstName"].ToString();                   
                }
				if (dtrUserDetail["LastName"].ToString() != null)
                {
                    UserName += " " + dtrUserDetail["LastName"].ToString();                   
                }
                if (dtrUserDetail["CreatedDate"].ToString() != null)
                {
                    Date = dtrUserDetail["CreatedDate"].ToString();
                }
				
				if (dtrUserDetail["Email"].ToString() != null)
                {
                    email = dtrUserDetail["Email"].ToString();                   
                }
                j++;
            }
        }
    }
    protected void btnPhase1_Click(object sender, EventArgs e)
    {
		Response.Redirect("Registration.aspx?phase=1",false);
    }
    protected void btnPhase2_Click(object sender, EventArgs e)
    {
        //Response.Redirect("New.aspx", false);
		Response.Redirect("Registration.aspx?phase=2",false);
    }
}