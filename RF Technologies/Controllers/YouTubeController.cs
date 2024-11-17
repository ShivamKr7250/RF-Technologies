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
                // Fetch channel details
                var channelDetails = await _youtubeService.GetChannelDetailsAsync(_channelId);
                // Fetch playlists
                var playlists = await _youtubeService.GetPlaylistsAsync(_channelId);
                // Fetch latest videos
                var latestVideos = await _youtubeService.GetLatestVideosAsync(_channelId);
                // Fetch live broadcast details
                //var liveBroadcastDetails = await _youtubeService.GetLiveBroadcastDetailsAsync(_channelId);

                // Populate YouTubeViewModel
                var model = new YouTubeViewModel
                {
                    ChannelId = channelDetails.Id,
                    ChannelTitle = channelDetails.Snippet.Title,
                    Description = channelDetails.Snippet.Description,
                    CustomUrl = channelDetails.Snippet.CustomUrl,
                    Country = channelDetails.Snippet.Country,
                    PublishedAt = channelDetails.Snippet.PublishedAt,
                    SubscriberCount = channelDetails.Statistics.SubscriberCount,
                    TotalViews = channelDetails.Statistics.ViewCount,
                    VideoCount = channelDetails.Statistics.VideoCount,
                    ChannelThumbnails = new Thumbnails
                    {
                        Default = channelDetails.Snippet.Thumbnails.Default__.Url,
                        Medium = channelDetails.Snippet.Thumbnails.Medium.Url,
                        High = channelDetails.Snippet.Thumbnails.High.Url
                    },
                    BannerImageUrl = channelDetails.BrandingSettings?.Image?.BannerExternalUrl,
                    Keywords = channelDetails.BrandingSettings?.Channel?.Keywords,
                    ShowRelatedChannels = channelDetails.BrandingSettings?.Channel?.ShowRelatedChannels,
                    FeaturedChannels = channelDetails.BrandingSettings?.Channel?.FeaturedChannelsUrls,
                    Playlists = playlists?.Select(p => new Playlist
                    {
                        PlaylistId = p.Id,
                        Title = p.Snippet.Title,
                        Description = p.Snippet.Description,
                        PublishedAt = p.Snippet.PublishedAt,
                        Thumbnails = new Thumbnails
                        {
                            Default = p.Snippet.Thumbnails.Default__.Url,
                            Medium = p.Snippet.Thumbnails.Medium.Url,
                            High = p.Snippet.Thumbnails.High.Url
                        }
                    }).ToList(),
                    Videos = latestVideos?.Select(v => new Video
                    {
                        VideoId = v.Id.VideoId,
                        Title = v.Snippet.Title,
                        Description = v.Snippet.Description,
                        PublishedAt = v.Snippet.PublishedAt,
                        Thumbnails = new Thumbnails
                        {
                            Default = v.Snippet.Thumbnails.Default__.Url,
                            Medium = v.Snippet.Thumbnails.Medium.Url,
                            High = v.Snippet.Thumbnails.High.Url
                        },
                        ViewCount = v.Snippet.Ch.ViewCount,
                        LikeCount = v.Statistics?.LikeCount,
                        CommentCount = v.Statistics?.CommentCount
                    }).ToList(),
                    //LiveBroadcastDetails = liveBroadcastDetails != null ? new LiveBroadcast
                    //{
                    //    BroadcastId = liveBroadcastDetails.Id,
                    //    Title = liveBroadcastDetails.Snippet.Title,
                    //    Description = liveBroadcastDetails.Snippet.Description,
                    //    ScheduledStartTime = liveBroadcastDetails.Snippet.ScheduledStartTime,
                    //    ActualStartTime = liveBroadcastDetails.Snippet.ActualStartTime,
                    //   // StreamStatus = liveBroadcastDetails.Status.StreamStatus
                    //} : null
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                return Content($"Error fetching YouTube data: {ex.Message}");
            }
        }


    }

}
