using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Vulcan.Product.Lib
{
    public static class SharepointHelper
    {
        //The purpose of this static helper class is to encapsulate
        //the communication with the SharePoint server for Hobart to
        //retrieve the product spec documentation

        public struct URLInfo
        {
            public string title;
            public string url;
        }

        private const string xmlDocQuery =
                                    " <Where> " +
                                    "      <Contains>" +
                                    "        <FieldRef Name=\"Modles\" />" +
                                    "        <Value Type=\"Text\">{0}</Value>" +
                                    "      </Contains>" +
                                    " </Where>";
                                    
                                    
        public static string GetURL(string model_number)
        {
           
            XmlDocument xDoc = new XmlDocument();
            XmlNode xQuery = xDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode xViewFields = xDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode xQueryOptions = xDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");
            xQuery.InnerXml = string.Format(xmlDocQuery, model_number);
            XmlNode xResults;


            //Add the Sharepoint Web Service reference: Todo put link in config file
            Hobart.Sharepoint.Lists l = new Hobart.Sharepoint.Lists();
            //l.Url = "https://my.hobartcorp.com/ResourceCenter/_vti_bin/lists.asmx";
			l.Url = "https://my.vulcanfeg.com/ResourceCenter/_vti_bin/lists.asmx";
            //l.Credentials = System.Net.CredentialCache.DefaultCredentials;
            l.Credentials = new System.Net.NetworkCredential("MarrinerSpecSheets", "hobart1", "ext");

            //Run query
            xResults = l.GetListItems("{8DBC2A9D-3880-4608-8B2F-116B1571B4F8}", string.Empty, xQuery, xViewFields, "0", xQueryOptions, string.Empty);

            System.IO.StringReader sr = new System.IO.StringReader(xResults.OuterXml.ToString());

            DataSet ds = new DataSet();
            DataTable dt;
            DataRow dr;

            ds.ReadXml(sr);
            dt = ds.Tables["row"];
            if (dt == null)
            {
                return "";
            }

            dr = dt.Rows[0];

            string strTitle = dr["ows_Title"].ToString();
            string strUrl = dr["ows_EncodedAbsUrl"].ToString();

            URLInfo uinfo = new URLInfo();
            uinfo.title = strTitle;
            uinfo.url = strUrl;

            return uinfo.url;
        }     
        
        public static DataTable GetGrid(string model_number)
        {
           
            XmlDocument xDoc = new XmlDocument();
            XmlNode xQuery = xDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode xViewFields = xDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode xQueryOptions = xDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");
            xQuery.InnerXml = string.Format(xmlDocQuery, model_number);
            XmlNode xResults;


            //Add the Sharepoint Web Service reference: Todo put link in config file
            Hobart.Sharepoint.Lists l = new Hobart.Sharepoint.Lists();
            //l.Url = "https://my.hobartcorp.com/ResourceCenter/_vti_bin/lists.asmx";
			l.Url = "https://my.vulcanfeg.com/ResourceCenter/_vti_bin/lists.asmx";
            //l.Credentials = System.Net.CredentialCache.DefaultCredentials;
            l.Credentials = new System.Net.NetworkCredential("MarrinerSpecSheets", "hobart1", "ext");

            //Run query
            xResults = l.GetListItems("{8DBC2A9D-3880-4608-8B2F-116B1571B4F8}", string.Empty, xQuery, xViewFields, "0", xQueryOptions, string.Empty);

            System.IO.StringReader sr = new System.IO.StringReader(xResults.OuterXml.ToString());

            DataSet ds = new DataSet();
            DataTable dt;
            DataRow dr;

            ds.ReadXml(sr);
            dt = ds.Tables["row"];
            
            return dt;
        }                        

        public static void GetModelData(string model_number)
        {
        /*
            XmlDocument xDoc = new XmlDocument();
            XmlNode xQuery = xDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode xViewFields = xDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode xQueryOptions = xDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");
            xQuery.InnerXml = string.Format(xmlDocQuery, model_number);
            XmlNode xResults;
            List<string> modelData  = new List<string>;


            //Add the Sharepoint Web Service reference: Todo put link in config file
            Hobart.Sharepoint.Lists l = new Hobart.Sharepoint.Lists();
            l.Url = "https://my.hobartcorp.com/ResourceCenter/_vti_bin/lists.asmx";
            //l.Credentials = System.Net.CredentialCache.DefaultCredentials;
            l.Credentials = new System.Net.NetworkCredential("MarrinerSpecSheets", "hobart1", "ext");

            //Run query
            xResults = l.GetListItems("{8DBC2A9D-3880-4608-8B2F-116B1571B4F8}", string.Empty, xQuery, xViewFields, "0", xQueryOptions, string.Empty);

            System.IO.StringReader sr = new System.IO.StringReader(xResults.OuterXml.ToString());

            DataSet ds = new DataSet();
            DataTable dt;
            DataRow dr;

            ds.ReadXml(sr);
            dt = ds.Tables["row"];
            if (dt == null)
            {
                return new URLInfo();
            }

            dr = dt.Rows[0];
            
            for(int i = 0; i < dt.Rows; i++)
            {
				string strTitle = dr["ows_Document Type"].ToString();
				string strUrl = dr["ows_EncodedAbsUrl"].ToString();
				
				if(strTitle == "Spec Sheet"
            }

            string strTitle = dr["ows_Document Type"].ToString();
            string strUrl = dr["ows_EncodedAbsUrl"].ToString();

            URLInfo uinfo = new URLInfo();
            uinfo.title = strTitle;
            uinfo.url = strUrl;
            
            

            return modelData;
            */
        }
    }
}
