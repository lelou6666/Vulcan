using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Ektron.Cms.Analytics.Reporting;
using Ektron.Cms.Common;
using System.Collections.Generic;
using System.Text;
using Ektron.Cms.Workarea.Reports;

public partial class controls_reports_TimeLineChart : WorkareaBaseControl
{
	public const int MaximumWidth = 1000;
	public const int MaximumHeight = 1000;
	public const int MaximumArea = 300000;

	public enum TimeUnit
	{
		Day,
		Week,
		Month
	}

	private DateTime _startDate1 = DateTime.Today;
	private ChartData _data1 = null;
	private DateTime _startDate2 = DateTime.Today;
	private ChartData _data2 = null;

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		TimeUnitInterval = TimeUnit.Day;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		const int DefaultWidth = 800;
		const int DefaultHeight = 200;

		if (Width.Type != UnitType.Pixel)
		{
			Width = new Unit(DefaultWidth, UnitType.Pixel);
		}
		if (Height.Type != UnitType.Pixel)
		{
			Height = new Unit(DefaultHeight, UnitType.Pixel);
		}
		if (Width.Value > MaximumWidth)
		{
			Width = new Unit(MaximumWidth, UnitType.Pixel);
		}
		if (Height.Value > MaximumHeight)
		{
			Height = new Unit(MaximumHeight, UnitType.Pixel);
		}
		if (Width.Value * Height.Value > MaximumArea)
		{
			Width = new Unit(DefaultWidth, UnitType.Pixel);
			Height = new Unit(DefaultHeight, UnitType.Pixel);
		}
		imgChart.Attributes.Add("width", Width.Value.ToString("0"));
		imgChart.Attributes.Add("height", Height.Value.ToString("0"));
		imgChart.AlternateText = BriefDescription;
		imgChart.Attributes.Add("title", imgChart.AlternateText);


		if (null == _data1)
		{
			_data1 = new ChartData(new List<double>());
		}

		double topScale1 = 0.0;
		double topScale2 = 0.0;
		string topScale1Label = "";
		string midScale1Label = "";
		string topScale2Label = "";
		string midScale2Label = "";
		if (null == _data2)
		{
			topScale1 = _data1.GetChartScale(_data1.Max());
			topScale2 = topScale1;
			midScale1Label = _data1.FormatChartLabel(topScale1 / 2);
			topScale1Label = _data1.FormatChartLabel(topScale1);
			midScale2Label = midScale1Label;
			topScale2Label = topScale1Label;
		}
		else
		{
			if (_data1.DataType == _data2.DataType)
			{
				// if both data sets are same type, use common topScale, otherwise use different top scales.
				double commonScale = Math.Max(_data1.Max(), _data2.Max());
				commonScale = _data1.GetChartScale(commonScale);
				topScale1 = commonScale;
				topScale2 = commonScale;
				midScale1Label = _data1.FormatChartLabel(commonScale / 2);
				topScale1Label = _data1.FormatChartLabel(commonScale);
				midScale2Label = midScale1Label;
				topScale2Label = topScale1Label;
			}
			else
			{
				topScale1 = _data1.GetChartScale(_data1.Max());
				midScale1Label = _data1.FormatChartLabel(topScale1 / 2);
				topScale1Label = _data1.FormatChartLabel(topScale1);
				topScale2 = _data2.GetChartScale(_data2.Max());
				midScale2Label = _data2.FormatChartLabel(topScale2 / 2);
				topScale2Label = _data2.FormatChartLabel(topScale2);
			}
		}

