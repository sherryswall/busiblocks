using System;
using System.Collections.Generic;
using System.Text;

namespace BusiBlocks.DocoBlock
{
    public enum ArticleStatus
    {
        All = 0,
        Enabled = 1,
        Disabled = 2,
        Approved = 3,
        NotApproved = 4,
        EnabledAndApproved = 5,
        DisabledOrNotApproved = 6
    }

    public enum EnabledStatus
    {
        All = 0,
        Enabled = 1,
        Disabled = 2
    }

    public enum VersionUpdateType
    {
        Content =0,
        ChapterName=1,
        ChapterAndContent=2,
        New=3
    }
}
