var proxy = $.connection.chatApplicationHub;
$(document).ready(function () {
    var _allUserList = [];
    var loginID = prompt("Please enter your Login ID");

    $.connection.hub.start().pipe(function () {
        //return proxy.server.getLoginUserId();
        return proxy.server.userConnect(loginID);
    }).done(function (obj) {
        //id = userId;
        window.addEventListener('beforeunload', function (e) {
            //proxy.server.OfflineUser(id);
        });
    });

    proxy.client.newOnlineUsers = function (onlineUserJson, availableUsersWithRecentChatJson) {
        if (availableUsersWithRecentChatJson) {
            var availableUsersWithRecentChat = $.parseJSON(availableUsersWithRecentChatJson);
            _allUserList = availableUsersWithRecentChat.Item1;
            preparAllUserChart(_allUserList, availableUsersWithRecentChat.Item2, loginID);
        }
        var userList = JSON.parse(onlineUserJson);
        preparAllOnlineUserChart(userList, loginID);
    };

    proxy.client.receiveOnlineUsers = function (onlineUserJson) {
        var userList = JSON.parse(onlineUserJson);
        preparAllOnlineUserChart(userList, loginID);
    };
    proxy.client.userNotAvailable = function () {
        alert("User Not Available");
    };

    proxy.client.reciveNewMessage = function (sendById, chatJson) {
        debugger;
        //Send messsage to specific user
        var chat = $.parseJSON(chatJson);
        buildReciveMessageToUser(sendById, chat);
    };

    $(document).on('keyup', '.write_msg', function (e) {
        if (e.keyCode == 13) {
            var userId = $(this).data('id');
            $('#btnSendMessage_' + userId).trigger('click');
        }
    });

    $(document).on('click', '.chat_list', function () {
        $this = $(this);
        $('#div_RecentChat_Area .chat_list').removeClass('active_chat');
        $this.addClass('active_chat');
        if ($this.data('id')) {
            var reveiverID = $this.data('id');
            proxy.server.getUserChats(loginID, reveiverID).done(function (obj) {
                var userChat = $.parseJSON(obj);
                $('#UnreadMessageCount_' + reveiverID).html('');
                $('.Chat-Welcome-CurrentUser').hide();
                fillUserChatData(userChat, reveiverID, loginID);
            }).fail(function (obj) {
                $('.Chat-Welcome-CurrentUser').show();
                $('.chat_list').hide();
            });
        }
    });

    $(document).on('click', '.btn-send-msg', function (e) {
        var userId = $(this).data('id');
        var $msgTextbox = $('#txtMessage_' + userId);
        if ($msgTextbox && $msgTextbox.val() != null && $msgTextbox.val().trim() != '') {
            sendMessageToUser(userId, $msgTextbox.val());
        }
    });

    function fillUserChatData(userChat, reveiverID, loginID) {
        console.log(userChat);
        if (userChat && userChat.length > 0) {
            $('#div_Msg_History_' + reveiverID).html('');
            for (var i = 0; i < userChat.length; i++) {
                var chat = userChat[i];
                //Incomein message
                if (chat.SendByID == reveiverID) {
                    buildReciveMessageToUser(reveiverID, chat);
                }
                //Outgoing messages
                else if (chat.SendByID == loginID) {
                    buildSendMessageToUser(reveiverID, chat);
                }
            }
        }
        $('#div_Chat_Inbox .div-chat-inbox').hide();
        $('#div_Chat_History_' + reveiverID).show();
        scrollToElement($('#div_Msg_History_' + reveiverID + '>div:last'));
        $('#txtMessage_' + reveiverID).focus();
    }


    function sendMessageToUser(userId, message) {
        proxy.server.sendMessageToUser(loginID, userId, message).done(function (obj) {
            if (obj) {
                var chat = $.parseJSON(obj);
                buildSendMessageToUser(userId, chat);
            }
        });
    }
    //Build Html for typed message
    //userId : which user you want to send
    function buildSendMessageToUser(userId, chat) {
        if (userId) {
            var $msgChat = $('#div_Msg_History_' + userId);
            if ($msgChat) {
                var sendMsgHtml = outgoingMessageHtmlTemplate();
                sendMsgHtml = sendMsgHtml.replace(/\{{UserId}}/g, userId).replace(/\{{Message}}/g, chat.Message).replace(/\{{Time}}/g, DateFormate(chat.SendTime, 'DD/MM/YYYY hh:mm a')); 
                $msgChat.append(sendMsgHtml);
                scrollToElement($('.outgoing_msg[data-id="' + userId + '"]:last'));
                $('#txtMessage_' + userId).val('');
                $('#txtMessage_' + userId).focus();
            }
        }
    }

    //Build Html for typed message
    //userId : which user you want to send
    function buildReciveMessageToUser(userId, chat) {
        if (userId) {
            var $msgChat = $('#div_Msg_History_' + userId);
            if ($msgChat) {
                var sendMsgHtml = incomingMessageHtmlTemplate();
                sendMsgHtml = sendMsgHtml.replace(/\{{UserId}}/g, userId).replace(/\{{Message}}/g, chat.Message).replace(/\{{Time}}/g, DateFormate(chat.SendTime, 'DD/MM/YYYY hh:mm a'));
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

    function preparAllUserChart(userList, recentChats, currentUserId) {
        if (userList && userList.length > 0) {
            var userHtmlTemplate = recentChatUserTemplate();
            $('#div_Chat_Inbox').html('');
            $('#div_RecentChat_Area').html('');
            var userRecentChatData = arrangeRecentUserChatData(userList, recentChats);
            userRecentChatData = userRecentChatData.sort(function compare(a, b) {
                if (a.LastMessageTime) {
                    debugger;
                }
                var dateA = Date(a.LastMessageTime);
                var dateB = Date(b.LastMessageTime);
                return dateA - dateB;
            });
            //if (userRecentChatData != null && userRecentChatData.length > 0) {
            //    userRecentChatData = userRecentChatData.sort(function (a, b) {  return a.LastMessageTime - b.LastMessageTime });
            //}
            var currentUser = userList.filter(x => x.ID == currentUserId);
            if (currentUser && currentUser.length > 0) {
                var chatInboxTemplate = welcomeUserHtmlTemplate();
                var chatInboxHtml = chatInboxTemplate.replace(/\{{Name}}/g, currentUser[0].Name)
                    .replace(/\{{UserId}}/g, currentUser[0].ID);
                $('#div_Chat_Inbox').append(chatInboxHtml);
            }
            
            for (var i = 0; i < userRecentChatData.length; i++) {
                if (currentUserId && currentUserId == userRecentChatData[i].UserID) {
                    
                } else {
                    var userHtml = userHtmlTemplate.replace(/\{{Name}}/g, userRecentChatData[i].Name)
                        .replace(/\{{UserId}}/g, userRecentChatData[i].UserID)
                        .replace('{{LastActive}}', DateFormate(userRecentChatData[i].LastMessageTime,'DD/MM/YYYY'))
                        .replace('{{LastMessage}}', userRecentChatData[i].LastMessage)
                        .replace('{{UnreadMessageCount}}', '');
                        //.replace('{{UnreadMessageCount}}', userRecentChatData[i].UnreadMessageCount);
                    $('#div_RecentChat_Area').append(userHtml);
                    var chatInboxTemplate = activeUserChatTemplate();
                    var chatInboxHtml = chatInboxTemplate.replace(/\{{Name}}/g, userRecentChatData[i].Name)
                        .replace(/\{{UserId}}/g, userRecentChatData[i].UserID);
                    $('#div_Chat_Inbox').append(chatInboxHtml);
                }
            }
        }

    }

    function arrangeRecentUserChatData(userList, recentChats) {
        var rtn = [];
        if (userList != null && userList.length > 0) {
            for (var i = 0; i < userList.length; i++) {
                var recentChat = {};
                recentChat.UserID = userList[i].ID;
                recentChat.Name = userList[i].Name;
                recentChat.ID = userList[i].ID;
                recentChat.ReceiveByID = userList[i].ID;
                recentChat.SendByID = "";
                recentChat.LastMessage = "";
                recentChat.LastMessageTime = "";
                recentChat.UnreadMessageCount = "";
                if (recentChats != null && recentChats.length > 0) {
                    var userChat = recentChats.filter(x => x.ReceiveByID == userList[i].ID);
                    if (userChat && userChat.length > 0) {
                        recentChat.SendByID = userChat[0].SendByID;
                        recentChat.LastMessage = userChat[0].LastMessage;
                        recentChat.LastMessageTime = userChat[0].LastMessageTime;
                        recentChat.UnreadMessageCount = userChat[0].UnreadMessageCount;
                    }
                }
                rtn.push(recentChat);
            }
        }
        return rtn;
    }

    function setRecentChatUI(recentChats) {
        if (recentChats && recentChats.length > 0) {
            for (var i = 0; i < recentChats.length; i++) {
                var chat = recentChats[i];
                //$('#div_Recent_' + chat.ReceiveByID)
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
                        <i class="fa fa-user user-icon" data-name="{{Name}}"></i>
                    </div>
                    <div class="chat_ib">
                        <h5>{{Name}} <span class="chat_date">{{LastActive}}</span></h5>
                        <div>
                            <span class="last-message-div">{{LastMessage}}</span>
                            <span class="unread-message-count" id="UnreadMessageCount_{{UserId}}">{{UnreadMessageCount}}</span>
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
                            <i class="fa fa-user user-icon" data-name="{{Name}}"></i>
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
                    <i class="fa fa-user user-icon"></i>
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

function welcomeUserHtmlTemplate() {
    return `<div id="div_Chat_Welcome_CurrentUser_{{UserID}}" class="Chat-Welcome-CurrentUser">
                <div id="div_welcome_user">
                    <div class="chat_img">
                        <i class="fa fa-user user-icon welcome-user-icon" data-name="{{Name}}"></i>
                    </div>
                    <h3 id="head_CurrentUser">Hii {{Name}}</h3>
                    <h5>Please select a chat to start messaging..</h5>
                </div>
            </div>`
}

function DateFormate(date, formate) {
    var formateDate = date;
    if (!formate) {
        formate = 'DD/MM/YYYY';
    }
    if (date) {
        formateDate = moment(date).format(formate);
    }
    return formateDate;
}

