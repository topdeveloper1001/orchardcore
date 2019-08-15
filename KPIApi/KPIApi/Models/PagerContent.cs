using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.Title.Model;
using OrchardCore.Flows.Models;
using OrchardCore.ContentFields.Fields;

namespace KPIApi.Models
{
    
    public class PagerContent 
    {
        
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public IEnumerable<ContentItem> ContentItems { get; set; }
        
    }
    
}
