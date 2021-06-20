var proxy = $.connection.chatApplicationHub;
$(document).ready(function () {
    var _allUserList = [];
    var id = prompt("Please enter your Login ID");

    $.connection.hub.start().pipe(function () {
        return proxy.server.getAllUsers();
    }).done(function (allUser) {
        console.log(allUser);
        _allUserList = $.parseJSON(allUser);
        preparAllUserChart(_allUserList, id);
        proxy.server.onlineUser(id);
        window.addEventListener('beforeunload', function (e) {
            //proxy.server.OfflineUser(obj.id);
        });
    });
    //$.connection.hub.start().done(function (obj) {
    //    proxy.server.onlineUser(id);
    //    window.addEventListener('beforeunload', function (e) {
    //        proxy.server.OfflineUser(obj.id);
    //    });
    //});
    proxy.client.receiveOnlineUsers = function (jsonData) {
        //console.log(jsonData);
        var userList = JSON.parse(jsonData);
        preparAllOnlineUserChart(userList, id);
    };
    proxy.client.userNotAvailable = function () {
        alert("User Not Available");
    };

    proxy.client.reciveNewMessage = function (sendById, message) {
        debugger;
        //Send messsage to specific user
        console.log(sendById + " - " + message);
        //openChat(sendById);
        buildReciveMessageToUser(sendById, message);
    };

    $(document).on('keyup', '.write_msg', function (e) {
        if (e.keyCode == 13) {
            var userId = $(this).data('id');
            $('#btnSendMessage_' + userId).trigger('click');
        }
    });

    $(document).on('click', '.chat_list', function () {
        $this = $(this);
        if ($this.data('id')) {
            var id = $this.data('id');
            openChat(id);
        }
    });

    $(document).on('click', '.btn-send-msg', function (e) {
        var userId = $(this).data('id');
        var $msgTextbox = $('#txtMessage_' + userId);
        if ($msgTextbox && $msgTextbox.val() != null && $msgTextbox.val().trim() != '') {
            sendMessageToUser(userId, $msgTextbox.val());
        }
    });

    function openChat(userId) {
        if (userId) {
            $('#div_Chat_Inbox .div-chat-inbox').hide();
            $('#div_Chat_History_' + userId).show();
            scrollToElement($('#div_Msg_History_' + userId + '>div:last'));
            $('#txtMessage_' + userId).focus();
        }
    }


    function sendMessageToUser(userId, message) {
        proxy.server.sendMessageToUser(userId, id, message);
        buildSendMessageToUser(userId, message);
        //TODO call api for send to reciver user
        

    }
    //Build Html for typed message
    //userId : which user you want to send
    function buildSendMessageToUser(userId, message) {
        if (userId) {
            var $msgChat = $('#div_Msg_History_' + userId);
            if ($msgChat) {
                var sendMsgHtml = outgoingMessageHtmlTemplate();
                sendMsgHtml = sendMsgHtml.replace(/\{{UserId}}/g, userId).replace(/\{{Message}}/g, message).replace(/\{{Time}}/g, '11:01 AM    |    Today'); 
                $msgChat.append(sendMsgHtml);
                scrollToElement($('.outgoing_msg[data-id="' + userId + '"]:last'));
                $('#txtMessage_' + userId).val('');
                $('#txtMessage_' + userId).focus();
            }
        }
    }

    //Build Html for typed message
    //userId : which user you want to send
    function buildReciveMessageToUser(userId, message) {
        if (userId) {
            var $msgChat = $('#div_Msg_History_' + userId);
            if ($msgChat) {
                var sendMsgHtml = incomingMessageHtmlTemplate();
                sendMsgHtml = sendMsgHtml.replace(/\{{UserId}}/g, userId).replace(/\{{Message}}/g, message).replace(/\{{Time}}/g, '11:01 AM    |    Today');
                $msgChat.append(sendMsgHtml);
                //scrollToElement($('.incoming_msg[data-id="' + userId + '"]:last'));
            }
        }
    }

    function preparAllOnlineUserChart(userList, currentUserId) {
        debugger;
        console.log(userList);
        $('.chat_list .chat_img').removeClass('online');
        if (userList && userList.length > 0) {
            for (var i = 0; i < userList.length; i++) {
                var $recent = $('#div_Recent_' + userList[i].UserID);
                if ($recent) {
                    $recent.find('.chat_img').addClass('online');
                }
            }
        }
    }

    function preparAllUserChart(userList, currentUserId) {
        debugger;
        console.log(userList);
        if (userList && userList.length > 0) {
            var userHtmlTemplate = recentChatUserTemplate();
            $('#div_Chat_Inbox').html('');
            $('#div_RecentChat_Area').html('');
            for (var i = 0; i < userList.length; i++) {
                if (currentUserId && currentUserId == userList[i].ID) {
                    console.log("Current user");
                    console.log(currentUserId);
                    //
                } else {
                    var userHtml = userHtmlTemplate.replace(/\{{Name}}/g, userList[i].Name)
                        .replace(/\{{UserId}}/g, userList[i].ID)
                        .replace('{{Profile_Url}}', 'https://ptetutorials.com/images/user-profile.png')
                        .replace('{{LastActive}}', userList[i].LastUserChat ? userList[i].LastUserChat.Time : "")
                        .replace('{{LastMessage}}', userList[i].LastUserChat ? userList[i].LastUserChat.Message : "");
                    $('#div_RecentChat_Area').prepend(userHtml);
                    var chatInboxTemplate = activeUserChatTemplate();
                    var chatInboxHtml = chatInboxTemplate.replace(/\{{Name}}/g, userList[i].Name)
                        .replace(/\{{Profile_Url}}/g, 'https://ptetutorials.com/images/user-profile.png')
                        .replace(/\{{UserId}}/g, userList[i].ID);
                    $('#div_Chat_Inbox').append(chatInboxHtml);
                }
            }
        }
    }
});


