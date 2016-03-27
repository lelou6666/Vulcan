<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Questionnaire.aspx.cs" Inherits="Questionnaire" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="body_wrapper">
<div class="body_wrapper2">
	<div class="row">
    	<div class="row_padding" style="overflow:auto;">
            <br />
            <h1>Endurance&trade; Range Customizer</h1>
            <h3>Build the product that best fits the needs of your operations.</h3>
            <br />
            <p style="font-size:20px;">Need Help selecting the right range?</p>
            <p>Start by answering these simple questions and we'll start building one just for you.</p>
            <div style="color: #d4d3d3; font-family: 'helvetica', Arial, Sans-Serif; font-size: 16px; line-height: 22px;">
                <table cellspacing="0" border="0" style="border-collapse:collapse;">
                    <tr>
                        <td>
                            <span class="question">How much space do you have available in your operation for a new Vulcan Range?</span>
                            <table border="0">
                                <tr>
                                    <td><input type="radio" name="question_1" id="question_1_answer_1" value="24" /></td>
                                    <td><label for="question_1_answer_1">24"</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_1" id="question_1_answer_2" value="36" /></td>
                                    <td><label for="question_1_answer_2">36"</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_1" id="question_1_answer_3" value="48" /></td>
                                    <td><label for="question_1_answer_3">48"</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_1" id="question_1_answer_4" value="60" /></td>
                                    <td><label for="question_1_answer_4">60"</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_1" id="question_1_answer_5" value="72" /></td>
                                    <td><label for="question_1_answer_5">72"</label></td>
                                </tr>
                            </table>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="question">What volume of cooking/serving does your operation complete daily?</span>
                            <table border="0">
                                <tr>
                                    <td><input type="radio" name="question_2" id="question_2_answer_2" value="Medium" /></td>
                                    <td><label for="question_2_answer_2">Medium</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_2" id="question_2_answer_3" value="High" /></td>
                                    <td><label for="question_2_answer_3">High</label></td>
                                </tr>
                            </table>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="question">Based on frequency, which cooking method do you use most often?</span>
                            <table border="0">
                                <tr>
                                    <td><input type="radio" name="question_3" id="question_3_answer_1" value="BakingRoasting" /></td>
                                    <td><label for="question_3_answer_1">Baking &amp; roasting</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_3" id="question_3_answer_2" value="SearingSauteing" /></td>
                                    <td><label for="question_3_answer_2">Searing &amp; saut&eacute;ing</label></td>
                                </tr>
                                <tr>
                                    <td><input type="radio" name="question_3" id="question_3_answer_3" value="BoilingSimmeringBraising" /></td>
                                    <td><label for="question_3_answer_3">Boiling, simmering &amp; braising</label></td>
                                </tr>
                            </table>
                            <br />
                        </td>
                    </tr>
                </table>
                <p><br /><asp:ImageButton ID="btnSubmit" runat="server" ImageUrl="images/find_range.jpg" ValidationGroup="Group" onclick="btnSubmit_Click" /><br /><br /></p>
            </div>
       </div>
    </div>
</div>
</div>
</asp:Content>