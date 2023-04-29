// برای مکالمه بین کاربران استفاده می شود
var _activeChat_ReciverUserId = '';

var chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chatroomhub")
    .build();

chatConnection.start();

// list user online
chatConnection.on("UpdateOnlineUsers", UpdateOnlineUsers);
chatConnection.on("GetLoadMessages", GetLoadMessages);
chatConnection.on("ReceiveMessage", ReceiveMessage);
chatConnection.on("UpdateArciveChatHistory", UpdateArciveChatHistory);
function UpdateOnlineUsers(onlineUsers) {
    //debugger;
    $("#online-users").html('');
    onlineUsers.forEach(function (onlineUser) {

        $("#online-users").append("<a class='list-group-item list-group-item-action d-flex justify-content-between align-items-center' data-id='" + onlineUser.userId + "' href='#'>" + onlineUser.nickname + "</a>");
    });
}


function GetLoadMessages(chatMessages) {
    //debugger;
    if (!chatMessages) return;
    chatMessages.forEach((elm, ind) => {
        showMessages(elm.nickname, elm.message, elm.timestamp, elm.seen);
    });
}

function ReceiveMessage(chatMessage) {
    //debugger;
    if (!chatMessage) return;
    if (chatMessage.senderId === _activeChat_ReciverUserId || chatMessage.reciverId === _activeChat_ReciverUserId) {
        showMessages(chatMessage.nickname, chatMessage.message, chatMessage.timestamp, chatMessage.seen);
    }
    if (chatMessage.senderId === _activeChat_ReciverUserId) {
        //debugger;
        chatConnection.invoke("UpdateSeenChatMessage", chatMessage.id);
    }
}

function UpdateArciveChatHistory(chatHistories) {
    //debugger;
    if (chatHistories == null) return;

    $("#chat-history-users").html('');
    chatHistories.forEach(chatHistoriy => {
        $("#chat-history-users").append("<a class='list-group-item list-group-item-action d-flex justify-content-between align-items-center' data-id='" + chatHistoriy.id + "' href='#'>" + chatHistoriy.nickname + "</a>");
    });
}


function showMessages(nickname, message, timestamp, seen) {
    // ✔️✔️✔️✔️✔️✔️✔️
    var seenCharachter = "";
    if (seen === true)
        seenCharachter = "✔️";
    $("#chatMessage").append("<li><div><span class='name'> " + nickname + " </span><span class='time'>" + timestamp + "</span><span>" + seenCharachter + "</span></div><div class='message'> " + message + " </div></li>");
}



// لیست کاربران آنلاین
var listUserOnline = document.getElementById('online-users');
// لیست کاربران تاریخچه
var listChatHistory = document.getElementById('chat-history-users');
// لیست پیام ها
var listMessagesEl = document.getElementById('chatMessage');

function removeAllChildren(node) {
    if (!node) return;
    while (node.lastChild) {
        node.removeChild(node.lastChild);
    }

}
// انتخاب کاربر آنلاین برای شروع چت
function setActiveUserButtonOnline(el) {
    var allButtonsOnline = listUserOnline.querySelectorAll('a.list-group-item');
    allButtonsOnline.forEach(function (btn) {
        btn.classList.remove('active');
    });
    el.classList.add('active');
}

// انتخاب کاربر تاریخچه برای مشاهده چت
function setActiveUserButtonHistory(el) {
    var allButtonsHistory = listChatHistory.querySelectorAll('a.list-group-item');
    allButtonsHistory.forEach(function (btn) {
        btn.classList.remove('active');
    });
    el.classList.add('active');
}

// آماده سازی شروع مکالمه
function switchActiveChatTo(reciverId) {
    if (reciverId === _activeChat_ReciverUserId) return;
    removeAllChildren(listMessagesEl);
    _activeChat_ReciverUserId = reciverId;
    if (reciverId != null)
        chatConnection.invoke('LoadMessages', reciverId)
}




listUserOnline.addEventListener('click', function (e) {
    setActiveUserButtonOnline(e.target);
    //debugger;
    var userId = e.target.getAttribute('data-id');
    switchActiveChatTo(userId);
});

listChatHistory.addEventListener('click', function (e) {
    setActiveUserButtonHistory(e.target);
    //debugger;
    var userId = e.target.getAttribute('data-id');
    switchActiveChatTo(userId);
});



function sendMessage(message) {
    if (message && message.length) {
        chatConnection.invoke('SendMessage', _activeChat_ReciverUserId, message);
    }
}


function Init() {
    // هر زمان برنامه دکمه ارسال فشرده شد، این اجرا می شود.
    var answerForm = $("#answerForm");
    answerForm.on('submit', function (e) {
        e.preventDefault();
        var message = e.target[0].value;
        sendMessage(message);

        e.target[0].value = '';
    })
}







$(document).ready(function () {
    Init();
})