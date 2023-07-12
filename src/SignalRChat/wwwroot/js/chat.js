
let toUserId = '';
let selectedGroupId = 0;


// Make scroll bar always at bottom
function scrollToBottom() {
    let messageBody = document.querySelector('#conversationContainer');
    messageBody.scrollTop = messageBody.scrollHeight - messageBody.clientHeight;
}

// function for loading the conversations
let loadConversions = (el, userId) => {
    toUserId = userId;
    selectedGroupId = 0;
    $('#GroupId').val(0);
    $(el).closest('li').siblings('li').removeClass('active');
    $(el).closest('li').addClass('active');
    $(`#loadConversions`).load(`/Chat/Chat/LoadConversationByUserId?toUserId=${userId}`, (response, status) => {
        $('.footer').removeClass('d-none');
        $('#ToUserId').val(userId);
        scrollToBottom();
        $(`#notify_${toUserId}`).text('');
        $(`#msgContent`).focus();

        // Unread message update
        $.post(`/Chat/Chat/UpdateVisibilityStatus`, { fromUserId: toUserId }, (result) => {
            console.log(result);
        });
    });
}

// function for loading the conversations
let loadConversionsByGroupId = (groupId) => {
    selectedGroupId = groupId;
    toUserId = '';
    $('#ToUserId').val('');
    $(`#loadConversions`).load(`/Chat/Chat/LoadConversionsByGroupId?groupId=${groupId}`, (response, status) => {
        $('.footer').removeClass('d-none');
        $('#GroupId').val(groupId);
        scrollToBottom();
        //$(`#notify_${toUserId}`).text('');
        $(`#msgContent`).focus();
        // Unread message update
        //$.post(`/Chat/Chat/UpdateVisibilityStatus`, { fromUserId: toUserId }, (result) => {
        //    console.log(result);
        //});
    });
}


let sendMessage = form => {
    try {
        let msgContent = $(`#msgContent`).val();
        let files = $('#Files').get(0);
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.isSuccess) {
                    setConversationToCaller(result, msgContent, files);
                    $(`#msgContent`).val('').trigger('input');
                    $(`#msgContent`).focus();
                    $('#Files').val("");
                    scrollToBottom();
                    //sendMessageToUser(conversation);          // this method is not called here because sendasync method has already called in controller
                }
                else {
                    toastr.error(`Message not sent`, `warning`);
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        return false;
    } catch (ex) {
        console.log(ex)
    }
}



// receive message from user the client
function addMessageToConversation(conversation) {

    // if message list box open then show message on message box otherwise increment notification count
    if (document.getElementById('loadConversions').contains(document.getElementById('messageList'))) {
        let toUserIdFromHeader = $('#toUserIdFromHeader').text();
        let toGroupIdFromHeader = $('#toGroupIdFromHeader').text();

        if (conversation.fromUserId == toUserIdFromHeader) {
            setConversationToChatBox(conversation)
            scrollToBottom();

            // Unread message update
            $.post(`/Chat/Chat/UpdateVisibilityStatus`, { fromUserId: conversation.fromUserId }, (result) => {
            });
        }
        else if (conversation.groupId > 0 && conversation.groupId == toGroupIdFromHeader) {
            setConversationWithUserToChatBox(conversation);
            scrollToBottom();
        }
        else {
            let notifyBadge = $(`#notify_${conversation.fromUserId}`);
            let notifyCount = $(notifyBadge).text();
            if (notifyCount) {
                $(notifyBadge).text(parseInt(notifyCount) + 1);
            }
            else {
                $(notifyBadge).text(0 + 1);
            }
        }
    }// if loadConversions does not contain messageList div
    else {
        let notifyBadge = $(`#notify_${conversation.fromUserId}`);
        let notifyCount = $(notifyBadge).text();
        if (notifyCount) {
            $(notifyBadge).text(parseInt(notifyCount) + 1);
        }
        else {
            $(notifyBadge).text(0 + 1);
        }
    }
    var audio = document.getElementById("notification-sound");
    audio.play();
}