		int segmentGroupSize = 1;
		int dateGroupSize = 1;
		switch (TimeUnitInterval)
		{
			case TimeUnit.Day: // group by week
				segmentGroupSize = 7;
				dateGroupSize = 7;
				break;
			case TimeUnit.Week: // group by week
				segmentGroupSize = 1;
				dateGroupSize = 7;
				break;
			case TimeUnit.Month: // group by month
				segmentGroupSize = 1;
				dateGroupSize = 1;
				break;
			default:
				throw new ArgumentOutOfRangeException("TimeUnitInterval", TimeUnitInterval.ToString(), "Unknown value.");
		}
		int numSegments = _data1.Count - 1;
		if (_data2 != null)
		{
			int numSegments2 = _data2.Count - 1;
			if (numSegments < numSegments2)
			{
				_data1.Pad(numSegments2 - numSegments);
				numSegments = numSegments2;
			}
			else if (numSegments2 < numSegments)
			{
				_data2.Pad(numSegments - numSegments2);
			}
		}
		if (numSegments < 1) numSegments = 1;
		// allow at least 100 pixels per segment label
		int groupMultiplier = (int)Math.Ceiling(((double)numSegments * 100.0) / ((double)segmentGroupSize * Width.Value));
		if (groupMultiplier > 1)
		{
			dateGroupSize *= groupMultiplier;
			segmentGroupSize *= groupMultiplier;
		}
		string segmentLabels1 = "";
		DateTime date1 = _startDate1;
		string segmentLabels2 = "";
		DateTime date2 = _startDate2;
		bool splitData = (_data2 != null && _startDate1 != _startDate2);
		System.Globalization.CultureInfo culture;
		if (CommonApi.UserId > 0)
		{
			culture = new System.Globalization.CultureInfo(CommonApi.UserLanguage);
		}
		else
		{
			culture = new System.Globalization.CultureInfo(CommonApi.RequestInformationRef.DefaultContentLanguage);
		}

		if (TimeUnitInterval == TimeUnit.Month)
		{
			for (int i = 0; i <= numSegments; i += segmentGroupSize)
			{
				//segmentLabels1 += "|" + date1.ToString("Y", culture); // e.g., August, 2009
				segmentLabels1 += "|" + date1.ToString(culture.DateTimeFormat.ShortDatePattern);
				date1 = date1.AddMonths(dateGroupSize);
				if (splitData)
				{
					//segmentLabels2 += "|" + date2.ToString("Y", culture); // e.g., August, 2009
					segmentLabels2 += "|" + date2.ToString(culture.DateTimeFormat.ShortDatePattern);
					date2 = date2.AddMonths(dateGroupSize);
				}
			}
		}
		else
		{
			for (int i = 0; i <= numSegments; i += segmentGroupSize)
			{
				//segmentLabels1 += "|" + date1.ToString("M", culture); // e.g., August 12
				segmentLabels1 += "|" + date1.ToString(culture.DateTimeFormat.ShortDatePattern);
				date1 = date1.AddDays(dateGroupSize);
				if (splitData)
				{
					//segmentLabels2 += "|" + date2.ToString("M", culture); // e.g., August 12
					segmentLabels2 += "|" + date2.ToString(culture.DateTimeFormat.ShortDatePattern);
					date2 = date2.AddDays(dateGroupSize);
				}
			}
		}

		// INFORMATION ABOUT GOOGLE CHART API:
		// simulated grid lines: chm=R,808080,0,0.5,0.501|r,808080,0,0.499,0.501
		// label position: chxp=0,10,35,75|1,25,35
		// dual x-axis (split data): chxt=x,y,r,x

		StringBuilder sbUrl = new StringBuilder(this.GoogleChartBaseUrl);
		sbUrl.Append("?cht=ls");
		sbUrl.AppendFormat("&chs={0}x{1}", Width.Value, Height.Value);
		sbUrl.Append("&chco=0077CC,FF9900&chls=4|2&chm=o,0077CC,0,-1,8|o,FF9900,1,-1,5");
		if (!splitData)
		{
			sbUrl.Append("|B,E6F2FA,0,0,0");
		}
		sbUrl.Append("&chxt=x,y,r");
		if (splitData)
		{
			sbUrl.Append(",x"); // second line of labels on x-axis
		}
		sbUrl.AppendFormat("&chxr=0,0,{0},{1}|1,0,100,50|2,0,100,50", numSegments, segmentGroupSize);
		if (splitData)
		{
			sbUrl.AppendFormat("|3,0,{0},{1}", numSegments, segmentGroupSize);
		}
		sbUrl.Append("&chxl=0:");
		sbUrl.Append(segmentLabels1);
		sbUrl.AppendFormat("|1:||{0}|{1}|2:||{2}|{3}", midScale1Label, topScale1Label, midScale2Label, topScale2Label);
		if (splitData)
		{
			// second line of labels on x-axis
			sbUrl.Append("|3:"); 
			sbUrl.Append(segmentLabels2);
		}
		sbUrl.AppendFormat("&chg={0:N},50&chd=s:", (segmentGroupSize * 100.0) / numSegments);
		sbUrl.Append(_data1.EncodeGoogleChartData(topScale1));
		if (_data2 != null)
		{
			sbUrl.Append(",");
			sbUrl.Append(_data2.EncodeGoogleChartData(topScale2));
			sbUrl.Append("&chxs=1,0055AA|2,DD7700");
			if (splitData)
			{
				sbUrl.Append("|0,0055AA|3,DD7700");
			}
		}

