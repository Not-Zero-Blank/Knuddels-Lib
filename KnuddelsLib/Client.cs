using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using static KnuddelsLib.JSON;

namespace KnuddelsLib
{
    public class VisitorArgs : EventArgs
    {
        public JSON.Visitor visitor;
    }
    public class NewMessageArgs : EventArgs
    {
        public Conversation chat;
        public string message;
    }
    public class Loop
    {
        public Loop(Action action, int delaysec)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delaysec * 1000);
            timer.AutoReset = true;
            timer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                action();
            };
            timer.Start();
        }
    }
    public class Client
    {
        public string _token;
        public JSON.CurrentUserInfo CurrentUser;
        public JSON.Visitor LastVisitor = null;
        public event EventHandler<VisitorArgs> NewProfileVisitor;
        public event EventHandler<NewMessageArgs> NewPrivateMessage;
        string nickname;
        string password;
        public bool Banned => BanMessage != null;
        public BanMessage BanMessage = null;
        public Client(string Nickname, string Password)
        {
            nickname = Nickname;
            password = Password;
        }

        public void Login()
        {
            RefreshToken();
            RefreshCurrentUser();
        }
        public bool Report(string userid)
        {
            try
            {
                var url = "https://api-de.knuddels.de/api-gateway/graphql";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + _token;
                httpRequest.ContentType = "application/json";
                var data = Payload.Report.Replace("*", userid);
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void StartClient()
        {
            new Loop(RefreshToken, 60 * 5);
            new Loop(delegate () { KeepOnline(); }, 5);
            new Loop(delegate () { NoSleep(); }, 20);
            if (NewProfileVisitor != null)
            {
                new Loop(delegate ()
                {
                    try
                    {
                        var Visitor = GetProfileVisitor()[0];
                        if (LastVisitor == null)
                        {
                            NewProfileVisitor.Invoke(this, new VisitorArgs() { visitor = Visitor });
                        }
                        else if (LastVisitor.nick != Visitor.nick)
                        {
                            NewProfileVisitor.Invoke(this, new VisitorArgs() { visitor = Visitor });
                        }
                        LastVisitor = Visitor;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed Visitor " + e);
                    }
                }, 3);
            }
            if (NewPrivateMessage != null)
            {
                new Loop(delegate ()
                {
                    var Inbox = GetChats(500).data.messenger.conversations.conversations;
                    foreach (var chat in Inbox)
                    {
                        string message = chat.latestMessage.formattedText;
                        if (chat.readState.markedAsUnread && !Loggedmessage.Contains(chat.id + ":" + chat.latestMessage.id))
                        {
                            Loggedmessage.Add(chat.id + ":" + chat.latestMessage.id);
                            NewPrivateMessage.Invoke(null, new NewMessageArgs() { chat = chat, message = message });
                        }
                    }
                }, 3);
            }
        }
        public void RefreshToken()
        {
            var token = GetLoginToken(nickname, password);
            if (token == "The Account isnt Working with the Default API!")
            {
                throw new Exception("The Account isnt Working with the Default API!");
            }
            if (token == "{\"error\":\"Falsches Passwort\"}")
            {
                throw new Exception("Falsches passwort");
            }
            if (token == "{\"error\":\"Zu viele Versuche\"}")
            {
                throw new Exception("Timeout");
            }
            if (token == "Banned!")
            {
                throw new Exception("Banned!");
            }
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var data = Payload.GetUsableToken;
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + token;
                httpRequest.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var data2 = JsonConvert.DeserializeObject<JSON.UsuableToken>(result);
                    _token = data2.data.login.refreshSession.token;
                }
        }
        List<string> Loggedmessage = new List<string>();
        public void ChangePassword(string newpassword)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var data = Payload.ChangePassword;
            data = data.Replace("*", newpassword);
            data = data.Replace("#", nickname);
            data = data.Replace("+", password);
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + _token;
                httpRequest.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                }
        }
        string GetLoginToken(string Nickname, string Password)
        {
            var url = "https://www.knuddels.de/logincheck.html";
            var data = $"nick={Nickname}&pwd={Password}";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + _token;
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.StartsWith("{\"error\":\"Nickname gesperrt\""))
                    {
                        Console.WriteLine(result);
                        BanMessage = JsonConvert.DeserializeObject<BanMessage>(result);
                        return "Banned!";
                    }
                    else if (result.StartsWith("{\"error\""))
                    {
                        return result;
                    }
                    try
                    {
                        var data2 = JsonConvert.DeserializeObject<JSON.LoginAPI>(result);
                        if (data2 == null)
                        {
                            return "The Account isnt Working with the Default API!";
                        }
                        return data2.jwt;
                    }
                    catch (WebException ex)
                    {
                        throw new Exception(Utils.ReadWebexception(ex));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
        }
        public JSON.Channel GetAllChannelObjects()
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.GetAllChannel;
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                try
                {
                    var nig = JsonConvert.DeserializeObject<JSON.RootChannel>(result).data.channel;
                    return nig;
                }
                catch (WebException wx)
                {
                    using (var ExceptionReader = new StreamReader(wx.Response.GetResponseStream()))
                    {
                        var resultex = ExceptionReader.ReadToEnd();
                        throw new Exception(resultex);
                    }
                }
            }
        }
        public List<string> GetAllChannelIds()
        {
            List<string> result = new List<string>();
            var channels = GetAllChannelObjects();
            if (channels == null)
            {
                throw new Exception("Server returned null!");
            }
            foreach (JSON.Category a in channels.categories)
            {
                foreach (var b in a.channelGroups)
                {
                    foreach (var c in b.channels)
                    {
                        result.Add(c.id);
                    }
                }
            }
            return result;
        }
        public List<string> GetUserIDsFromChannel(string id)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.JoinChannelbyId.Replace("*", id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var list = JsonConvert.DeserializeObject<JSON.Root10>(result).data.channel.joinById.channel.users;
                var allIds = new List<string>();
                foreach (JSON.User10 a in list)
                {
                    if (a.nick != "James")
                    {
                        allIds.Add(a.id);
                    }
                }
                return allIds;
            }
        }
        public List<User10> GetUsersFromChannel(string id)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.JoinChannelbyId.Replace("*", id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                try
                {
                    var list = JsonConvert.DeserializeObject<JSON.Root10>(result).data.channel.joinById.channel.users;
                    var resultlist = new List<User10>();
                    foreach (JSON.User10 a in list)
                    {
                        if (a.nick != "James")
                        {
                            resultlist.Add(a);
                        }
                    }
                    return resultlist;
                }
                catch
                {
                    return new List<User10>();
                }
            }
        }
        public List<string> GetUserNICKsFromChannel(string id)
        {
            try
            {
                var url = "https://api-de.knuddels.de/api-gateway/graphql";
                var data = Payload.JoinChannelbyId.Replace("*", id);
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + _token;
                httpRequest.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var list = JsonConvert.DeserializeObject<JSON.Root10>(result).data.channel.joinById.channel.users;
                    var allIds = new List<string>();
                    foreach (JSON.User10 a in list)
                    {
                        if (a.nick != "James")
                        {
                            if (!allIds.Contains(a.nick))
                            {
                                allIds.Add(a.nick);
                            }
                            else
                            {
                            }
                        }
                    }
                    return allIds;
                }
            }
            catch (WebException e)
            {
                throw new Exception(Utils.ReadWebexception(e));
            }
        }
        public List<Message11> GetMessagesFromChannel(string id, string beforemessage = "null")
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.GetMessagesFromChannelID.Replace("*", id).Replace("null", beforemessage);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<JSON.Root11>(result).data.messenger.conversation.messages.messages;
            }
        }
        public JSON.RootConversationWithParticipants ConverstationWithParticipants(string id)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.ConverstationWithParticipants.Replace("*", id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<JSON.RootConversationWithParticipants>(result);
            }
        }
        public OtherParticipant11 GetDetailedUserInfo(string nick)
        {
            var x = GetUserinfobyName(nick);
            var y = ConverstationWithParticipants(x.data.user.userFromNick.id);
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.GetMessagesFromChannelID.Replace("*", y.data.messenger.conversationWithParticipants.id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<JSON.Root11>(result).data.messenger.conversation.otherParticipants[0];
            }
        }
        public JSON.AlbumData GetAlbumPhotos(string id)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.GetAlbumPhotos.Replace("*", id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<JSON.AlbumRoot>(result).data;
            }
        }
        public List<string> GetPicturesFromChannelID(string id)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.GetMessagesFromChannelID.Replace("*", id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var messages = JsonConvert.DeserializeObject<JSON.Root11>(result).data.messenger.conversation.messages.messages;
                if (messages.Count == 5000)
                {
                    messages.AddRange(GetMessagesFromChannel(id, messages.Last().id));
                }
                var pictures = new List<string>();
                foreach (var a in messages)
                {
                    if (a.image != null)
                    {
                        var image = JsonConvert.DeserializeObject<JSON.Image11>(a.image.ToString());
                        pictures.Add(image.url);
                    }
                }
                return pictures;
            }
        }
        public List<string> GetChatIDs(int Fetchlimit)
        {
            var result = new List<string>();
            foreach (var a in GetChats(Fetchlimit).data.messenger.conversations.conversations)
            {
                result.Add(a.id);
            }
            return result;
        }
        public JSON.Chats GetChats(int Fetchlimit, string before = "null")
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            if (before != "null") before = "\"" + before + "\"";
            var data = Payload.GetMessageOverview.Replace("*", Fetchlimit.ToString()).Replace("#", before);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<JSON.Chats>(result);
            }
        }
        public void MarkMessageAsRead(string id)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.MarkRead.Replace("*", id);
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
        public JSON.ProfileVisitor GetProfileVisitorData()
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            String data = Payload.ProfileVisitorData;
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var reader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ProfileVisitor>(result);
            }
        }
        public List<JSON.Visitor> GetProfileVisitor()
        {
            var AllData = GetProfileVisitorData();
            List<JSON.Visitor> result = new List<JSON.Visitor>();
            if (AllData == null)
            {
                throw new Exception("Server returned null!");
            }
            result = AllData.data.user.profileVisitors.visitors;
            return result;
        }
        void RefreshCurrentUser()
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            var data = Payload.GetUserByToken;
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {

                var result = streamReader.ReadToEnd();
                CurrentUser = JsonConvert.DeserializeObject<JSON.CurrentUserInfo>(result);
            }
        }
        public string SendMessageByUsername(string Username, string Text)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var user = GetUserIDbyNickname(Username);
            if (user == null)
            {
                throw new Exception("Coudnt fetch userid!");
            }
            var data = Payload.SendMessage.Replace("*", Text).Replace("-", user);
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {

                var result = streamReader.ReadToEnd();
                return result.ToString();
            }
        }
        public string SendMessageByID(string id, string Text)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var data = Payload.SendMessage.Replace("*", Text).Replace("-", id);
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {

                var result = streamReader.ReadToEnd();
                return result.ToString();
            }
        }
        public string SendChannelMessage(string id, string Text)
        {
            var url = "https://api-de.knuddels.de/api-gateway/graphql";
            var data = Payload.ChannelMessage.Replace("#", Text).Replace("*", id);
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + _token;
            httpRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {

                var result = streamReader.ReadToEnd();
                return result.ToString();
            }
        }
        public string GetUserIDbyNickname(string Username)
        {
            var tmp = GetUserinfobyName(Username);
            if (tmp != null)
            {
                return tmp.data.user.userFromNick.id;
            }
            return null;
        }
        public bool KeepOnline()
        {
            try
            {
                var url = "https://api-de.knuddels.de/api-gateway/graphql";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + _token;
                httpRequest.ContentType = "application/json";
                var data = Payload.KeepOnline;
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool NoSleep()
        {
            try
            {
                var url = "https://api-de.knuddels.de/api-gateway/graphql";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Headers["Authorization"] = "Bearer " + _token;
                httpRequest.ContentType = "application/json";
                var data = Payload.NoSleep;
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public JSON.UserInfo GetUserinfobyName(string nickname)
        {
            try
            {
                var url = "https://api-de.knuddels.de/api-gateway/graphql";
                var data = Payload.GetUserInfoByName.Replace("*", nickname);
                if (proxy == null)
                {
                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Method = "POST";
                    httpRequest.Headers["Authorization"] = "Bearer " + _token;
                    httpRequest.ContentType = "application/json";
                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }
                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<JSON.UserInfo>(result);
                    }
                }
                else
                {
                    var request = ProxyDriver.ProxyRequest(proxy, url);
                    request.AddHeader("Authorization", "Bearer " + _token);
                    var response = request.Post(new Uri(url), data, "application/json");
                    return JsonConvert.DeserializeObject<JSON.UserInfo>(response.ToString());
                }
            }
            catch (WebException e)
            {
                throw new Exception(Utils.ReadWebexception(e));
            }
        }
    }
    internal class Utils
    {
        public static string ReadWebexception(WebException wx)
        {
            using (var ExceptionReader = new StreamReader(wx.Response.GetResponseStream()))
            {
                var resultex = ExceptionReader.ReadToEnd();
                return resultex;
            }
        }
    }
}
