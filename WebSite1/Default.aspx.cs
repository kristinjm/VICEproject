using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    int multiplier = 4;
    protected void Page_Load(object sender, EventArgs e)
    {
        int rand2 = RandomNumber(266, 339);
        int rand1 = RandomNumber(167, 206);
        int rand3 = RandomNumber(11, 14);
        int rand4 = RandomNumber(00, 99);
        int totalSavings = rand2 - rand1;

        cost.Text = "$" + totalSavings.ToString() + ".00";
        Label1.Text = "Total Annual Operating Cost: $" + (rand2 * multiplier).ToString() + ".00";
        Label2.Text = "Total Annual Operating Cost: $" + (rand1 * multiplier).ToString() + ".00";
        Label3.Text = "Total Annual Savings";
        Label4.Text = rand3+"."+rand4+" GPM";
    }
protected void costButton_Click(object sender, EventArgs e)
{
    if(Radio1.Checked){multiplier = 1;}
    else if (Radio1.Checked) { multiplier = 2; }
    else if (Radio1.Checked) { multiplier = 3; }
    else if (Radio1.Checked) { multiplier = 4; }

    int rand2 = RandomNumber(266, 339);
    int rand1 = RandomNumber(167, 206);
    int rand3 = RandomNumber(11, 14);
    int rand4 = RandomNumber(00, 99);
    int totalSavings = rand2 - rand1;

    cost.Text = "$" + totalSavings.ToString() +".00";
    Label1.Text = "Total Annual Operating Cost: $" + (rand2 * multiplier).ToString() + ".00";
    Label2.Text = "Total Annual Operating Cost: $" + (rand1 * multiplier).ToString() + ".00";
    Label3.Text = "Total Annual Savings";
    Label4.Text = rand3 + "." + rand4 + " GPM";
}
private int RandomNumber(int min, int max)
{
    Random random = new Random();
    return random.Next(min, max);
}
}