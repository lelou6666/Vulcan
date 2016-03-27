<?xml version="1.0" encoding="utf-8"?>
<%@ Register TagPrefix="cms" Namespace="Ektron.Cms.Controls" Assembly="Ektron.Cms.Controls" %>
<%@ Page ContentType="text/xml" Language="vb" AutoEventWireup="false" Inherits="cmsdatalist" CodeFile="cmsdatalist.aspx.vb" %>
<asp:Literal id="CmsDataListXml" runat="server"></asp:Literal>
<cms:ContentBlock id="CmsDataList" runat="server" DynamicParameter="id"></cms:ContentBlock>
