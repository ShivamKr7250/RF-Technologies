using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Model.VM;
using RF_Technologies.Data_Access.Data;

namespace RF_Technologies.Controllers
{
    public class YouTubeController : Controller
    {
        private readonly YouTubeServiceFile _youtubeService;
        private readonly string _channelId = "UC58YPozYGH36TKshvZw0Yrw";

        public YouTubeController(YouTubeServiceFile youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var channelDetails = await _youtubeService.GetChannelDetailsAsync(_channelId);
                var playlists = await _youtubeService.GetPlaylistsAsync(_channelId);
                var latestVideos = await _youtubeService.GetLatestVideosAsync(_channelId);

                var model = new YouTubeViewModel
                {
                    ChannelTitle = channelDetails.Snippet.Title,
                    SubscriberCount = channelDetails.Statistics.SubscriberCount,
                    TotalViews = channelDetails.Statistics.ViewCount,
                    Playlists = playlists,
                    LatestVideos = latestVideos
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Handle exceptions, perhaps return an error view or message
                return Content($"Error fetching YouTube data: {ex.Message}");
            }
        }
    }

}
