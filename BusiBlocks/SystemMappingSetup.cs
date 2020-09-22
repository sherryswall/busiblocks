using BusiBlocks.SchemaGenerator;

[assembly: SetupMapping("Audit", "BusiBlocks.Audit.AuditRecord, BusiBlocks")]
[assembly:
    SetupMapping("Profile", "BusiBlocks.Profile.ProfileUser, BusiBlocks; BusiBlocks.Profile.ProfileProperty, BusiBlocks"
        )]
[assembly:
    SetupMapping("Access", "BusiBlocks.AccessLayer.Access, BusiBlocks; BusiBlocks.AccessLayer.Folder, BusiBlocks")]
[assembly:
    SetupMapping("Doco",
        "BusiBlocks.DocoBlock.Category, BusiBlocks.DocoBlock; BusiBlocks.DocoBlock.Article, BusiBlocks.DocoBlock; BusiBlocks.DocoBlock.FileAttachment, BusiBlocks.DocoBlock; BusiBlocks.DocoBlock.VersionedArticle, BusiBlocks.DocoBlock;BusiBlocks.DocoBlock.Chapter,BusiBlocks.DocoBlock;BusiBlocks.DocoBlock.ChapterVersion,BusiBlocks.DocoBlock;BusiBlocks.DocoBlock.Draft,BusiBlocks.DocoBlock;"
        )]
[assembly: SetupMapping("People and Site and Address and Membership and Roles and PrivateMessages",
    "BusiBlocks.Membership.User, BusiBlocks; BusiBlocks.SiteLayer.Region, BusiBlocks; BusiBlocks.SiteLayer.RegionType, BusiBlocks; BusiBlocks.SiteLayer.Site, BusiBlocks; BusiBlocks.SiteLayer.SiteType, BusiBlocks; BusiBlocks.PersonLayer.Person, BusiBlocks; BusiBlocks.PersonLayer.PersonType, BusiBlocks; BusiBlocks.PersonLayer.PersonSite, BusiBlocks; BusiBlocks.PersonLayer.ItemStatus, BusiBlocks; BusiBlocks.PersonLayer.PersonPersonType, BusiBlocks; BusiBlocks.PersonLayer.PersonTypeRole, BusiBlocks; BusiBlocks.PersonLayer.PersonRegion, BusiBlocks; BusiBlocks.Roles.Role, BusiBlocks; BusiBlocks.Roles.UserInRole, BusiBlocks; BusiBlocks.AddressLayer.Address, BusiBlocks; BusiBlocks.CommsBlock.PrivateMessages.PrivateMessage, BusiBlocks.CommsBlock"
    )]
[assembly:
    SetupMapping("Forums",
        "BusiBlocks.CommsBlock.Forums.Category, BusiBlocks.CommsBlock; BusiBlocks.CommsBlock.Forums.Topic, BusiBlocks.CommsBlock; BusiBlocks.CommsBlock.Forums.Message, BusiBlocks.CommsBlock"
        )]
[assembly:
    SetupMapping("News",
        "BusiBlocks.CommsBlock.News.Category, BusiBlocks.CommsBlock; BusiBlocks.CommsBlock.News.Item, BusiBlocks.CommsBlock"
        )]
[assembly:
    SetupMapping("Forms",
        "BusiBlocks.FormsBlock.FormDefinition, BusiBlocks.FormsBlock; BusiBlocks.FormsBlock.FormProperty, BusiBlocks.FormsBlock; BusiBlocks.FormsBlock.FormInstance, BusiBlocks.FormsBlock; BusiBlocks.FormsBlock.FormPropertyInstance, BusiBlocks.FormsBlock; "
        )]
[assembly:
    SetupMapping("Versions",
        "BusiBlocks.Versioning.VersionItem;BusiBlocks"
        )]
[assembly:
    SetupMapping("Feedbacks",
        "BusiBlocks.ContactFeedback.FeedbackForm;BusiBlocks"
        )]
