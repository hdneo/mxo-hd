using System;
using System.Collections.Generic;
using System.Text;

namespace hds
{
    public class ViewManager
    {
        public List<ClientView> views = new List<ClientView>();
        UInt16 currentViewID = 3; // We start from 3 to increment
        uint currentSpawnCounter = 1;
        

        public ViewManager(){
        }

        public ClientView addView(UInt64 entityId, UInt32 goId)
        {
            // Add the Object to the client View List
            currentViewID++;
            ClientView newView = new ClientView(goId, currentViewID, entityId);
            views.Add(newView);
            return newView;
        }

        public ClientView getViewById(UInt16 viewId)
        {
            ClientView view = null;
            view = views.Find(delegate(ClientView cv) { return cv.ViewID == viewId; });
            return view;
        }

        public void removeViewByViewId(UInt16 viewId)
        {
            ClientView view = getViewById(viewId);
            if (view != null)
            {
                views.Remove(view);
            }
        }

        // This method should check if client has a view for a GoID 
        // If not it adds it to his List of Views and response the ClientView Object
        public ClientView getViewForEntityAndGo(UInt64 entityId, UInt32 goID)
        {

            ClientView view = views.Find(delegate(ClientView cv) { return cv.entityId == entityId; });

            // Nothing found
            if (view == null)
            {
                view = new ClientView(0, 0, 0);
            }

            if (view.ViewID == 0)
            {
                view = addView(entityId,goID);
            }
            /*
            else
            {
                // The View was already created
                view.viewCreated = true;
            }*/
            return view;
        }

    }
}