		imgChart.ImageUrl = sbUrl.ToString();

		base.Render(writer);
	}

	private string _briefDescription;
	public string BriefDescription
	{
		get { return _briefDescription; }
		set { _briefDescription = value; }
	}
    private Unit _width;
    public Unit Width
    {
        get { return _width; }
        set { _width = value; }
    }
    private Unit _height;
    public Unit Height
    {
        get { return _height; }
        set { _height = value; }
    }

	public string CssClass
	{
		get { return imgChart.CssClass; }
		set { imgChart.CssClass = value; }
	}

    private DateTime _startDate;
    public DateTime StartDate
    {
        get { return _startDate; }
        set { _startDate = value; }
    }
    private TimeUnit _timeUnitInterval;
    public TimeUnit TimeUnitInterval
    {
        get { return _timeUnitInterval; }
        set { _timeUnitInterval = value; }
    }

	public void LoadData(DateTime startDate, List<int> data)
	{
		_startDate1 = startDate;
		_data1 = ChartData.CreateChartData(data);
	}
	public void LoadData2(List<int> data)
	{
		if (null == data)
		{
			_data2 = null;
		}
		else
		{
			_startDate2 = _startDate1;
			_data2 = ChartData.CreateChartData(data);
		}
	}
	public void LoadData(DateTime startDate, List<float> data)
	{
		_startDate1 = startDate;
		_data1 = ChartData.CreateChartData(data);
	}
	public void LoadData2(List<float> data)
	{
		if (null == data)
		{
			_data2 = null;
		}
		else
		{
			_startDate2 = _startDate1;
			_data2 = ChartData.CreateChartData(data);
		}
	}
	public void LoadData(DateTime startDate, List<double> data)
	{
		_startDate1 = startDate;
		_data1 = ChartData.CreateChartData(data);
	}
	public void LoadData2(List<double> data)
	{
		if (null == data)
		{
			_data2 = null;
		}
		else
		{
			_startDate2 = _startDate1;
			_data2 = ChartData.CreateChartData(data);
		}
	}
	public void LoadData(DateTime startDate, List<TimeSpan> data)
	{
		_startDate1 = startDate;
		_data1 = ChartData.CreateChartData(data);
	}
	public void LoadData2(List<TimeSpan> data)
	{
		if (null == data)
		{
			_data2 = null;
		}
		else
		{
			_startDate2 = _startDate1;
			_data2 = ChartData.CreateChartData(data);
		}
	}
	public void LoadSplitData(DateTime startDate1, List<int> data1, DateTime startDate2, List<int> data2)
	{
		_startDate1 = startDate1;
		_data1 = ChartData.CreateChartData(data1);
		_startDate2 = startDate2;
		_data2 = ChartData.CreateChartData(data2);
	}
	public void LoadSplitData(DateTime startDate1, List<float> data1, DateTime startDate2, List<float> data2)
	{
		_startDate1 = startDate1;
		_data1 = ChartData.CreateChartData(data1);
		_startDate2 = startDate2;
		_data2 = ChartData.CreateChartData(data2);
	}
	public void LoadSplitData(DateTime startDate1, List<double> data1, DateTime startDate2, List<double> data2)
	{
		_startDate1 = startDate1;
		_data1 = ChartData.CreateChartData(data1);
		_startDate2 = startDate2;
		_data2 = ChartData.CreateChartData(data2);
	}
	public void LoadSplitData(DateTime startDate1, List<TimeSpan> data1, DateTime startDate2, List<TimeSpan> data2)
	{
		_startDate1 = startDate1;
		_data1 = ChartData.CreateChartData(data1);
		_startDate2 = startDate2;
		_data2 = ChartData.CreateChartData(data2);
	}
}
