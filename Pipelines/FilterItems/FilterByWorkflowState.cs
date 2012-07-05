namespace scLabs.PreProdPreview.Pipelines.FilterItems
{
   using System.Linq;
   using Sitecore;
   using Sitecore.Data;
   using Sitecore.Data.Items;
   using Sitecore.Pipelines.FilterItem;
   using Sitecore.SecurityModel;
   using Sitecore.Sites;
   using Sitecore.StringExtensions;

   public class FilterByWorkflowState
   {
      public void Process(FilterItemPipelineArgs args)
      {
         var site = Context.Site;
         if (site == null) return;

         using (new SecurityDisabler())
         {
            if (!site.EnableWorkflow && Config.FilterByWorkflowEnabled)
            {
               var filtered = ProcessWorkflowStateFiltering(args.FilteredItem);
               args.FilteredItem = filtered;
            }
         }
      }

      protected virtual Item ProcessWorkflowStateFiltering(Item item)
      {
         if (Config.NoFilteringSite == null) return item;

         using (new SiteContextSwitcher(Config.NoFilteringSite))
         {
            var versions = item.Versions.GetVersions();
            if (versions.Length == 0) return item;

            for (var i = versions.Length - 1; i >= 0; i--)
            {
               var version = versions[i];
               if (!ShouldBeFiltered(version))
               {
                  return version;
               }
            }
            return null;
         }
      }

      protected virtual bool ShouldBeFiltered(Item item)
      {
         var currentStateId = item[FieldIDs.WorkflowState];

         if (currentStateId.IsNullOrEmpty()) return false;

         return ID.IsID(currentStateId) &&
                Config.WorkflowStatesToFilter.Contains(ID.Parse(currentStateId));
      }
   }
}


