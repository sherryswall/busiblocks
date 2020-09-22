using System;
using System.Linq;
using System.Web.Services;
using BusiBlocks.DocoBlock;

/// <summary>
/// Summary description for AutoSave
/// </summary>
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class AutoSave : WebService
{
    [WebMethod]
    public bool Save(string chaptId, string content, string chapName)
    {
        var cv = DocoManager.GetAllChapterVersion().Where(x => x.Id.Equals(chaptId)).First<ChapterVersion>();

        if (chapName != cv.Name)
        {
            cv.Name = chapName;
            DocoManager.UpdateChapterVersion(cv);
        }
        var draft = new Draft();
        draft.VersionId = chaptId;
        draft.Content = content;
        draft.SaveDate = DateTime.Now;
        DocoManager.UpsertDraft(draft);

        return true;
    }

    [WebMethod]
    public string ChangeChapterName(string chaptId, string chapName)
    {
        var cv = DocoManager.GetAllChapterVersion().Where(x => x.Id.Equals(chaptId)).First<ChapterVersion>();

        if (chapName != cv.Name)
        {
            cv.Name = chapName;
            DocoManager.UpdateChapterVersion(cv);
        }
        return cv.Name;
    }
}
