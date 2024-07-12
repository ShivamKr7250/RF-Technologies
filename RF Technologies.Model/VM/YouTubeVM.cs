using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace RF_Technologies.Model.VM
{
    public class YouTubeViewModel
    {
        public string ChannelTitle { get; set; }
        public ulong? SubscriberCount { get; set; }
        public ulong? TotalViews { get; set; }
        public IList<Playlist> Playlists { get; set; }
        public IList<SearchResult> LatestVideos { get; set; }
    }
}
