using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace tritpo_vknet_t1
{
    class Interface
    {
        VkApi api = new VkApi();
        /// <summary> ID пользователя от имени которого была авторизация</summary>
        ulong selfId;

        /// <summary> Инициализация и авторизация </summary>
        /// <param name="login"> Е-mail или телефон </param> 
        /// <param name="password"> Пароль </param> 
        public Interface(string login, string password)
        {
            var auth = new ApiAuthParams();
            auth.Login = login;
            auth.Password = password;
            auth.Settings = Settings.All;
            auth.ApplicationId = 5256959;
            api.Authorize(auth);
            selfId = (ulong)api.UserId;
        }

        /// <summary> Получение списка диалогов </summary>
        /// <param name="dialogsCount"> Kоличество диалогов, которое необходимо получить </param> 
        /// <param name="dialogsCountOffset"> Cмещение от последнего активного диалога(&gt;=0) </param>
        public List<DialogInfo> getDialogs(uint dialogsCount, int dialogsCountOffset)
        {
            List<DialogInfo> dialogs = new List<DialogInfo>();
            var dialGetParams = new DialogsGetParams();
            dialGetParams.Count = dialogsCount;
            dialGetParams.Offset = dialogsCountOffset;
            var dialog = api.Messages.GetDialogs(dialGetParams);
            var mess = dialog.Messages.ToArray<VkNet.Model.Message>();

            var dialogUserIdList = new List<long>();
            foreach (var s in mess) dialogUserIdList.Add((s.UsersCount != null) ? (long)s.ChatId : (long)s.UserId);
            var dialogUserList = api.Users.Get(dialogUserIdList);

            for (int j = 0; j < dialogUserList.Count; j++)
            {
                var addip = new DialogInfo();

                if (mess[j].UsersCount != null) { addip.Name = mess[j].Title; addip.IsChat = true; addip.Id = (long)mess[j].ChatId; }
                else { addip.Name = dialogUserList.ElementAtOrDefault(j).FirstName + " " + dialogUserList.ElementAtOrDefault(j).LastName; addip.IsChat = false; addip.Id = (long)mess[j].UserId; }
                addip.DialogIconAddr = mess[j].Photo50;
                addip.Unread = mess[j].Unread;
                addip.LastReceivedMessage = mess[j].Body;
                dialogs.Add(addip);
            }
            return dialogs;
        }

        /// <summary> Получение списка сообщений </summary>
        /// <param name="currentDialog"> Выбранный диалог </param> 
        /// <param name="messageCount"> Kоличество сообщений которое необходимо получить(&lt;200) </param> 
        /// <param name="messageCountOffset"> Смещение от последнего сообщения </param>
        public List<MessageInfo> getMessages(DialogInfo currentDialog, uint messageCount, int messageCountOffset)
        {
            var histGetParams = new HistoryGetParams();
            histGetParams.Offset = messageCountOffset;
            histGetParams.Count = messageCount;
            if (currentDialog.IsChat) histGetParams.ChatID = (ulong)currentDialog.Id;
            else histGetParams.UserID = currentDialog.Id;

            var currentMess = api.Messages.GetHistory(histGetParams).Messages.ToList<VkNet.Model.Message>();
            return getMessageInfo(currentMess);
        }

        /// <summary> Получение непосредственно сообщений, если есть пересланные уходит в рекурсию </summary>
        /// <param name="currentMess"> Список сообщений с которых нужно получить данные </param>
        /// <returns> Список сообщений без мусора и без необходимости задействовать Интернет </returns>
        private List<MessageInfo> getMessageInfo(List<VkNet.Model.Message> currentMess)
        {
            List<MessageInfo> messages = new List<MessageInfo>();
            var messageSendersList = new List<long>();
            foreach (var s in currentMess) messageSendersList.Add(s.FromId == null ? (long)s.UserId : (long)s.FromId);
            var chatUsers = api.Users.Get(messageSendersList, ProfileFields.All);       

            foreach (var c in currentMess)
            {
                MessageInfo addip = new MessageInfo();
                addip.Name = String.Format("{0} {1}", chatUsers.First(user => user.Id == (c.FromId == null ? (long)c.UserId : (long)c.FromId)).FirstName, chatUsers.First(user => user.Id == (c.FromId == null ? (long)c.UserId : (long)c.FromId)).LastName);// за эту строчку до сих пор стыдно, но исходную библиотеку постоянно меняли и переделывать весь код приходилось практически на ходу
                addip.Body = c.Body;
                addip.DateTime = (DateTime)c.Date;
                var ReadState = c.ReadState;
                addip.Unread = (ReadState == VkNet.Enums.MessageReadState.Unreaded) ? true : false;
                addip.SenderIconAddr = chatUsers.First(user => user.Id == (c.FromId == null ? (long)c.UserId : (long)c.FromId)).Photo50.AbsoluteUri;
                if (c.ForwardedMessages.Count != 0) addip.FwdMessage = getMessageInfo(c.ForwardedMessages.ToList<VkNet.Model.Message>());
                addip.Id = (ulong)c.Id;
                messages.Add(addip);
            }
            return messages;
        }

        /// <summary> Пометить сообщение как прочитанное </summary>
        /// <param name="toMark"> Список сообщений для пометки </param>
        public void markAsRead(List<MessageInfo> toMark)
        {
            List<ulong> listToMark = new List<ulong>();
            foreach (var s in toMark) listToMark.Add(s.Id);
            api.Messages.MarkAsRead(listToMark);
        }

        /// <summary> Получение списка аудиозаписей </summary>
        /// <returns> Список аудизаписей </returns>
        public List<TrackInfo> getTracks()
        {
            var tracks = api.Audio.Get(selfId);
            return getTracksInfo(tracks);
        }

        /// <summary> Получение списка рекомендованных аудиозаписей </summary>
        /// <returns> Список аудизаписей </returns>
        public List<TrackInfo> getRecommendedTracks()
        {
            var tracks = api.Audio.GetRecommendations(selfId);
            return getTracksInfo(tracks);
        }

        /// <summary> Непосрредственно получение информации о треках </summary>
        /// <param name="tracks"> Коллекция аудиозаписей возвращенная VkApi</param>
        /// <returns> Список аудиозаписей </returns>
        List<TrackInfo> getTracksInfo(ReadOnlyCollection<VkNet.Model.Attachments.Audio> tracks)
        {
            List<TrackInfo> trackList = new List<TrackInfo>();
            foreach (var s in tracks)
            {
                var addip = new TrackInfo();
                addip.Artist = s.Artist;
                addip.Duration = s.Duration;
                addip.Title = s.Title;
                addip.Path = s.Url;
                trackList.Add(addip);
            }
            return trackList;
        }

        /// <summary> Отправка сообщения в выбранный диалог </summary>
        /// <param name="currentDialog"> Выбранный диалог </param> 
        /// <param name="messageText"> Текст сообщения </param> 
        public void SendMessage(DialogInfo currentDialog, string messageText)
        {
            MessageSendParams sendParams = new MessageSendParams();
            if (currentDialog.IsChat) sendParams.ChatId = currentDialog.Id;
            else sendParams.UserId = currentDialog.Id;
            sendParams.Message = messageText;

            api.Messages.Send(sendParams);
        }
    }
}