// receive message from the Group client (Not working now)
function addMessageToGroupConversation(conversation) {
    if (document.getElementById('loadConversions').contains(document.getElementById('messageList'))) {
        setConversationWithUserToChatBox(conversation);
        scrollToBottom();
    } else {
        let notifyBadge = $(`#notify_${conversation.fromUserId}`);
        let notifyCount = $(notifyBadge).text();
        if (notifyCount) {
            $(notifyBadge).text(parseInt(notifyCount) + 1);
        }
        else {
            $(notifyBadge).text(0 + 1);
        }
    }
}

// All Active user make color green other black
function userConnectivity(connectedUserList) {
    let ulIdListList = document.querySelectorAll(`.userlistclass`);
    ulIdListList.forEach((value) => {
        let attrValue = value.getAttribute('id');
        let userIdValue = attrValue.split('_')[1];
        let isInclude = connectedUserList.includes(`${userIdValue}`);
        if (isInclude) {
            $(value).find(`.fa-circle`).removeClass('offline');
            $(value).find(`.fa-circle`).addClass('online');
        } else {
            $(value).find(`.fa-circle`).removeClass('online');
            $(value).find(`.fa-circle`).addClass('offline');
        }
    });
}


// create group
// Get modal for create or update User Group
function getCreateOrUpdateGroupModal(id, title = 'Title') {
    $.get(`/Chat/Chat/OnGetGroupCreateOrEdit?id=${id}`, (result) => {
        $('#formModal #modalHeaderTitle').html(title);
        $('#formModal .modal-body').html(result);
        $('#formModal').modal('show');
    });
}


// Get modal for assign or remove members from User Group
function getCreateOrUpdateUserGroupModal(groupId, title = '') {
    $.get(`/Chat/Chat/OnGetCreateOrEditUsersInGroup?groupId=${groupId}`, (result) => {
        $('#formModal #modalHeaderTitle').html(title);
        $('#formModal .modal-body').html(result);
        $('#formModal').modal('show');
    });
}

// Delete group
function deleteGroup(groupId) {
    var isConfirmed = confirm("Do you want to delete this group?");
    if (isConfirmed) {
        $.post(`/Chat/Chat/DeleteGroup`, { groupId: groupId }, (result) => {
            if (result.isSuccess) {
                location.reload();
            }
            else {
                toastr.error(`${result.msg}`);
            }
        });
    }
    else {
        return;
    }
}

