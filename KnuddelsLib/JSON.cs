using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnuddelsLib
{
    public class JSON
    {
        #region ConversationWithParticipants7
        public class ConversationWithParticipants
        {
            public string id { get; set; }
            public string __typename { get; set; }
        }

        public class DataConversationWithParticipants
        {
            public MessengerConversationWithParticipants messenger { get; set; }
        }
        public class MessengerConversationWithParticipants
        {
            public ConversationWithParticipants conversationWithParticipants { get; set; }
            public string __typename { get; set; }
        }

        public class RootConversationWithParticipants
        {
            public DataConversationWithParticipants data { get; set; }
            public Extensions extensions { get; set; }
        }


        #endregion
        #region Album
        public class Album
        {
            public string id { get; set; }
            public string title { get; set; }
            public bool isOwner { get; set; }
            public List<AlbumPhoto> albumPhotos { get; set; }
            public string __typename { get; set; }
        }

        public class AlbumPhoto
        {
            public string id { get; set; }
            public string thumbnailUrl { get; set; }
            public string __typename { get; set; }
            public string photoUrl { get; set; }
            public object administrationUrl { get; set; }
            public object description { get; set; }
            public bool isOwner { get; set; }
        }

        public class AlbumProfilePhoto
        {
            public string id { get; set; }
            public string thumbnailUrl { get; set; }
            public string photoUrl { get; set; }
            public object administrationUrl { get; set; }
            public object description { get; set; }
            public bool isOwner { get; set; }
            public string __typename { get; set; }
        }

        public class AlbumData
        {
            public AlbumUser user { get; set; }
        }

        public class AlbumRoot
        {
            public AlbumData data { get; set; }
            public Extensions extensions { get; set; }
        }

        public class AlbumUser
        {
            public AlbumUser user { get; set; }
            public string __typename { get; set; }
            public List<AlbumPhoto> albumPhotos { get; set; }
            public List<Album> albums { get; set; }
            public AlbumProfilePhoto albumProfilePhoto { get; set; }
        }
        #endregion
        public class BanMessage
        {
            public string error { get; set; }
            public ErrorDetails errorDetails { get; set; }
            public class ErrorDetails
            {
                public List<string> reason { get; set; }
                public string admin { get; set; }
                public long locked_until { get; set; }
                public string lock_type { get; set; }
                public string target { get; set; }
            }
        }
        class ResultID
        {
            public string id { get; set; }
            public string altterm { get; set; }
            public string alttermh { get; set; }
            public bool softselectorwarning { get; set; }
            public int status { get; set; }
        }
        #region Login    
        public class Ok
        {
            public string nick { get; set; }
            public string text { get; set; }
            public string pwd { get; set; }
        }

        public class LoginAPI
        {
            public string jwt { get; set; } //token
            public Ok ok { get; set; } //UserInformations
        }
        #endregion
        #region UsableToke
        public class RefreshSession5
        {
            public string expiry { get; set; }
            public string token { get; set; }
            public string __typename { get; set; }
        }

        public class Login5
        {
            public RefreshSession5 refreshSession { get; set; }
            public string __typename { get; set; }
        }

        public class Data5
        {
            public Login5 login { get; set; }
        }

        public class Extensions5
        {
            [JsonProperty("gql-correlation-id")]
            public string GqlCorrelationId { get; set; }
            public string instance { get; set; }
        }

        public class UsuableToken
        {
            public Data5 data { get; set; }
            public Extensions5 extensions { get; set; }
        }
        #endregion
        #region Chats
        public class ProfilePicture
        {
            public string urlCustomSizeSquare { get; set; }
            public string __typename { get; set; }
        }
        public class Text
        {
            public string text { get; set; }
            public bool bold { get; set; }
            public bool italic { get; set; }
        }
        public class RootText
        {
            public Text text { get; set; }
        }

        public class OtherParticipant
        {
            public string id { get; set; }
            public string nick { get; set; }
            public bool isOnline { get; set; }
            public bool canSendImages { get; set; }
            public string __typename { get; set; }
            public int age { get; set; }
            public string albumPhotosUrl { get; set; }
            public bool canReceiveMessages { get; set; }
            public string city { get; set; }
            public object distance { get; set; }
            public string gender { get; set; }
            public string ignoreState { get; set; }
            public bool isIgnoring { get; set; }
            public ProfilePicture profilePicture { get; set; }
            public string readMe { get; set; }
            public string relationshipStatus { get; set; }
            public string sexualOrientation { get; set; }
            public int onlineMinutes { get; set; }
            public bool isAppBot { get; set; }
            public bool isLockedByAutomaticComplaint { get; set; }
            public string automaticComplaintCommand { get; set; }
            public bool isReportable { get; set; }
        }

        public class LastReadMessage
        {
            public string id { get; set; }
            public string __typename { get; set; }
        }

        public class ReadState
        {
            public bool markedAsUnread { get; set; }
            public int unreadMessageCount { get; set; }
            public LastReadMessage lastReadMessage { get; set; }
            public string __typename { get; set; }
        }

        public class Sender
        {
            public string id { get; set; }
            public string nick { get; set; }
            public bool isOnline { get; set; }
            public bool canSendImages { get; set; }
            public string __typename { get; set; }
        }

        public class Image
        {
            public string url { get; set; }
            public string __typename { get; set; }
        }

        public class LatestMessage
        {
            public string id { get; set; }
            public object nestedMessage { get; set; }
            public Sender sender { get; set; }
            public bool starred { get; set; }
            public string formattedText { get; set; }
            public string timestamp { get; set; }
            public Image image { get; set; }
            public object snap { get; set; }
            public string __typename { get; set; }
        }

        public class Conversation
        {
            public string id { get; set; }
            public bool isArchived { get; set; }
            public List<OtherParticipant> otherParticipants { get; set; }
            public ReadState readState { get; set; }
            public LatestMessage latestMessage { get; set; }
            public string __typename { get; set; }
            public List<Conversation> conversations { get; set; }
            public bool hasMore { get; set; }
        }

        public class Messenger
        {
            public Conversation conversations { get; set; }
            public string __typename { get; set; }
        }

        public class Data
        {
            public Messenger messenger { get; set; }
        }

        public class Extensions
        {
            [JsonProperty("gql-correlation-id")]
            public string GqlCorrelationId { get; set; }
            public string instance { get; set; }
        }

        public class Chats
        {
            public Data data { get; set; }
            public Extensions extensions { get; set; }
        }
        #endregion
        #region ProfileVisitor

        public class Visitor
        {
            public string id { get; set; }
            public string nick { get; set; }
            public int age { get; set; }
            public string gender { get; set; }
            public string city { get; set; }
            public ProfilePicture profilePicture { get; set; }
            public string __typename { get; set; }
        }

        public class ProfileVisitors
        {
            public List<Visitor> visitors { get; set; }
            public string visibilityStatus { get; set; }
            public string __typename { get; set; }
        }

        public class User
        {
            public ProfileVisitors profileVisitors { get; set; }
            public string __typename { get; set; }
        }
        public class Data2
        {
            public User user { get; set; }
        }
        public class ProfileVisitor
        {
            public Data2 data { get; set; }
            public Extensions extensions { get; set; }
        }
        #endregion
        #region IDbyNick
        public class ProfilePicture2
        {
            public string urlCustomSizeSquare { get; set; }
            public string __typename { get; set; }
        }

        public class UserFromNick
        {
            public string id { get; set; }
            public string nick { get; set; }
            public ProfilePicture2 profilePicture { get; set; }
            public bool isOnline { get; set; }
            public string readMe { get; set; }
            public bool canReceiveMessages { get; set; }
            public string __typename { get; set; }
        }
        public class User3
        {
            public UserFromNick userFromNick { get; set; }
            public string __typename { get; set; }
        }

        public class Data3
        {
            public User3 user { get; set; }
        }
        public class UserInfo
        {
            public Data3 data { get; set; }
            public Extensions extensions { get; set; }
        }
        #endregion
        #region TokenUserInfo
        public class ProfilePicture4
        {
            public bool exists { get; set; }
            public string urlLargeSquare { get; set; }
            public string urlVeryLarge { get; set; }
            public string __typename { get; set; }
        }

        public class CurrentUser
        {
            public string id { get; set; }
            public string nick { get; set; }
            public string gender { get; set; }
            public ProfilePicture4 profilePicture { get; set; }
            public int knuddelAmount { get; set; }
            public int age { get; set; }
            public string dateOfRegistration { get; set; }
            public string __typename { get; set; }
        }

        public class User4
        {
            public CurrentUser currentUser { get; set; }
            public string __typename { get; set; }
        }

        public class Data4
        {
            public User4 user { get; set; }
        }

        public class Extensions4
        {
            [JsonProperty("gql-correlation-id")]
            public string GqlCorrelationId { get; set; }
            public string instance { get; set; }
        }

        public class CurrentUserInfo
        {
            public Data4 data { get; set; }
            public Extensions4 extensions { get; set; }
        }
        #endregion
        #region Channels
        public class BackgroundColor
        {
            public int alpha { get; set; }
            public int blue { get; set; }
            public int green { get; set; }
            public int red { get; set; }
            public string __typename { get; set; }
        }

        public class Info
        {
            public string previewImageUrl { get; set; }
            public BackgroundColor backgroundColor { get; set; }
            public string __typename { get; set; }
        }

        public class Channel2
        {
            public string id { get; set; }
            public int onlineUserCount { get; set; }
            public string __typename { get; set; }
            public string name { get; set; }
        }

        public class ChannelGroup
        {
            public string id { get; set; }
            public string name { get; set; }
            public Info info { get; set; }
            public List<Channel2> channels { get; set; }
            public string __typename { get; set; }
        }

        public class ChannelAd
        {
            public int adCampaignId { get; set; }
            public ChannelGroup channelGroup { get; set; }
            public string __typename { get; set; }
        }

        public class ChannelGroup2
        {
            public string id { get; set; }
            public string name { get; set; }
            public Info info { get; set; }
            public List<Channel> channels { get; set; }
            public string __typename { get; set; }
        }

        public class Category
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<ChannelGroup> channelGroups { get; set; }
            public string __typename { get; set; }
        }

        public class Channel
        {
            public List<ChannelAd> channelAds { get; set; }
            public List<Category> categories { get; set; }
            public string __typename { get; set; }
        }

        public class Data6
        {
            public Channel channel { get; set; }
        }

        public class Extensions6
        {
            [JsonProperty("gql-correlation-id")]
            public string GqlCorrelationId { get; set; }
            public string instance { get; set; }
        }

        public class RootChannel
        {
            public Data6 data { get; set; }
            public Extensions6 extensions { get; set; }
        }
        #endregion
        #region inChannel
        public class ProfilePicture10
        {
            public string urlCustomSizeSquare { get; set; }
            public string __typename { get; set; }
        }

        public class User10
        {
            public string id { get; set; }
            public string nick { get; set; }
            public int age { get; set; }
            public string gender { get; set; }
            public ProfilePicture10 profilePicture { get; set; }
            public string __typename { get; set; }
        }

        public class BackgroundColor10
        {
            public int alpha { get; set; }
            public int blue { get; set; }
            public int green { get; set; }
            public int red { get; set; }
            public string __typename { get; set; }
        }

        public class BackgroundImageInfo10
        {
            public string mode { get; set; }
            public string url { get; set; }
            public string __typename { get; set; }
        }

        public class HighlightColor10
        {
            public int alpha { get; set; }
            public int blue { get; set; }
            public int green { get; set; }
            public int red { get; set; }
            public string __typename { get; set; }
        }

        public class GroupInfo10
        {
            public BackgroundColor10 backgroundColor { get; set; }
            public BackgroundImageInfo10 backgroundImageInfo { get; set; }
            public HighlightColor10 highlightColor { get; set; }
            public bool isMyChannel { get; set; }
            public string __typename { get; set; }
        }

        public class Channel210
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<User10> users { get; set; }
            public GroupInfo10 groupInfo { get; set; }
            public string __typename { get; set; }
        }

        public class JoinById10
        {
            public Channel210 channel { get; set; }
            public object error { get; set; }
            public string __typename { get; set; }
        }
        public class Channel10
        {
            public JoinById10 joinById { get; set; }
        }

        public class Data10
        {
            public Channel10 channel { get; set; }
        }

        public class Extensions10
        {
            [JsonProperty("gql-correlation-id")]
            public string GqlCorrelationId { get; set; }
            public string instance { get; set; }
        }

        public class Root10
        {
            public Data10 data { get; set; }
            public Extensions10 extensions { get; set; }
        }
        #endregion
        #region Messages
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class ProfilePicture11
        {
            public string urlCustomSizeSquare { get; set; }
            public string __typename { get; set; }
        }

        public class OtherParticipant11
        {
            public string id { get; set; }
            public string nick { get; set; }
            public bool isOnline { get; set; }
            public bool canSendImages { get; set; }
            public string __typename { get; set; }
            public int age { get; set; }
            public string albumPhotosUrl { get; set; }
            public bool canReceiveMessages { get; set; }
            public string city { get; set; }
            public object distance { get; set; }
            public string gender { get; set; }
            public string ignoreState { get; set; }
            public bool isIgnoring { get; set; }
            public ProfilePicture11 profilePicture { get; set; }
            public string readMe { get; set; }
            public string relationshipStatus { get; set; }
            public string sexualOrientation { get; set; }
            public int onlineMinutes { get; set; }
            public bool isAppBot { get; set; }
            public bool isLockedByAutomaticComplaint { get; set; }
            public string automaticComplaintCommand { get; set; }
            public bool isReportable { get; set; }
        }

        public class LastReadMessage11
        {
            public string id { get; set; }
            public string __typename { get; set; }
        }

        public class ReadState11
        {
            public bool markedAsUnread { get; set; }
            public int unreadMessageCount { get; set; }
            public LastReadMessage11 lastReadMessage { get; set; }
            public string __typename { get; set; }
        }

        public class Sender11
        {
            public string id { get; set; }
            public string nick { get; set; }
            public bool isOnline { get; set; }
            public bool canSendImages { get; set; }
            public string __typename { get; set; }
        }

        public class LatestMessage11
        {
            public string id { get; set; }
            public object nestedMessage { get; set; }
            public Sender11 sender { get; set; }
            public bool starred { get; set; }
            public string formattedText { get; set; }
            public string timestamp { get; set; }
            public object image { get; set; }
            public object snap { get; set; }
            public string __typename { get; set; }
        }
        public class Image11
        {
            public string url { get; set; }
            public string __typename { get; set; }
        }
        public class Message11
        {
            public string id { get; set; }
            public object nestedMessage { get; set; }
            public Sender11 sender { get; set; }
            public bool starred { get; set; }
            public string formattedText { get; set; }
            public string timestamp { get; set; }
            public object image { get; set; }
            public object snap { get; set; }
            public string __typename { get; set; }
            public List<Message11> messages { get; set; }
            public bool hasMore { get; set; }
        }

        public class Conversation11
        {
            public string id { get; set; }
            public bool isArchived { get; set; }
            public List<OtherParticipant11> otherParticipants { get; set; }
            public ReadState11 readState { get; set; }
            public LatestMessage11 latestMessage { get; set; }
            public string __typename { get; set; }
            public Message11 messages { get; set; }
        }

        public class Messenger11
        {
            public Conversation11 conversation { get; set; }
            public string __typename { get; set; }
        }

        public class Data11
        {
            public Messenger11 messenger { get; set; }
        }

        public class Extensions11
        {
            [JsonProperty("gql-correlation-id")]
            public string GqlCorrelationId { get; set; }
            public string instance { get; set; }
        }

        public class Root11
        {
            public Data11 data { get; set; }
            public Extensions11 extensions { get; set; }
        }


        #endregion
    }
}
