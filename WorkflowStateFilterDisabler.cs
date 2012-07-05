using System;
using System.Collections.Generic;
using System.Threading;
using Sitecore.Web;

namespace scLabs.PreProdPreview
{
   public class WorkflowStateFilterDisabler : IDisposable
   {
      #region Fields

      private static object _lock = new object();
      private string _oldValue;
      protected static List<ThreadStart> _onLeave = new List<ThreadStart>();
      private const string ItemsKey = "WorkflowStateFilterDisabler";
      private int m_disposed;

      #endregion

      #region ctor

      public WorkflowStateFilterDisabler()
      {
         lock (_lock)
         {
            _oldValue = WebUtil.GetItemsString(ItemsKey);
            WebUtil.SetItemsValue(ItemsKey, "1");
         }
      }

      #endregion

      #region Properties

      public static bool IsActive
      {
         get { return (WebUtil.GetItemsString(ItemsKey) == "1"); }
      }

      #endregion

      #region Events

      public static event ThreadStart OnLeave
      {
         add
         {
            lock (_lock)
            {
               _onLeave.Add(value);
            }
         }
         remove
         {
            lock (_lock)
            {
               _onLeave.Remove(value);
            }
         }
      }

      #endregion

      #region Methods

      public void Dispose()
      {
         if (Interlocked.CompareExchange(ref m_disposed, 1, 0) == 0)
         {
            WebUtil.SetItemsValue(ItemsKey, _oldValue);
         }
         if (IsActive) return;
         lock (_lock)
         {
            if (IsActive) return;

            foreach (var start in _onLeave)
            {
               start();
            }
         }
      }

      #endregion
   }
}