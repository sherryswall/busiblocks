<%@ Application Language="C#" %>

<script runat="server">

    
    private const string ErrorInvalideWebResource = "this is an invalid webresource request";
    private const string WebResourceFilename = "webresource.axd";
        
    void Application_Start(object sender, EventArgs e) 
    {
        //Configure log4net
        log4net.Config.XmlConfigurator.Configure();

        SiteMap.Providers["BreadCrumbsMap"].SiteMapResolve += ResolveCustomNodes; //check for querystrings in breadcrumbs
    }

    private SiteMapNode ResolveCustomNodes(object sender, SiteMapResolveEventArgs e)
    {
        SiteMapNode node = null;

        node = AppendQueryStringToNode(e.Context.Request, node);

        //put any further custom resolve functions here
        
        return node;
    }


    private SiteMapNode AppendQueryStringToNode(HttpRequest httpRequest, SiteMapNode node)
    {
        string queryString = httpRequest.QueryString.ToString();
        
        if (!string.IsNullOrEmpty(queryString)) //if there is a query string in the URL (e.g. ?id=1234)
        {
            string path = "~" + httpRequest.Path;
            node = SiteMap.Providers["BreadCrumbsMap"].FindSiteMapNode(path); //find the node in the sitemap
            if (node != null)
            {
                node.ReadOnly = false;
                if (node.Url.IndexOf("?") >= 0) //if node has existing query string
                {
                    node.Url = node.Url.Substring(0, node.Url.IndexOf("?")); //remove the existing query string from the sitemap node
                }
                node.Url = node.Url + "?" + queryString; //add the new query string to the sitemap node
            }
        }

        return node;
    }


    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
    }

    
                
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
        try
        {
            Exception exception = Server.GetLastError();

            //Don't log ThreadAbortException because is fired each time a Redirect is called
            if (!(exception is System.Threading.ThreadAbortException))
            {
                if ((!exception.Message.ToLower().Trim().Contains(ErrorInvalideWebResource)) && (!Request.Url.ToString().ToLower().Trim().Contains(WebResourceFilename)))
                {
                    if (HttpContext.Current.Session != null)
                    {
                        Session["error"] = exception;                        
                    }
                    
                    HttpContext.Current.Response.Redirect(Navigation.Error().GetServerUrl(true));
                    
                }
            }
        }
        catch (Exception)
        {
            // Exception on exception handling code ... don't do nothing otherwise this method is always called
        }
        
        Server.ClearError();
        
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }

       
</script>
