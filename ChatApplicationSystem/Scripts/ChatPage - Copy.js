var proxy = $.connection.chatApplicationHub;
$(document).ready(function () {
    var _allUserList = [];
    var id = prompt("Please enter your Login ID");
    $.connection.hub.start().done(function (obj) {
        proxy.server.onlineUser(id);
        window.addEventListener('beforeunload', function (e) {
            proxy.server.OfflineUser(obj.id);
        });
    });
    //$.connection.hub.start().done(function (obj) {
    //    //preparAllUserChart(_allUserList);
    //    proxy.server.onlineUser(id);
    //    window.addEventListener('beforeunload', function (e) {
    //        proxy.server.offlineUser(obj.id);
    //    });
    //});
    //$.connection.hub.start().pipe(function () {
    //    return proxy.server.getAllUsers();
    //}).done(function (allNotification) {
    //    _allUserList = $.parseJSON(allUser);
    //    preparAllUserChart(_allUserList);
    //    proxy.server.onlineUser(id);
    //    window.addEventListener('beforeunload', function (e) {
    //        proxy.server.offlineUser(obj.id);
    //    });
    //});
    proxy.client.receiveOnlineUsers = function (jsonData) {
        //console.log(jsonData);
        var userList = JSON.parse(jsonData);

        preparAllUserChart(userList, id);
    };
    proxy.client.userNotAvailable = function () {
        alert("User Not Available");
    };

    function preparAllUserChart(userList, currentUserId) {
        debugger;
        console.log(userList);
        if (userList && userList.length > 0) {
            var userHtmlTemplate = recentChatUserTemplate();
            $('#div_Chat_Inbox').html('');
            $('#div_RecentChat_Area').html('');
            for (var i = 0; i < userList.length; i++) {
                if (currentUserId && currentUserId == userList[i].UserID) {
                    console.log("Current user");
                    console.log(currentUserId);
                    //
                } else {
                    var userHtml = userHtmlTemplate.replace(/\{{Name}}/g, userList[i].Name)
                        .replace(/\{{UserId}}/g, userList[i].UserID)
                        .replace('{{Profile_Url}}', 'https://ptetutorials.com/images/user-profile.png')
                        .replace('{{LastActive}}', userList[i].LastUserChat ? userList[i].LastUserChat.Time : "")
                        .replace('{{LastMessage}}', userList[i].LastUserChat ? userList[i].LastUserChat.Message : "");
                    $('#div_RecentChat_Area').prepend(userHtml);
                    var chatInboxTemplate = activeUserChatTemplate();
                    var chatInboxHtml = chatInboxTemplate.replace(/\{{Name}}/g, name)
                        .replace(/\{{Profile_Url}}/g, 'https://ptetutorials.com/images/user-profile.png')
                        .replace(/\{{UserId}}/g, id);
                    $('#div_Chat_Inbox').append(chatInboxHtml);
                }
            }
        }
    }
});

function recentChatUserTemplate() {
    return `<div class="chat_list" id="div_Recent_{{UserId}}" data-id="{{UserId}}">
                <div class="chat_people">
                    <div class="chat_img">
                        <img src="{{Profile_Url}}" alt="{{Name}}" />
                    </div>
                    <div class="chat_ib">
                        <h5>{{Name}} <span class="chat_date">{{LastActive}}</span></h5>
                        <div class="last-message-div">
                            {{LastMessage}}
                        </div>
                    </div>
                </div>
            </div>`;
}

function activeUserChatTemplate() {
    return `<div class="div-chat-inbox" id="div_Chat_History_{{UserId}}" style="display: none;">
                <div class="active-chat-inbox">
                    <div class="chat_img">
                        <img src="{{Profile_Url}}" alt="{{Name}}">
                    </div>
                    <div class="">
                        <h4 class="active-chat-user">{{Name}} <span class="chat_date"></span></h4>
                    </div>
                </div>
                <div class="msg_history">
                </div>
            </div>`;
}