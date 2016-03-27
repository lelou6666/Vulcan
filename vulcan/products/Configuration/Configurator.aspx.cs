using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class Configurator : System.Web.UI.Page
{
	int size = 0;
	string strImage = "";
	string strName = "";
	int[] burners;
	string[] gasOptions = new string[] {"Natural","LP"};
	string[] ovenOptions;
	string[] panOrientations;
	string[] griddleOptions;
	string[] salamanderOptions = new string[] {"None","Infrared Salamander Broiler","Radiant Salamander Broiler"};
			
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            size = Convert.ToInt32(Request.QueryString["Size"]);
            switch (size) {
				case 24:
					strImage = "24inch.png";
					strName = "24&quot; Range (4 Burners)";
					burners = new int[] {4};
					ovenOptions = new string[] {"Standard"};
					panOrientations = new string[] {"Front to Back"};
					griddleOptions = new string[] {"No Griddle"};
				break;
				case 36:
					strImage = "36inch.png";
					strName = "36&quot; Range";
					burners = new int[] {0,2,6};
					ovenOptions = new string[] {"Standard","Convection"};
					panOrientations = new string[] {"Front to Back","Side to Side","Front to Back & Side to Side"};
					griddleOptions = new string[] {"No Griddle","24&quot; Griddle, Right, Manual Controls","24&quot; Griddle, Right, Thermostatic Controls","36&quot; Griddle, Right, Manual Controls","36&quot; Griddle, Right, Thermostatic Controls","24&quot; Broiler, Right, Thermostatic Controls"};
				break;
				case 48:
					strImage = "48inch.png";
					strName = "48&quot; Range";
					burners = new int[] {2,4,8};
					ovenOptions = new string[] {"Standard","Convection"};
					panOrientations = new string[] {"Front to Back","Front to Back & Side to Side"};
					griddleOptions = new string[] {"No Griddle","24&quot; Griddle, Right, Manual Controls","24&quot; Griddle, Right, Thermostatic Controls","36&quot; Griddle, Right, Manual Controls","36&quot; Griddle, Right, Thermostatic Controls"};
				break;
				case 60:
					strImage = "60inch_v2.png";
					strName = "60&quot; Range";
					burners = new int[] {4,6,10};
					ovenOptions = new string[] {"Standard","Convection"};
					panOrientations = new string[] {"Front to Back","Side to Side","Front to Back & Side to Side"};
					griddleOptions = new string[] {"No Griddle","24&quot; Griddle, Right, Manual Controls","24&quot; Griddle, Right, Thermostatic Controls","36&quot; Griddle, Right, Manual Controls","36&quot; Griddle, Right, Thermostatic Controls","24&quot; Broiler, Right, Manual Controls"};
				break;
				case 72:
					strImage = "72inch_v2.png";
					strName = "72&quot; Range";
					burners = new int[] {6,8,12};
					ovenOptions = new string[] {"Standard","Convection"};
					panOrientations = new string[] {"Front to Back & Side to Side"};
					griddleOptions = new string[] {"No Griddle","24&quot; Griddle, Right, Manual Controls","24&quot; Griddle, Right, Thermostatic Controls","24&quot; Broiler, Right, Thermostatic Controls", "36&quot; Griddle, Right, Manual Controls","36&quot; Griddle, Right, Thermostatic Controls"};
				break;
			}

			//Populate product name and image
			ProductName.Text = strName;
			ProductImage.ImageUrl = "Images/" + strImage;

			//Populate checkbox lists
            FillCheckBoxList(cblOven,"Oven");
            FillCheckBoxList(cblGas,"Gas");
            FillCheckBoxList(cblBurners,"Burner");
            FillCheckBoxList(cblSheet,"Sheet");
            FillCheckBoxList(cblGriddle,"Griddle");
			FillCheckBoxList(cblSalamander,"Salamander");
			
            //Hide checkboxes & panels
			cbOven.Visible = false;
            cbGas.Visible = false;
            cbBurner.Visible = false;
            cbSheet.Visible = false;
            cbGriddle.Visible = false;
			Panel2.Visible = false;
            Panel3.Visible = false;
            Panel4.Visible = false;
            Panel5.Visible = false;
			Panel6.Visible = false;
        }   
    }
    protected void Back2_Click(object sender, ImageClickEventArgs e)
    {
		Arrow1.ImageUrl = "Images/step1_on.png";
		Arrow2.ImageUrl = "Images/step2.png";
        Panel1.Visible = true;
        Panel2.Visible = false;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
    }
    protected void Back3_Click(object sender, ImageClickEventArgs e)
    {
        Arrow2.ImageUrl = "Images/step2_on.png";
		Arrow3.ImageUrl = "Images/step3.png";
		Panel2.Visible = true;
        Panel3.Visible = false;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;
    }
    protected void Back4_Click(object sender, ImageClickEventArgs e)
    {
        Arrow3.ImageUrl = "Images/step3_on.png";
		Arrow4.ImageUrl = "Images/step4.png";
		Panel3.Visible = true;
        Panel4.Visible = false;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;
        cbBurner.Visible = true;
        cbBurner.Enabled = false;
        if (cblBurners.Items.Count > 0)
        {
            cbBurner.Text = cblBurners.SelectedItem.Text;
        }
        else
        {
            cbBurner.Text = "Not Available for this Product";
        }
        cbBurner.Checked = true;
    }
    protected void Back5_Click(object sender, ImageClickEventArgs e)
    {
        Arrow4.ImageUrl = "Images/step4_on.png";
		Arrow5.ImageUrl = "Images/step5.png";
		Panel4.Visible = true;
        Panel5.Visible = false;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;
        cbBurner.Visible = true;
        cbBurner.Enabled = false;
        if (cblBurners.Items.Count > 0)
        {
            cbBurner.Text = cblBurners.SelectedItem.Text;
        }
        else
        {
            cbBurner.Text = "Not Available for this Product";
        }
        cbBurner.Checked = true;
        cbSheet.Visible = true;
        cbSheet.Enabled = false;
        if (cblSheet.Items.Count > 0)
        {
            cbSheet.Text = cblSheet.SelectedItem.Text;
        }
        else
        {
            cbSheet.Text = "Not Available for this Product";
        }
        cbSheet.Checked = true;
    }
	protected void Back6_Click(object sender, ImageClickEventArgs e)
    {
		Arrow5.ImageUrl = "Images/step5_on.png";
		Arrow6.ImageUrl = "Images/step6.png";
		Panel5.Visible = true;
        Panel6.Visible = false;
        cbGriddle.Visible = true;
        cbGriddle.Enabled = false;
		if (cblGriddle.Items.Count > 0)
        {
            cbGriddle.Text = cblGriddle.SelectedItem.Text;
        }
        else
        {
            cbGriddle.Text = "Not Available for this Product";
        }
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;
        cbBurner.Visible = true;
        cbBurner.Enabled = false;
        if (cblBurners.Items.Count > 0)
        {
            cbBurner.Text = cblBurners.SelectedItem.Text;
        }
        else
        {
            cbBurner.Text = "Not Available for this Product";
        }
        cbBurner.Checked = true;
        cbSheet.Visible = true;
        cbSheet.Enabled = false;
        if (cblSheet.Items.Count > 0)
        {
            cbSheet.Text = cblSheet.SelectedItem.Text;
        }
        else
        {
            cbSheet.Text = "Not Available for this Product";
        }
        cbSheet.Checked = true;
	}
    protected void Next1_Click(object sender, ImageClickEventArgs e)
    {
        Arrow1.ImageUrl = "Images/step1.png";
		Arrow2.ImageUrl = "Images/step2_on.png";
		Panel1.Visible = false;
        Panel2.Visible = true;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
    }
    protected void Next2_Click(object sender, ImageClickEventArgs e)
    {
        Arrow2.ImageUrl = "Images/step2.png";
		Arrow3.ImageUrl = "Images/step3_on.png";
		Panel2.Visible = false;
        Panel3.Visible = true;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;    
    }
    protected void Next3_Click(object sender, ImageClickEventArgs e)
    {
        Arrow3.ImageUrl = "Images/step3.png";
		Arrow4.ImageUrl = "Images/step4_on.png";
		Panel3.Visible = false;
        Panel4.Visible = true;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;
        cbBurner.Visible = true;
        cbBurner.Enabled = false;
        if (cblBurners.Items.Count > 0)
        {
            cbBurner.Text = cblBurners.SelectedItem.Text;
			
			//Selectively enable/disable griddle options
			size = Convert.ToInt32(Request.QueryString["Size"]);
            switch (size) {
				case 36:
					for (int i = 0; i < cblGriddle.Items.Count; i++) {
						cblGriddle.Items[i].Enabled = false;
					}
					switch (cblBurners.SelectedItem.Text) {
						case "0":
							cblGriddle.Items[3].Enabled = true;
							cblGriddle.Items[3].Selected = true;
							cblGriddle.Items[4].Enabled = true;
						break;
						case "2":
							cblGriddle.Items[1].Enabled = true;
							cblGriddle.Items[1].Selected = true;
							cblGriddle.Items[2].Enabled = true;
							cblGriddle.Items[5].Enabled = true;
						break;
						case "6":
							cblGriddle.Items[0].Enabled = true;
						break;
					}
				break;
				case 48:
					for (int i = 0; i < cblGriddle.Items.Count; i++) {
						cblGriddle.Items[i].Enabled = false;
					}
					switch (cblBurners.SelectedItem.Text) {
						case "2":
							cblGriddle.Items[3].Enabled = true;
							cblGriddle.Items[3].Selected = true;
							cblGriddle.Items[4].Enabled = true;
						break;
						case "4":
							cblGriddle.Items[1].Enabled = true;
							cblGriddle.Items[1].Selected = true;
							cblGriddle.Items[2].Enabled = true;
						break;
						case "8":
							cblGriddle.Items[0].Enabled = true;
						break;
					}
				break;
				case 60:
					for (int i = 0; i < cblGriddle.Items.Count; i++) {
						cblGriddle.Items[i].Enabled = false;
					}
					switch (cblBurners.SelectedItem.Text) {
						case "4":
							cblGriddle.Items[3].Enabled = true;
							cblGriddle.Items[3].Selected = true;
							cblGriddle.Items[4].Enabled = true;
						break;
						case "6":
							cblGriddle.Items[1].Enabled = true;
							cblGriddle.Items[1].Selected = true;
							cblGriddle.Items[2].Enabled = true;
							cblGriddle.Items[5].Enabled = true;
						break;
						case "10":
							cblGriddle.Items[0].Enabled = true;
						break;
					}
				break;
				case 72:
					for (int i = 0; i < cblGriddle.Items.Count; i++) {
						cblGriddle.Items[i].Enabled = false;
					}
					switch (cblBurners.SelectedItem.Text) {
						case "6":
							cblGriddle.Items[3].Enabled = true;
							cblGriddle.Items[3].Selected = true;
							cblGriddle.Items[4].Enabled = true;
						break;
						case "8":
							cblGriddle.Items[1].Enabled = true;
							cblGriddle.Items[1].Selected = true;
							cblGriddle.Items[2].Enabled = true;
						break;
						case "12":
							cblGriddle.Items[0].Enabled = true;
						break;
					}
				break;
			}
        }
        else
        {
            cbBurner.Text = "Not Available for this Product";
        }
        cbBurner.Checked = true;      
    }
    protected void Next4_Click(object sender, ImageClickEventArgs e)
    {
        Arrow4.ImageUrl = "Images/step4.png";
		Arrow5.ImageUrl = "Images/step5_on.png";
		Panel4.Visible = false;
        Panel5.Visible = true;
        cbOven.Visible = true;
        cbOven.Enabled = false;
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        cbOven.Checked = true;
        cbGas.Visible = true;
        cbGas.Enabled = false;
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        cbGas.Checked = true;
        cbBurner.Visible = true;
        cbBurner.Enabled = false;
        if (cblBurners.Items.Count > 0)
        {
            cbBurner.Text = cblBurners.SelectedItem.Text;
        }
        else
        {
            cbBurner.Text = "Not Available for this Product";
        }
        cbBurner.Checked = true;
        cbSheet.Visible = true;
        cbSheet.Enabled = false;
        if (cblSheet.Items.Count > 0)
        {
            cbSheet.Text = cblSheet.SelectedItem.Text;
        }
        else
        {
            cbSheet.Text = "Not Available for this Product";
        }
        cbSheet.Checked = true;       
    }
	
	protected void Next5_Click(object sender, ImageClickEventArgs e)
    { 
		Arrow5.ImageUrl = "Images/step5.png";
		Arrow6.ImageUrl = "Images/step6_on.png";
		Panel5.Visible = false;
        Panel6.Visible = true;
		
		cbGriddle.Visible = true;
        cbGriddle.Enabled = false;
        if (cblGriddle.Items.Count > 0)
        {
            cbGriddle.Text = cblGriddle.SelectedItem.Text;
        }
        else
        {
            cbGriddle.Text = "Not Available for this Product";
        }
        cbGriddle.Checked = true;  
	}
    protected void Finish_Click(object sender, ImageClickEventArgs e)
    {        
        if (cblOven.Items.Count > 0)
        {
            cbOven.Text = cblOven.SelectedItem.Text;
        }
        else
        {
            cbOven.Text = "Not Available for this Product";
        }
        if (cblGas.Items.Count > 0)
        {
            cbGas.Text = cblGas.SelectedItem.Text;
        }
        else
        {
            cbGas.Text = "Not Available for this Product";
        }
        if (cblBurners.Items.Count > 0)
        {
            cbBurner.Text = cblBurners.SelectedItem.Text;
        }
        else
        {
            cbBurner.Text = "Not Available for this Product";
        }
        if (cblSheet.Items.Count > 0)
        {
            cbSheet.Text = cblSheet.SelectedItem.Text;
        }
        else
        {
            cbSheet.Text = "Not Available for this Product";
        }
        if (cblGriddle.Items.Count > 0)
        {
            cbGriddle.Text = cblGriddle.SelectedItem.Text;
        }
        else
        {
            cbGriddle.Text = "Not Available for this Product";
        }
		if (cblSalamander.Items.Count > 0)
        {
            cbSalamander.Text = cblSalamander.SelectedItem.Text;
        }
        else
        {
            cbSalamander.Text = "Not Available for this Product";
        }
		size = Convert.ToInt32(Request.QueryString["Size"]);
        Response.Redirect("Result.aspx?" + "Size=" + size + "&oven=" + cbOven.Text.Replace("&","-") + "&gas=" + cbGas.Text.Replace("&","-") + "&burners=" + cbBurner.Text.Replace("&","-") + "&pan=" + cbSheet.Text.Replace("&","-") + "&griddle=" + cbGriddle.Text.Replace("&","-") + "&salamander=" + cbSalamander.Text.Replace("&","-"),false);
    }
    public void FillCheckBoxList(RadioButtonList cbl, string type)
    {
        switch (type) {
			case "Oven":
				if (ovenOptions.Length == 0) {
					lblovenEmpty.Text = "No option to select for this Product";
				} else {
					for (int i = 0; i < ovenOptions.Length; i++) {
						ListItem li = new ListItem();
						if (i == 0) {
							li.Selected = true;
						}
						li.Text = ovenOptions[i];
						li.Value = ovenOptions[i];
						cbl.Items.Add(li);
					}
				}
			break;
			case "Gas":
				if (gasOptions.Length == 0) {
					lblGasEmpty.Text = "No option to select for this Product";
				} else {
					for (int i = 0; i < gasOptions.Length; i++) {
						ListItem li = new ListItem();
						if (i == 0) {
							li.Selected = true;
						}
						li.Text = gasOptions[i];
						li.Value = gasOptions[i];
						cbl.Items.Add(li);
					}
				}
			break;
			case "Burner":
				if (burners.Length == 0) {
					lblBurnerEmpty.Text = "No option to select for this Product";
				} else {
					for (int i = 0; i < burners.Length; i++) {
						ListItem li = new ListItem();
						if (i == 0) {
							li.Selected = true;
						}
						li.Text = burners[i].ToString();
						li.Value = burners[i].ToString();
						cbl.Items.Add(li);
					}
				}
			break;
			case "Sheet":
				if (panOrientations.Length == 0) {
					lblSheetEmpty.Text = "No option to select for this Product";
				} else {
					for (int i = 0; i < panOrientations.Length; i++) {
						ListItem li = new ListItem();
						if (i == 0) {
							li.Selected = true;
						}
						li.Text = panOrientations[i];
						li.Value = panOrientations[i];
						cbl.Items.Add(li);
					}
				}
			break;
			case "Griddle":
				if (griddleOptions.Length == 0) {
					lblGriddleEmpty.Text = "No option to select for this Product";
				} else {
					for (int i = 0; i < griddleOptions.Length; i++) {
						ListItem li = new ListItem();
						if (i == 0) {
							li.Selected = true;
						}
						li.Text = griddleOptions[i];
						li.Value = griddleOptions[i];
						cbl.Items.Add(li);
					}
				}
			break;
			case "Salamander":
				if (salamanderOptions.Length == 0) {
					lblSalamanderEmpty.Text = "No option to select for this Product";
				} else {
					for (int i = 0; i < salamanderOptions.Length; i++) {
						ListItem li = new ListItem();
						if (i == 0) {
							li.Selected = true;
						}
						li.Text = salamanderOptions[i];
						li.Value = salamanderOptions[i];
						cbl.Items.Add(li);
					}
				}
			break;
		}
    }
}