namespace scLabs.PreProdPreview
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Sitecore;
   using Sitecore.Configuration;
   using Sitecore.Data;
   using Sitecore.Sites;

   public static class Config
   {
      public static string FilteredWorkflowStates
      {
         get
         {
            return Settings.GetSetting("PreProdPreview.FilteredWorkflowStates");
         }
      }

      public static IEnumerable<ID> WorkflowStatesToFilter
      {
         get
         {
            var stateIds = MainUtil.RemoveEmptyStrings(FilteredWorkflowStates.ToLower().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            return from stateId in stateIds where ID.IsID(stateId) select ID.Parse(stateId);
         }
      }

      public static SiteContext NoFilteringSite
      {
         get { return SiteContextFactory.GetSiteContext("nofiltering_reserved"); }
      }

      public static bool FilterByWorkflowEnabled
      {
         get
         {
            return Context.Site != null &&
                   Context.Site.SiteInfo.Properties["filterByWorkflow"] != null &&
                   Context.Site.SiteInfo.Properties["filterByWorkflow"].Equals("true", StringComparison.InvariantCultureIgnoreCase);
         }
      }
   }

}