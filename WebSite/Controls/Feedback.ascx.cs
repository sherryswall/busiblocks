using System.Web.UI;
using BusiBlocks;

public partial class Controls_Feedback : UserControl
{
    public void Show(string block, string obj, string action, string item)
    {
        var feedback = new Feedback(block, obj, action, item);
        Show(feedback);
    }

    public void Show(string action, string item)
    {
        var feedback = new Feedback(action, item);
        Show(feedback);
    }

    private void Show(Feedback feedback)
    {
        pnlLeft.CssClass = feedback.Action;
        pnlContent.CssClass = feedback.Type;
        litMessage.Text = feedback.Message;
        pnlFeedback.Visible = true;
    }

    public void Hide()
    {
        pnlFeedback.CssClass = "";
        litMessage.Text = "";
        pnlFeedback.Visible = false;
    }
}
