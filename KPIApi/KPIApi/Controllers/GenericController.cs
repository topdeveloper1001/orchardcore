using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Navigation;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement;
using KPIApi.Models;
using YesSql;
using YesSql.Services;
namespace KPIApi.Controllers
{
    [Route("api/generic")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class GenericController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISession _session;
        public dynamic New { get; set; }
        public GenericController(
            IContentManager contentManager,
            ISession session,
            IShapeFactory shapeFactory,
            IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _contentManager = contentManager;
            _session = session;
            New = shapeFactory;
        }



        [Route("{contentItemId}"), HttpGet]
        public async Task<IActionResult> Get(string contentItemId)
        {
            //string contentItemId = "4yndzhy0yqgyhx3g9gx5dvwjxk"; // KPI Organisation Structure ContentItemId
            var contentItem = await _contentManager.GetAsync(contentItemId);
            //OrganisationStructure os = (OrganisationStructure)contentItem;



            if (contentItem == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewContent, contentItem))
            {
                return Unauthorized();
            }

            return Ok(contentItem);
        }

        [Route("GetByType/{contentType}"), HttpGet]
        public async Task<IActionResult> GetByType(string contentType, bool latest = true, int page = 1, int pageSize = 20)
        {
            

            var query = _session.Query<ContentItem, ContentItemIndex>();
            query = query.With<ContentItemIndex>(x => x.ContentType == contentType && x.Latest == latest);
            var pager = new Pager(new PagerParameters() { Page = page, PageSize = pageSize }, pageSize);
            var totalCount = await query.CountAsync();
            var pageOfContentItems = await query.Skip(pager.GetStartIndex()).Take(pager.PageSize).ListAsync();

            var pagerContent = new PagerContent()
            {
                Page = page,
                PageSize = pageSize,
                TotalItemCount = totalCount,
                ContentItems = pageOfContentItems
            };

            // We display a specific type even if it's not listable so that admin pages
            // can reuse the Content list page for specific types.

            
            return Ok(pagerContent);
        }

        [HttpDelete]
        [Route("{contentItemId}")]
        public async Task<IActionResult> Delete(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId);

            if (contentItem == null)
            {
                return StatusCode(204);
            }

            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.DeleteContent, contentItem))
            //{
            //    return Unauthorized();
            //}

            await _contentManager.RemoveAsync(contentItem);

            return Ok(contentItem);
        }

        
        [Route("Create/{type}"), HttpPost]
        public async Task<IActionResult> CreateContentWithType(string type)
        {
            var contentItem = await _contentManager.NewAsync(type);
            return Ok(contentItem);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ContentItem newContentItem, bool draft = false)
        {
            var contentItem = await _contentManager.GetAsync(newContentItem.ContentItemId, VersionOptions.DraftRequired);
            
            if (contentItem == null)
            {

                await _contentManager.CreateAsync(newContentItem, VersionOptions.DraftRequired);

                
            }
            else
            {
                
                await _contentManager.UpdateAsync(newContentItem);
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!draft)
            {
                await _contentManager.PublishAsync(newContentItem);
            }

            return Ok(newContentItem);
        }
    }
}
