<%@ Page Language="C#" MasterPageFile="DistributionWizard.master" AutoEventWireup="true" CodeFile="DistributionWizardChooseFolder.aspx.cs" Inherits="Community_DistributionWizard_DistributionWizardChooseFolder" Title="Distribution Wizard" %>
<%@ Register Src="SelectFolder.ascx" TagName="SelectFolder" TagPrefix="uc1" %>
<%@ MasterType VirtualPath="DistributionWizard.master" %>

<asp:Content ID="contentChooseFolder" ContentPlaceHolderID="cphDistributionWizardContent" Runat="Server">
    <div class="DistributionWizardErrorMessage"><asp:Label ID="lblErrorMessage" runat="server"></asp:Label></div>
    <div id="DistributionWizardFolderSelection">
        <input id="inputSelectedFolderID" runat="server" type="hidden" value="-1" />
        <script type="text/javascript" language="javascript">
            var selectedFolderIDHiddenFieldName = "<%= SelectedFolderIDHiddenFieldName %>";
            var currentFolderID = null;
            function selectFolder(folderID)
            {
                var hiddenField = document.getElementById(selectedFolderIDHiddenFieldName);
                if( hiddenField != null )
                {
                    hiddenField.value = folderID;
                }
                
                if( currentFolderID != null )
                {
                    var previousSelectedFolderAnchor = document.getElementById("ekFolderAnchor" + "sfSelectDestinationFolder_" + currentFolderID);
                    if( previousSelectedFolderAnchor != null )
                    {
                        previousSelectedFolderAnchor.className = "";
                    }
                }
                
                var newSelectedFolderAnchor = document.getElementById("ekFolderAnchor" + "sfSelectDestinationFolder_" + folderID);
                if( newSelectedFolderAnchor != null )
                {
                    newSelectedFolderAnchor.className = "DistributionWizardFolderSelectionSelectedFolder";
                }
                
                currentFolderID = folderID;
            }
        </script>
        <uc1:SelectFolder ID="sfSelectDestinationFolder" runat="server" />
    </div>
    <div id="DistributionWizardFooter"><asp:Button ID="btnBack" Text="Back" runat="server" OnClick="btnBack_Click" />&nbsp;<asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" />&nbsp;&nbsp;<asp:Button ID="btnDone" runat="server" Text="Done" Enabled="false" />&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" /></div>
</asp:Content>

