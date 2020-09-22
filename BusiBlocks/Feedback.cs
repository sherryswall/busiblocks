using System;
using System.Xml;

namespace BusiBlocks
{
    public class Feedback
    {
        #region Constants

        private const string FailedErrorMessage = "Error: BusiBlockFeedback has failed.";

        private const string XmlFilename = "feedback.config";

        public static class Actions
        {
            public readonly static string Created = "created";
            public readonly static string Saved = "saved";
            public readonly static string Deleted = "deleted";
            public readonly static string Acknowledged = "acknowledged";

            public readonly static string Exception = "exception";
            public readonly static string Error = "error";
        }

        private const string XmlGlobalIdentifier = "global";
        private const string XmlTemplateTypeIdentifier = "type";
        private const string XmlElementNameIdentifier = "name";
        private const string XmlFeedbackElementRoot = "feedback";
        
        #endregion

        private string Template { get; set; }
        private DateTime DateTimeStamp { get; set; }

        public string Message { get; set; }
        public string Type { get; set; }

        public string Block { get; set; }
        public string Object { get; set; }
        public string Action { get; set; }
        public string Item { get; set; }

        public Feedback(string action, string item)
            : this(XmlGlobalIdentifier, XmlGlobalIdentifier, action, item) { }

        public Feedback(string obj, string action, string item)
            : this(XmlGlobalIdentifier, obj, action, item) { }

        public Feedback(string block, string obj, string action, string item)
        {
            DateTimeStamp = DateTime.Now;
            
            Block = block;
            Object = obj;
            Action = action;
            Item = item;

            LoadFeedbackTemplate();
            LoadFeedbackMessage();
        }

        private void LoadFeedbackMessage()
        {
            Message = Template;

            Message = Message.Replace("$object", Object);
            Message = Message.Replace("$block", Block);
            Message = Message.Replace("$item", Item);
            Message = Message.Replace("$time", DateTimeStamp.ToString("HH:mm"));
        }

        private void LoadFeedbackTemplate()
        {
            XmlNode templateNode = GetFeedbackNode(Block, Object, Action);

            if (templateNode == null) templateNode = GetFeedbackNode(XmlGlobalIdentifier, Object, Action);
            if (templateNode == null) templateNode = GetFeedbackNode(Block, XmlGlobalIdentifier, Action);
            if (templateNode == null) templateNode = GetFeedbackNode(XmlGlobalIdentifier, XmlGlobalIdentifier, Action);

            Template = (templateNode == null ? FailedErrorMessage : templateNode.InnerXml);
            Type = (templateNode == null ? string.Empty : templateNode.Attributes[XmlTemplateTypeIdentifier].Value);
        }
        
        private static XmlNode GetFeedbackNode(string blockName, string objectName, string actionName)
        {
            var doc = new XmlDocument();

            string configFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XmlFilename);

            doc.Load(configFile);
            XmlNodeList blockNodes = doc.DocumentElement.SelectNodes(XmlFeedbackElementRoot).Item(0).ChildNodes;

            XmlNode feedbackNode = null;

            foreach (XmlNode blockNode in blockNodes)
                if (!blockNode.NodeType.Equals(XmlNodeType.Comment))
                    if (blockNode.Attributes[XmlElementNameIdentifier].Value.ToUpper() == blockName.ToUpper())
                        foreach (XmlNode objectNode in blockNode.ChildNodes)
                            if (objectNode.Attributes[XmlElementNameIdentifier].Value.ToUpper() == objectName.ToUpper())
                                foreach (XmlNode actionNode in objectNode.ChildNodes)
                                    if (actionNode.Attributes[XmlElementNameIdentifier].Value.ToUpper() == actionName.ToUpper())
                                        if (feedbackNode == null)
                                            feedbackNode = actionNode;
            return feedbackNode;
        }    
    }
}
