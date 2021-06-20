<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChatApplicationSystem.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.css" type="text/css" rel="stylesheet" />
    <link href="Content/chat-theme.css" rel="stylesheet" />
    <script src="Scripts/jquery-3.3.1.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/moment.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="messaging-container">
            <div class="header-bg"></div>
            <div class="messaging-body">
                <div class="inbox_msg">
                    <div class="inbox_people">
                        <div class="headind_srch">
                            <div class="recent_heading">
                                <h4>Recent Chat</h4>
                            </div>
                            <%-- <div class="srch_bar">
                                <div class="stylish-input-group">
                                    <input type="text" class="search-bar" placeholder="Search"/>
                                    <span class="input-group-addon">
                                        <button type="button"><i class="fa fa-search" aria-hidden="true"></i></button>
                                    </span>
                                </div>
                            </div>--%>
                        </div>
                        <div class="inbox_chat" id="div_RecentChat_Area">
                        </div>
                    </div>
                    <div class="message">
                        <div id="div_Chat_Inbox">
                            <div id="div_Chat_Welcome_CurrentUser_{{UserID}}" class="Chat-Welcome-CurrentUser">
                                <div id="div_welcome_user">
                                    <div class="chat_img">
                                        <i class="fa fa-user user-icon welcome-user-icon" data-name="{{Name}}"></i>
                                    </div>
                                    <h3 id="head_CurrentUser"></h3>
                                    <h5>Please select a chat to start messaging..</h5>
                                </div>
                            </div>
                        </div>
                        <div class="type_msg" id="div_type_message" style="display: none">
                            <div class="input_msg_write">
                                <div class="msg-write-input">
                                    <%--<textarea spellcheck="true" class="write_msg" id="txtMessage" placeholder="Type a message"></textarea>--%>
                                    <input type="text" class="write_msg" id="txtMessagetxtMessage" placeholder="Type a message" />
                                </div>
                                <div class="send-msg-btn-group">
                                    <button class="" type="button"><i class="fa fa-paperclip" aria-hidden="true"></i></button>
                                    <button class="btn btn-send" id="btnSendMessage" type="button">Send <i class="fa fa-paper-plane-o" aria-hidden="true"></i></button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <input type="hidden" id="hdnName" />
            <input type="hidden" id="hdnId" />
        </div>
    </form>
    <script src="Scripts/jquery.signalR-2.4.1.min.js" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("~/signalr/hubs") %>" type="text/javascript"></script>
    <script src="Scripts/ChatPage.js"></script>
</body>
</html>