function scrollToElement(element) {
    if (element && element.length > 0) { element[0].scrollIntoView(); }
}
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
    return `<div class="div-chat-inbox" id="div_Chat_History_{{UserId}}" data-id="{{UserId}}" style="display: none;">
                <div>
                    <div class="active-chat-inbox">
                        <div class="chat_img">
                            <img src="{{Profile_Url}}" alt="{{Name}}">
                        </div>
                        <div class="active-chat-name">
                            <h4 class="active-chat-user">{{Name}} <span class="chat_date"></span></h4>
                        </div>
                        <div class="div-chat-menu hide"><i class="fa fa-bars"></i></div>
                    </div>
                    <div class="msg_history" id="div_Msg_History_{{UserId}}" data-id="{{UserId}}">
                        
                    </div>
                <div>
                <div class="type_msg" id="div_type_message_{{UserId}}">
                    <div class="input_msg_write">
                        <div class="msg-write-input">
                            <textarea spellcheck="true" class="write_msg hide" placeholder="Type a message"></textarea>
                            <input type="text" class="write_msg" id="txtMessage_{{UserId}}" data-id="{{UserId}}" placeholder="Type a message" />
                        </div>
                        <div class="send-msg-btn-group">
                            <button class="hide" type="button"><i class="fa fa-paperclip" aria-hidden="true"></i></button>
                            <button class="btn btn-send-msg" id="btnSendMessage_{{UserId}}" type="button" data-id="{{UserId}}">Send <i class="fa fa-paper-plane-o" aria-hidden="true"></i></button>
                        </div>
                    </div>
                </div>
            </div>`;
}

function outgoingMessageHtmlTemplate() {
    return `<div class="outgoing_msg" data-id="{{UserId}}">
                <div class="sent_msg">
                    <p>{{Message}}</p>
                    <span class="time_date">{{Time}}</span>
                </div>
            </div>`;
}
function incomingMessageHtmlTemplate() {
    return `<div class="incoming_msg" data-id="{{UserId}}">
                <div class="incoming_msg_img">
                    <img src="https://ptetutorials.com/images/user-profile.png">
                </div>
                <div class="received_msg">
                    <div class="received_withd_msg">
                        <p>
                            {{Message}}
                        </p>
                        <span class="time_date">{{Time}}</span>
                    </div>
                </div>
            </div>`;
}