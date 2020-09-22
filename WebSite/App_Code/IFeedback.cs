using System;

public interface IFeedback
{
    void SetError(Type context, string message);
    void SetException(Type context, Exception ex);
    void ShowFeedback(string Block, string Object, string Action, string Item);
    void ShowFeedback(string Action, string Item);
    void QueueFeedback(string block, string obj, string action, string item);
    void QueueFeedback(string action, string item);
    void HideFeedback();
}
