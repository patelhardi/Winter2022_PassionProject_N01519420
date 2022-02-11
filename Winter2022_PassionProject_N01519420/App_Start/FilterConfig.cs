using System.Web;
using System.Web.Mvc;

namespace Winter2022_PassionProject_N01519420
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
