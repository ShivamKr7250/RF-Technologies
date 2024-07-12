using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;

namespace RF_Technologies.Data_Access.Data
{
    public class YouTubeServiceFile
    {
        private readonly YouTubeService _youtubeService;
        private readonly string _apiKey;

        public YouTubeServiceFile(string apiKey)
        {
            _apiKey = apiKey;
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = "RF Technologies"
            });
        }

        public async Task<Channel> GetChannelDetailsAsync(string channelId)
        {
            var request = _youtubeService.Channels.List("snippet,contentDetails,statistics");
            request.Id = channelId;
            var response = await request.ExecuteAsync();

            // Check if response or Items is null or empty
            if (response?.Items == null || response.Items.Count == 0)
            {
                // Handle the case where no channels are found
                throw new Exception("No channels found with the provided channel ID.");
            }

            return response.Items[0];
        }


        public async Task<IList<Playlist>> GetPlaylistsAsync(string channelId)
        {
            var request = _youtubeService.Playlists.List("snippet");
            request.ChannelId = channelId;
            request.MaxResults = 50;
            var response = await request.ExecuteAsync();
            return response.Items;
        }

        public async Task<IList<PlaylistItem>> GetPlaylistItemsAsync(string playlistId)
        {
            var request = _youtubeService.PlaylistItems.List("snippet");
            request.PlaylistId = playlistId;
            request.MaxResults = 50;
            var response = await request.ExecuteAsync();
            return response.Items;
        }

        public async Task<IList<SearchResult>> GetLatestVideosAsync(string channelId, int maxResults = 5)
        {
            var request = _youtubeService.Search.List("snippet");
            request.ChannelId = channelId;
            request.MaxResults = maxResults;
            request.Order = SearchResource.ListRequest.OrderEnum.Date;
            var response = await request.ExecuteAsync();
            return response.Items;
        }
    }
}