// Set conversation to chatbox
function setConversationToChatBox(conversation) {
    if (conversation.textMessage) {
        $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                   ${conversation.textMessage.replace(/\n\r?/g, '<br />')}
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
    }

    if (conversation.conversationFiles.length > 0) {
        for (var i = 0; i < conversation.conversationFiles.length; i++) {
            let fileType = conversation.conversationFiles[i].fileType.split('/')[0];
            let fileName = conversation.conversationFiles[i].fileName;
            let fileSrc = conversation.conversationFiles[i].fileUrl;
            let fileSize = conversation.conversationFiles[i].fileSize;


            if (fileType == 'image') {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                   <img src="${fileSrc}" alt="file" onclick="showImageToModal('${fileSrc}')" class="img-fluid img-thumbnail ml-0 mr-0">
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
            else if (fileType == 'video') {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                   <span>${fileName}</span>
                                        <video controls>
                                            <source src="${fileSrc}">
                                        </video>
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
            else if (fileType == 'audio') {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                   <span>${fileName}</span>
                                        <audio controls>
                                            <source src="${fileSrc}">
                                        </audio>
                                    <br>
                                   <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
            else {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                        <span onclick="showFileInModal('${fileSrc}')" class="fileName"><i class="fa-solid fa-file-lines"></i>${fileName}</span>
                                        <span class="fileSize">${fileSize}</span>
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
        }
    }
}

// Set conversation to chatbox (<span class='fromUser'>${conversation.fromUserName}</span>)
function setConversationWithUserToChatBox(conversation) {
    debugger;
    if (conversation.textMessage) {
        $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                <span class='fromUser'>${conversation.fromUserName}</span>
                                   ${conversation.textMessage.replace(/\n\r?/g, '<br />')}
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
    }

    if (conversation.conversationFiles.length > 0) {
        for (var i = 0; i < conversation.conversationFiles.length; i++) {
            let fileType = conversation.conversationFiles[i].fileType.split('/')[0];
            let fileName = conversation.conversationFiles[i].fileName;
            let fileSrc = conversation.conversationFiles[i].fileUrl;
            let fileSize = conversation.conversationFiles[i].fileSize;


            if (fileType == 'image') {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                <span class='fromUser'>${conversation.fromUserName}</span>
                                   <img src="${fileSrc}" alt="file" onclick="showImageToModal('${fileSrc}')" class="img-fluid img-thumbnail ml-0 mr-0">
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
            else if (fileType == 'video') {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                <span class='fromUser'>${conversation.fromUserName}</span>
                                   <span>${fileName}</span>
                                        <video controls>
                                            <source src="${fileSrc}">
                                        </video>
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
            else if (fileType == 'audio') {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                <span class='fromUser'>${conversation.fromUserName}</span>
                                   <span>${fileName}</span>
                                        <audio controls>
                                            <source src="${fileSrc}">
                                        </audio>
                                    <br>
                                   <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
            else {
                $(`#messageList`).append(`<div class="message-box friend-message">
                                <p>
                                <span class='fromUser'>${conversation.fromUserName}</span>
                                        <span onclick="showFileInModal('${fileSrc}')" class="fileName"><i class="fa-solid fa-file-lines"></i>${fileName}</span>
                                        <span class="fileSize">${fileSize}</span>
                                    <br>
                                    <span>${conversation.toShortTime}</span>
                                </p>
                            </div>`);
            }
        }
    }
}

// Set conversation to caller
function setConversationToCaller(result, msgContent, files) {
    if (msgContent) {
        $(`#messageList`).append(`<div class="message-box my-message">
                                <p>
                                   ${msgContent.replace(/\n\r?/g, '<br />')}
                                    <br>
                                    <span>${result.time}</span>
                                </p>
                            </div>`);

    }
    if (files.files.length > 0) {
        for (var i = 0; i < files.files.length; i++) {
            let fileSrc = URL.createObjectURL(files.files[i])
            let fileType = files.files[i].type.split('/')[0];
            let fileName = files.files[i].name;
            let fileSize = files.files[i].size;

            if (fileType == 'image') {
                $(`#messageList`).append(`<div class="message-box my-message">
                                <p>
                                   <img src="${fileSrc}" alt="file" onclick="showImageToModal('${fileSrc}')" class="img-fluid img-thumbnail ml-0 mr-0">
                                    <br>
                                    <span>${result.time}</span>
                                </p>
                            </div>`);
            }
            else if (fileType == 'video') {
                $(`#messageList`).append(`<div class="message-box my-message">
                                <p>
                                   <span>@${fileName}</span>
                                        <video controls>
                                            <source src="${fileSrc}">
                                        </video>
                                    <br>
                                    <span>${result.time}</span>
                                </p>
                            </div>`);
            }
            else if (fileType == 'audio') {
                $(`#messageList`).append(`<div class="message-box my-message">
                                <p>
                                   <span>@${fileName}</span>
                                        <audio controls>
                                            <source src="${fileSrc}">
                                        </audio>
                                    <br>
                                    <span>${result.time}</span>
                                </p>
                            </div>`);
            }
            else {
                $(`#messageList`).append(`<div class="message-box my-message">
                                <p>
                                        <span onclick="showFileInModal('${fileSrc}')" class="fileName"><i class="fa-solid fa-file-lines"></i>${fileName}</span>
                                        <span class="fileSize">${fileSize}</span>
                                    <br>
                                    <span>${result.time}</span>
                                </p>
                            </div>`);
            }
        }
    }
}

// Set Notification 
function getNotifications(notification) {
    let currentCount = document.querySelector('#notificationCount').textContent;
    let newCount = parseInt(currentCount) + 1;
    document.querySelector('#notificationCount').textContent = newCount;
    document.querySelector('#notifyCountInAllSection').textContent = newCount;

    $(`#allNotifySection`).after(`<li>
                <hr class="dropdown-divider">
        </li>

        <li class="notification-item" href="javascript:void(0)" onclick="loadConversionsByGroupId('${notification.fromGroupId}')">
            <i class="bi bi-exclamation-circle text-warning"></i>
            <div>
                <h4>${notification.title}</h4>
                <p>${notification.text}</p>
                <p>A moment ago</p>
            </div>
        </li>
    `);
}