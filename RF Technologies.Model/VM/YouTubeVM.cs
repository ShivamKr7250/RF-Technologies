using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace RF_Technologies.Model.VM
{
        public class YouTubeViewModel
        {
            // General Channel Information
            public string ChannelId { get; set; }
            public string ChannelTitle { get; set; }
            public string Description { get; set; }
            public string CustomUrl { get; set; }
            public string Country { get; set; }
            public DateTime? PublishedAt { get; set; }
            public ulong? SubscriberCount { get; set; }
            public ulong? TotalViews { get; set; }
            public ulong? VideoCount { get; set; }
            public Thumbnails ChannelThumbnails { get; set; }

            // Related Playlists
            public IList<Playlist> Playlists { get; set; }

            // Latest Videos or Search Results
            public IList<SearchResult> LatestVideos { get; set; }

            // Branding Settings (e.g., Banner)
            public string BannerImageUrl { get; set; }
            public string Keywords { get; set; }
            public bool? ShowRelatedChannels { get; set; }
            public IList<string> FeaturedChannels { get; set; }

            // Statistics
            public Statistics ChannelStatistics { get; set; }

            // Content Details
            public ContentDetails ChannelContentDetails { get; set; }

            // Video Information
            public IList<Video> Videos { get; set; }

            // Live Streaming Information
            public LiveBroadcast LiveBroadcastDetails { get; set; }
        }

        // Supporting Classes
        public class Thumbnails
        {
            public string Default { get; set; }
            public string Medium { get; set; }
            public string High { get; set; }
        }

        public class Statistics
        {
            public ulong? ViewCount { get; set; }
            public ulong? SubscriberCount { get; set; }
            public bool? HiddenSubscriberCount { get; set; }
            public ulong? VideoCount { get; set; }
        }

        public class ContentDetails
        {
            public string RelatedPlaylistsUpload { get; set; }
            public string RelatedPlaylistsWatchHistory { get; set; }
            public string RelatedPlaylistsWatchLater { get; set; }
        }

        public class Playlist
        {
            public string PlaylistId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public Thumbnails Thumbnails { get; set; }
            public DateTime? PublishedAt { get; set; }
        }

        public class Video
        {
            public string VideoId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public Thumbnails Thumbnails { get; set; }
            public DateTime? PublishedAt { get; set; }
            public ulong? ViewCount { get; set; }
            public ulong? LikeCount { get; set; }
            public ulong? DislikeCount { get; set; }
            public ulong? CommentCount { get; set; }
        }

        public class SearchResult
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public Thumbnails Thumbnails { get; set; }
            public DateTime? PublishedAt { get; set; }
        }

        public class LiveBroadcast
        {
            public string BroadcastId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ScheduledStartTime { get; set; }
            public DateTime? ActualStartTime { get; set; }
            public string StreamStatus { get; set; }
        }
}
