using EShop.RazorPage.Infrastructure.RazorUtils;
using EShop.RazorPage.Models;
using EShop.RazorPage.Models.Comments;
using EShop.RazorPage.Services.Comments;
using Microsoft.AspNetCore.Mvc;

namespace EShop.RazorPage.Pages.Admin.Comments;

public class IndexModel : BaseRazorFilter<CommentFilterParams>
{
    private readonly ICommentService _commentService;
    private readonly IRenderViewToString _renderView;

    public IndexModel(ICommentService commentService, IRenderViewToString renderView)
    {
        _commentService = commentService;
        _renderView = renderView;
    }

    public CommentFilterResult CommentResult { get; set; }
    public async Task OnGet(int pageId = 1, DateTime? startDate = null,
        DateTime? endDate = null, CommentStatus? commentStatus = null, int? userId = null)
    {
        CommentResult = await _commentService.GetCommentsByFilter(new CommentFilterParams()
        {
            Take = 1,
            PageId = pageId,
            StartDate = startDate,
            EndDate = endDate,
            CommentStatus = commentStatus,
            UserId = userId
        });
    }

    public async Task<IActionResult> OnGetRenderChangeStatusPage(long commentId)
    {
        return await AjaxTryCatch(async () =>
        {
            var comment = await _commentService.GetCommentById(commentId);
            if (comment == null)
                return ApiResult<string>.Error();

            var model = new ChangeStatusCommand()
            {
                CommentId = comment.Id,
                Status = comment.Status
            };

            var view = await _renderView.RenderToStringAsync("_ChangeStatus", model, PageContext);
            return ApiResult<string>.Success(view);
        });
    }

    public async Task<IActionResult> OnPostChangeCommentStatus(ChangeStatusCommand command)
    {
        return await AjaxTryCatch(() =>
            _commentService.ChangeStatus(command.CommentId, command.Status));
    }
}
