using System.Text;
using Alpha.BetLists.API.Resources;
using TplNetCore.Element;

namespace Alpha.BetLists.API.Templates;

public class PTRacing_Template : Template
{
    protected static string s0 = "<span class='pt-win blue' style='font-weight:normal;'>";
    protected static string s1 { get { return BetListResources.Win; } }
    protected static string s2 = ":<b>&nbsp;";
    public string PT_Win = "{PT_Win}";
    protected static string s3 = "</b></span><br /><span class='pt-place' style='font-weight:normal;'>";
    protected static string s4 { get { return BetListResources.Place; } }
    protected static string s5 = ":<b>&nbsp;";
    public string PT_Place = "{PT_Place}";
    protected static string s6 = "</b></span>";
    public override void ToString(ref StringBuilder bd)
    {
        if (!Visible) return;
        if (RenderChild)
        {
            bd.Append(s0);
            bd.Append(s1);
            bd.Append(s2);
            bd.Append(PT_Win);
            bd.Append(s3);
            bd.Append(s4);
            bd.Append(s5);
            bd.Append(PT_Place);
            bd.Append(s6);
        }
        if (IsAssigned) { bd.Append(Value); }
    }

    public string ToString()
    {
        StringBuilder bd = new StringBuilder();
        ToString(ref bd);
        return bd.ToString();
    }
}