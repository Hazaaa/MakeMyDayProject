// JS part
var numberOfShownPosts = 10;

// Getting 10 latest posts from db
function GetLatestPosts(loggedin, username) {
    // Getting posts async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-latest-posts',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (loggedin == true) {
                for (var i = 0; i < data.length; i++) {
                    GeneratePostBody(data[i], true, false, username);
                    
                }
            }
            else {
                for (var i = 0; i < data.length; i++) {
                    GeneratePostBody(data[i], false, false);
                }
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Load more posts
function LoadMorePosts() {
    var user = GetLoggedUsername();
    var loggedin = GetIfLogged();
    var loadMoreButton = document.getElementById("loadMoreButton");
    // Getting posts async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-posts/' + numberOfShownPosts ,
        success: function (data) {
            if (loggedin == true) {
                for (var i = 0; i < data.length; i++) {
                    GeneratePostBody(data[i], true, false, user);
                }
                if (data.length !== 10) {
                    loadMoreButton.style.visibility = "hidden";
                }
                else {
                    loadMoreButton.style.visibility = "visible";
                }
            }
            else {
                for (var i = 0; i < data.length; i++) {
                    GeneratePostBody(data[i], false, false);
                }
                if (data.length !== 10) {
                    loadMoreButton.style.visibility = "hidden";
                }
                else {
                    loadMoreButton.style.visibility = "visible";
                }
            }
            numberOfShownPosts += 10;
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Dynamically creating body for posts
function GeneratePostBody(post, loggedin, newPost, username) {
    // Here we will append posts
    var postContainter = document.getElementById("postContainter");

    // Post Card (bootstrap)
    var postCardMainDiv = document.createElement("div");
    postCardMainDiv.style.width = "32rem";
    postCardMainDiv.style.textAlign = "left";
    postCardMainDiv.style.marginBottom = "10px";
    postCardMainDiv.className = "card border-danger"
    postCardMainDiv.id = "post-" + post.id;

    // Card header
    var cardHeaderDiv = document.createElement("div");
    cardHeaderDiv.className = "card-header bg-danger";
    cardHeaderDiv.style.textAlign = "left";

    // Profile picture baloon with username and time created
    var profilePictureDiv = document.createElement("div");
    profilePictureDiv.className = "profile-picture";

    var picture = document.createElement("img");
    if (newPost === true)
        picture.src = post.creator.profilepictureurl.replace(/~/g, "");
    else
        picture.src = post.creatorPict.replace(/~/g, "");
    
    picture.width = "50";
    picture.height = "50";
    picture.className = "rounded-circle";
    picture.style.border = "0.5px solid gray";

    var usernameSpan = document.createElement("span");
    usernameSpan.className = "username";
    usernameSpan.style.paddingLeft = "15px";
    usernameSpan.style.fontFamily = "'Franklin Gothic Medium'";
    usernameSpan.style.fontSize = "20px";
    usernameSpan.style.color = "white";

    var linkToUser = document.createElement("a");
    linkToUser.href = '/Profile/' + post.creator.username;
    linkToUser.style.color = "white";
    linkToUser.innerHTML = post.creator.username;

    usernameSpan.appendChild(linkToUser);

    var timepostedspan = document.createElement("span");
    timepostedspan.className = "timeposted";
    timepostedspan.style.fontFamily = "'Franklin Gothic Medium'";
    timepostedspan.style.fontSize = "10px";
    timepostedspan.style.color = "lightgray";
    timepostedspan.innerHTML = " - " + post.timeCreated;

    // Appending to card header
    profilePictureDiv.appendChild(picture);
    profilePictureDiv.appendChild(usernameSpan);
    profilePictureDiv.appendChild(timepostedspan);

    // Adding delete icon
    if (post.creator.username === username) {
        var iconDelete = document.createElement("i");
        iconDelete.className = "fas fa-trash-alt";
        iconDelete.title = "Delete comment.";
        iconDelete.style.color = "white";
        iconDelete.style.marginLeft = "10px";
        iconDelete.id = post.id;

        iconDelete.onclick = function () {
            DeletePost(post.id);
        };

        profilePictureDiv.appendChild(iconDelete);
    }

    cardHeaderDiv.appendChild(profilePictureDiv);
    postCardMainDiv.appendChild(cardHeaderDiv);


    // Card body (post content)
    var bodyDiv = document.createElement("div");
    bodyDiv.className = "card-body";

    // Content text
    var content = document.createElement("p");
    content.className = "card-text";
    content.innerHTML = ReplaceHashtagsAndTags(post.content);

    // Appending to card body
    bodyDiv.appendChild(content);
    postCardMainDiv.appendChild(bodyDiv);

    // Including picture to post if there is one
    if (post.pictureurl !== null) {
        var postPicture = document.createElement("img");
        postPicture.className = "card-img-bottom";
        postPicture.src = post.pictureurl.replace(/~/g, "");
        postPicture.alt = "Card image cap";

        // Appending picture to card
        postCardMainDiv.appendChild(postPicture);
    }

    // Like and dislike buttons
    var cardFooterDiv = document.createElement("div");
    cardFooterDiv.className = "card-footer bg-transparent border-danger";

    var rowForButtons = document.createElement("div");
    rowForButtons.className = "row";

    var buttonsDiv = document.createElement("div");
    buttonsDiv.className = "btn-group btn-group-sm offset-4";
    buttonsDiv.setAttribute("role", "group");
    buttonsDiv.setAttribute("aria-label", "options-" + post.id);


    var likeButton = document.createElement("button");
    likeButton.type = "button";
    likeButton.id = "like-" + post.id;
    likeButton.className = "btn btn-danger";

    if (loggedin === true) {
        likeButton.onclick = function () {
            LikeDislikePost(post.id, "like");
        };
    }

    var likeIcon = document.createElement("i");
    likeIcon.className = "fas fa-thumbs-up";

    likeButton.innerHTML = "<i class='fas fa-thumbs-up'></i> Likes (" + post.likes + ")";

    var dislikeButton = document.createElement("button");
    dislikeButton.type = "button";
    dislikeButton.id = "dislike-" + post.id;
    dislikeButton.className = "btn btn-danger";

    if (loggedin === true) {
        dislikeButton.onclick = function () {
            LikeDislikePost(post.id, "dislike");
        };
    }
    

    var dislikeIcon = document.createElement("i");
    dislikeIcon.className = "fas fa-thumbs-down";

    dislikeButton.innerHTML = "<i class='fas fa-thumbs-down'></i> Dislikes (" + post.dislikes + ")";

    buttonsDiv.appendChild(likeButton);
    buttonsDiv.appendChild(dislikeButton);

    rowForButtons.appendChild(buttonsDiv);
    cardFooterDiv.appendChild(rowForButtons);
    
    cardFooterDiv.appendChild(document.createElement("hr"));

    // Comments part
    var commentsRow = document.createElement("div");
    commentsRow.className = "row";
    commentsRow.id = "commentsRow-" + post.id;

    var numOfComments = document.createElement("p");
    numOfComments.id = "numOfComments-" + post.id;
    numOfComments.className = "offset-5";
    numOfComments.innerHTML = "See comments";
    numOfComments.style.color = "blue";
    numOfComments.style.textDecoration = "underline";

    if (loggedin == true) {
        numOfComments.onclick = function () {
            commentsRow.innerHTML = "";
            GenerateCommentsForPost(post.id, username);
        }
    }
    else {
        numOfComments.onclick = function () {
            window.location.href = '../Account/Register';
        };
    }

    commentsRow.appendChild(numOfComments);
    cardFooterDiv.appendChild(commentsRow);

    // Appendding buttons and comments to card footer div
    postCardMainDiv.appendChild(cardFooterDiv);

    // Appending to postcontainer
    if (newPost)
        postContainter.insertBefore(postCardMainDiv, postContainter.childNodes[5]);
    else
        postContainter.appendChild(postCardMainDiv);
}

// Function for like or dislike post
function LikeDislikePost(postId, choice) {
    if (choice === "like") {
        $.ajax({
            type: "POST",
            url: '/api/like-post',
            data: { 'id': postId, 'username': GetLoggedUsername()},
            success: function (data) {
                if (data !== null) {
                    var likeButton = document.getElementById("like-" + postId);
                    var dislikeButton = document.getElementById("dislike-" + postId);

                    likeButton.setAttribute("disabled", "disabled");

                    var valLike = likeButton.innerHTML.split('(')[1].split(')')[0];
                    valLike++;

                    likeButton.innerHTML = "<i class='fas fa-thumbs-up'></i> Likes (" + valLike + ")";

                    if (dislikeButton.hasAttribute("disabled")) {
                        dislikeButton.removeAttribute("disabled");

                        var valDislike = dislikeButton.innerHTML.split('(')[1].split(')')[0];

                        if (valDislike != 0)
                            valDislike--;

                        dislikeButton.innerHTML = "<i class='fas fa-thumbs-down'></i> Dislikes (" + valDislike + ")";
                    }
                }
                else {
                    alert("Error while trying to like post :(");
                }
            }
        });
    }
    else {
        $.ajax({
            type: "POST",
            url: '/api/dislike-post',
            data: { 'id': postId, 'username': GetLoggedUsername() },
            success: function (data) {
                if (data !== null) {
                    var likeButton = document.getElementById("like-" + postId);
                    var dislikeButton = document.getElementById("dislike-" + postId);

                    dislikeButton.setAttribute("disabled", "disabled");

                    var valDislike = dislikeButton.innerHTML.split('(')[1].split(')')[0];
                    valDislike++;

                    dislikeButton.innerHTML = "<i class='fas fa-thumbs-down'></i> Dislikes (" + valDislike + ")";
                    

                    if (likeButton.hasAttribute("disabled")) {
                        likeButton.removeAttribute("disabled");

                        var valLike = likeButton.innerHTML.split('(')[1].split(')')[0];

                        if (valLike != 0)
                            valLike--;

                        likeButton.innerHTML = "<i class='fas fa-thumbs-up'></i> Likes (" + valLike + ")";
                    }
                }
                else {
                    alert("Error while trying to dislike post :(");
                }
            }
        });
    }
}

// Uploads post to db
function UploadPost() {
    var postText = document.getElementById("newPost");
    if (postText.value === "")
        return;

    var formData = new FormData();

    var hashTagsAndTags = ValidationForHashtagAndTag(postText.value);

    var uploadedPicture = document.getElementById("file-upload").files[0];

    formData.append("uploadedPicture", uploadedPicture);

    var post = {
        Text: postText.value,
        Hashtags: hashTagsAndTags.Hashtags,
        Tags: hashTagsAndTags.Tags
    };

    formData.append("post", JSON.stringify(post));

    $.ajax({
        type: "POST",
        url: '/api/upload-post',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data !== null) {
                GeneratePostBody(data, true, true, GetLoggedUsername());
                postText.value = "";

                var numOfPosts = document.getElementById("numOfPostsP");
                var num = parseInt(numOfPosts.innerHTML);
                numOfPosts.innerHTML = ++num;
            }
            else {
                alert("Error while trying to uplad post :(");
            }
        }
    });
}

// Gets hashtags and tags from input text
function ValidationForHashtagAndTag(text) {

    var regexHashtag = /\s#([\d\w])+/;
    var regexTag = /\s@([\d\w])+/;

    var hashtags = [];
    var tags = [];

    if (regexHashtag.test(text)) {
        var splittedText = text.split(" ");

        for (var i = 0; i < splittedText.length; i++) {
            if (splittedText[i][0] === "#") {
                hashtags.push(splittedText[i]);
            }
        }
    }

    if (regexTag.test(text)) {
        var splittedText = text.split(" ");

        for (var i = 0; i < splittedText.length; i++) {
            if (splittedText[i][0] === "@") {
                tags.push(splittedText[i]);
            }
        }
    }

    var objectForReturn = {
        Hashtags: hashtags,
        Tags: tags,
    };

    return objectForReturn;
}

// Replace hashtags and tags from text to <a>
function ReplaceHashtagsAndTags(text) {

    // Getting if content got any hashtags or tags so we can replace them with <a>
    var hashtagsAndTags = ValidationForHashtagAndTag(text);

    var tempContent = text;

    // Replacing hashtags with link to hashtag page with all posts that contains that hashtag
    if (hashtagsAndTags.Hashtags.length != 0) {
        for (var i = 0; i < hashtagsAndTags.Hashtags.length; i++) {
            tempContent = tempContent.replace(hashtagsAndTags.Hashtags[i], " <a href='../hashtag/" + hashtagsAndTags.Hashtags[i].replace(/#/g, "") + "'>" + hashtagsAndTags.Hashtags[i] + "</a> ");
        }
    }

    // Replacing tags with link to user profile that is tagged
    if (hashtagsAndTags.Tags.length != 0) {
        for (var i = 0; i < hashtagsAndTags.Tags.length; i++) {
            tempContent = tempContent.replace(hashtagsAndTags.Tags[i], " <a href='../profile/" + hashtagsAndTags.Tags[i].replace(/@/g, "") + "'>" + hashtagsAndTags.Tags[i] + "</a> ");
        }
    }

    return tempContent;
}

// Getting comments for post
function GetCommentsForPost(postId) {
    // Getting comments async
    var returnData = "";
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-post-comments/' + postId,
        async: false,
        success: function (data) {
            returnData = data;
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
    return returnData;
}

// Uploads comment to db
function UploadComment(postId, username) {
    var commentInput = document.getElementById("newComment-" + postId);
    if (commentInput.value !== "") {
        $.ajax({
            type: "POST",
            url: '/api/upload-comment',
            data: { 'postId': postId, 'text': commentInput.value },
            success: function (data) {
                if (data != null) {

                    var commentsUl = document.getElementById("commentsMainUl-" + postId);

                    CreateCommentBody(postId, data, username, commentsUl, true);
                    commentInput.value = "";
                }
                else {
                    alert("Error while uploading comment :(");
                }
            }
        });
    }
    else
        return;
}

// Dynamically creating comment body
function CreateCommentBody(postId, comment, username, commentsMainUl, newComment) {
    var commentLi = document.createElement("li");
    commentLi.className = "list-group-item";
    commentLi.id = "comment-" + comment.id;
    commentLi.style.width = "100%";


    var usernameSpan = document.createElement("span");
    usernameSpan.className = "username";
    usernameSpan.style.paddingLeft = "10px";
    usernameSpan.style.fontFamily = "'Franklin Gothic Medium'";
    usernameSpan.style.fontSize = "15px";
    usernameSpan.style.color = "white";

    var linkToUser = document.createElement("a");
    linkToUser.href = '/Profile/' + comment.creator.username;
    linkToUser.style.color = "#dc3545";
    linkToUser.innerHTML = comment.creator.username;

    usernameSpan.appendChild(linkToUser);

    var timecommentedspan = document.createElement("span");
    timecommentedspan.className = "timeposted";
    timecommentedspan.style.fontFamily = "'Franklin Gothic Medium'";
    timecommentedspan.style.fontSize = "10px";
    timecommentedspan.style.color = "lightgray";
    timecommentedspan.innerHTML = " - " + comment.time + "  ";

    commentLi.appendChild(usernameSpan);
    commentLi.appendChild(timecommentedspan);


    if (username === comment.creator.username) {
        var iconEdit = document.createElement("i");
        iconEdit.className = "fas fa-pen fa-xs";
        iconEdit.title = "Edit comment.";
        iconEdit.style.color = "#dc3545";
        iconEdit.style.marginRight = "10px";
        iconEdit.style.marginLeft = "10px";
        iconEdit.id = comment.id;

        iconEdit.onclick = function () {
            var buttonEdit = document.getElementById("editButtonComment-" + iconEdit.id);
            buttonEdit.style.visibility = "visible";

            var textInput = document.getElementById("inputComment-" + iconEdit.id);
            textInput.removeAttribute("disabled");
            textInput.style.borderBottom = "1px solid black";
        };

        commentLi.appendChild(iconEdit);

        var iconDelete = document.createElement("i");
        iconDelete.className = "fas fa-trash-alt fa-xs";
        iconDelete.title = "Delete comment.";
        iconDelete.style.color = "#dc3545";
        iconDelete.id = comment.id;

        iconDelete.onclick = function () {
            DeleteComment(iconDelete.id, postId);
        };

        commentLi.appendChild(iconDelete);
    }


    var textDiv = document.createElement("div");

    var textP = document.createElement("input");
    textP.type = "text";
    textP.value = comment.text;
    textP.style.marginTop = "5px";
    textP.style.marginLeft = "10px";
    textP.style.maxWidth = "380px";
    textP.style.width = "80%";
    textP.style.border = "none";
    textP.disabled = "true";
    textP.id = "inputComment-" + comment.id;

    textDiv.appendChild(textP);

    if (username === comment.creator.username) {
        var editButton = document.createElement("button");
        editButton.className = "btn btn-outline-danger btn-sm";
        editButton.innerHTML = "<i class='fas fa-pen'></i> Save";
        editButton.style.visibility = "hidden";
        editButton.id = "editButtonComment-" + comment.id;
        editButton.style.marginLeft = "10px";

        editButton.onclick = function () {
            var textInput = document.getElementById("inputComment-" + editButton.id.replace(/editButtonComment-/g, ""));
            UpdateComment(editButton.id.replace(/editButtonComment-/g, ""), textInput.value);
        };

        textDiv.appendChild(editButton);
    };

    commentLi.appendChild(textDiv);

    // Appending to comments row
    if (newComment === true) {
        commentsMainUl.insertBefore(commentLi, commentsMainUl.lastChild);
    }
    else {
        commentsMainUl.appendChild(commentLi);
    }
}

// Dynamically add comments for posts
function GenerateCommentsForPost(postId, username) {

    var commentsRow = document.getElementById("commentsRow-" + postId);

    // This part is for new comment
    var commentsMainUl = document.createElement("ul");
    commentsMainUl.className = "list-group col-12";
    commentsMainUl.id = "commentsMainUl-" + postId;
    commentsMainUl.style.marginLeft = "8px";

    // Getting comments from db
    var returnedComments = GetCommentsForPost(postId);

    if (returnedComments.length !== 0) {
        for (var i = 0; i < returnedComments.length; i++) {
            CreateCommentBody(postId, returnedComments[i], username, commentsMainUl, false);
        }
    }

    // New comment part
    var newCommentLi = document.createElement("li");
    newCommentLi.className = "list-group-item";
    newCommentLi.style.width = "100%";

    // Adding input
    var inputNewComment = document.createElement("input");
    inputNewComment.type = "textarea";
    inputNewComment.placeholder = "Comment something...";
    inputNewComment.id = "newComment-" + postId;
    inputNewComment.className = "newComments";

    newCommentLi.appendChild(inputNewComment);

    // Adding button post
    var buttonForComment = document.createElement("button");
    buttonForComment.type = "button";
    buttonForComment.className = "btn btn-outline-danger btn-sm";
    buttonForComment.innerHTML = "Post";
    buttonForComment.style.marginLeft = "10px";

    buttonForComment.onclick = function () {
        UploadComment(postId, username);
    };

    newCommentLi.appendChild(buttonForComment);
    

    // Appending to comments row
    commentsMainUl.appendChild(newCommentLi);
    commentsRow.appendChild(commentsMainUl);
}

// Update comment
function UpdateComment(commentid, text) {
    var newComment = {
        commentid: commentid,
        text: text
    };

    $.ajax({
        type: "PUT",
        url: 'http://localhost:60321/api/updatecomment',
        dataType: 'json',
        data: newComment,
        success: function (data) {
            if (data === "OK") {
                var textInput = document.getElementById("inputComment-" + commentid);
                var editButton = document.getElementById("editButtonComment-" + commentid);

                textInput.disabled = "false";
                textInput.style.border = "none";

                editButton.style.visibility = "hidden";
            }
            else {
                alert(data);
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Delete comment
function DeleteComment(commentId, postId) {
    $.ajax({
        type: "DELETE",
        url: 'http://localhost:60321/api/delete-comment/' + commentId,
        success: function (data) {
            if (data === "OK") {
                var commentUl = document.getElementById("commentsMainUl-" + postId);

                var children = [...commentUl.childNodes];
                children.forEach(function (child) {
                    if (child.id === "comment-" + commentId) {
                        commentUl.removeChild(child);
                    }
                });
                alert("Comment deleted.");
            }
            else {
                alert(data);
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Delete post
function DeletePost(postId) {
    
    $.ajax({
        type: "DELETE",
        url: 'http://localhost:60321/api/delete-post/' + postId,
        success: function (data) {
            if (data === "OK") {
                var postContainer = document.getElementById("postContainter");
                var children = [...postContainer.childNodes];
                children.forEach(function (child) {
                    if (child.id === "post-" + postId) {
                        postContainer.removeChild(child);
                    }
                });
                alert("Post deleted.");

                var numOfPosts = document.getElementById("numOfPostsP");
                var num = parseInt(numOfPosts.innerHTML);
                numOfPosts.innerHTML = --num;
            }
            else {
                alert(data);
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Get users hashtags
function GetUserHashtags(username){

    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-user-hashtags/' + username,
        success: function (data) {
            if (data !== null && data.length != 0) {
                for (var i = 0; i < data.length; i++) {
                    AddUserHastag(data[i], "sideUserHashtagsUl");
                }
            }
            else {
                var mainHashtagUl = document.getElementById("hashtagText");
                mainHashtagUl.innerHTML = "No hashtags for you :(";
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Get all hashtags
function GetHashtags() {
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-hashtags',
        success: function (data) {
            if (data !== null && data.length != 0) {
                for (var i = 0; i < data.length; i++) {
                    AddUserHastag(data[i], "hashtagsUl");
                }
            }
            else {
                var allHashtagText = document.getElementById("allHashtagText");
                allHashtagText.innerHTML = "No hashtags :(";
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Add hashtags to user hashtags list
function AddUserHastag(hashtag, containerUl) {
    var mainHashtagUl = document.getElementById(containerUl);

    var hashtagLink = document.createElement("a");
    hashtagLink.href = "../hashtag/" + hashtag.text.replace(/#/g, "");
    hashtagLink.className = "list-group-item list-group-item-action list-group-item-danger";
    hashtagLink.innerHTML = hashtag.text;

    mainHashtagUl.appendChild(hashtagLink);
}

// Array that will store all usernames
var usernames = [];

function InitializeSearchInput() {
    // Getting usernames async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-all-users',
        success: function (data) {
            if (data != null) {
                usernames = data;
            }
            else
                alert("Error while trying to search for user :(");
        },
        error: function () {
            alert('Ajax error :(');
        }
    });

    // Setting on key down event to input
    var input = document.getElementById("inputForUserSearch");

    input.onkeydown = function () {
        var entered = input.value;

        if (entered === "")
            return;

        var found = [];

        var datalist = document.getElementById("usersForSearch");
        datalist.innerHTML = "";

        usernames.forEach(function (username) {
            if (username.includes(entered) === true) {
                var element = document.createElement("option");
                element.value = username;

                datalist.appendChild(element);
            }
        });
    }

    var button = document.getElementById("searchButton");

    button.onclick = function () {

        if (input.value === "")
            return;

        window.location.href = "../Profile/" + input.value;
    };
}

function CountUnreadedMessages() {
    // Getting number of unreaded messages async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/count-unreaded-messages',
        success: function (data) {
            if (data != -1) {
                var gettingBadgeSpan = document.getElementById("unreadedMessagesBadge");
                if (data != 0) {
                    gettingBadgeSpan.innerHTML = data;
                }
            }
            else
                alert("Error while trying to search for user :(");
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

////////////////////////////////////////////////////////////
///////////////PROFILE PART////////////////////////////////
///////////////////////////////////////////////////////////

// Get posts for user profile
function GetPostsForProfile() {
    var username = GetProfileUsername();
    var loggedInUsername = GetLoggedUsername();

    var data = { 'username': username, 'skip': 0 };

    // Getting user posts async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-user-posts',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    GeneratePostBody(data[i], true, false, loggedInUsername);

                }
            }
            else
                alert("Error while trying to get user posts :(");
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Load more users posts
function LoadMoreUserPosts() {
    var user = GetLoggedUsername();
    var profileUsername = GetProfileUsername();
    var loadMoreUserButton = document.getElementById("loadMoreUserButton");

    var data = { 'username': profileUsername, 'skip': numberOfShownPosts };
    // Getting posts async
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        url: 'http://localhost:60321/api/get-user-posts',
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    GeneratePostBody(data[i], true, false, user);
                }
                if (data.length !== 10) {
                    loadMoreUserButton.style.visibility = "hidden";
                }
                else {
                    loadMoreUserButton.style.visibility = "visible";
                }
                numberOfShownPosts += 10;
            }
            else {
                alert("Error while trying to load more user posts :(");
            }
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Get latest activities
function GetLatestActivities() {
    // Getting user activities async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-user-activities/' + GetLoggedUsername(),
        success: function (data) {
            if (data != null) {
                if (data.length != 0) {
                    for (var i = 0; i < data.length; i++) {
                        AddLatestActivity(data[i]);
                    }
                }
                else {
                    var textActivity = document.getElementById("activitiesText");

                    textActivity.innerHTML = "No recent activities...";
                }
            }
            else
                alert("Error while trying to get user posts :(");
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

// Add activity to activities list
function AddLatestActivity(data) {
    var mainActivityUl = document.getElementById("sideLatestActivities");

    var activity = document.createElement("li");
    activity.className = "list-group-item list-group-item-info";
    activity.innerHTML = data;

    mainActivityUl.appendChild(activity);
}

// Following another user
function FollowUser() {
    var toFollow = GetProfileUsername();

    $.ajax({
        type: "POST",
        url: '/api/follow-user',
        data: { 'userToFollow': toFollow },
        success: function (data) {
            if (data === "OK") {
                var followButton = document.getElementById("followButton");
                followButton.innerHTML = "Unfollow";
                followButton.onclick = function () {
                    UnfollowUser();
                };

                var numOfFollowers = document.getElementById("numOfFollowersProfile");
                var num = parseInt(numOfFollowers.innerHTML);
                numOfFollowers.innerHTML = ++num;
            }
            else {
                alert("Error while uploading comment :(");
            }
        }
    });
}

// Unfollowing another user
function UnfollowUser() {
    var toUnfollow = GetProfileUsername();

    $.ajax({
        type: "POST",
        url: '/api/unfollow-user',
        data: { 'userToUnfollow': toUnfollow },
        success: function (data) {
            if (data === "OK") {
                var followButton = document.getElementById("followButton");
                followButton.innerHTML = "Follow";
                followButton.onclick = function () {
                    FollowUser();
                };

                var numOfFollowers = document.getElementById("numOfFollowersProfile");
                var num = parseInt(numOfFollowers.innerHTML);
                numOfFollowers.innerHTML = --num;
            }
            else {
                alert("Error while uploading comment :(");
            }
        }
    });
}

// Uploading profile picture
function UploadProfilePicture() {
    var formData = new FormData();

    var uploadedPicture = document.getElementById("profilePictureChange");

    if (uploadedPicture.files[0] === null || uploadedPicture.files[0] === undefined)
        return;

    formData.append("uploadedPicture", uploadedPicture.files[0]);

    $.ajax({
        type: "POST",
        url: '/api/upload-profile-picture',
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data === "OK") {
                window.location.href = "../Profile/" + GetLoggedUsername();
            }
            else {
                alert("Error while trying to upload  :(");
            }
        }
    });
}

////////////////////////////////////////////////////////////
///////////////HASHTAG PART////////////////////////////////
///////////////////////////////////////////////////////////

// Getting posts with hashtag in its content
function GetPostsWithHashtag(hashtag) {
    // Getting posts async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-posts-with-hashtag/' + hashtag,
        success: function (data) {
            if (data != null) {
                if (data.length != 0) {
                    for (var i = 0; i < data.length; i++) {
                        GeneratePostBody(data[i], GetIfLogged(), false, GetLoggedUsername());
                    }
                }
                else {
                    var textActivity = document.getElementById("postContainter");

                    textActivity.innerHTML = "No posts with that hashtag...";
                    textActivity.style.color = "#dc3545";
                }
            }
            else
                alert("Error while trying to get posts :(");
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

////////////////////////////////////////////////////////////
///////////////TOPRATED PART////////////////////////////////
///////////////////////////////////////////////////////////

// Getting top rated posts
function GetTopRatedPosts() {
    // Getting posts async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-top-rated-posts',
        success: function (data) {
            if (data != null) {
                if (data.length != 0) {
                    for (var i = 0; i < data.length; i++) {
                        GeneratePostBody(data[i], GetIfLogged(), false, GetLoggedUsername());
                    }
                }
                else {
                    var textActivity = document.getElementById("postContainter");

                    textActivity.innerHTML = "No posts to order...";
                    textActivity.style.color = "#dc3545";
                }
            }
            else
                alert("Error while trying to get posts :(");
        },
        error: function () {
            alert('Ajax error :(');
        }
    });
}

////////////////////////////////////////////////////////////
///////////////MESSAGES PART////////////////////////////////
///////////////////////////////////////////////////////////

// Send Message
function SendMessage() {
    var messageInput = document.getElementById("newMessage");

    if (messageInput.value !== "") {
        $.ajax({
            type: "POST",
            url: '/api/send-message',
            data: { 'reciever': GetReciever(), 'text': messageInput.value },
            success: function (data) {
                if (data != null) {
                    GenerateMessageBody(data.text, data.timesent);
                    messageInput.value = "";

                    // Scroling till end of message div
                    var objDiv = document.getElementById("messageContainer");
                    objDiv.scrollTop = objDiv.scrollHeight;
                }
                else {
                    alert("Error while uploading comment :(");
                }
            }
        });
    }
    else
        return;
}

// Generate new message body
function GenerateMessageBody(text, time) {
    var messageContainer = document.getElementById("messageContainer");

    var picture = GetLoggedUserPicture();

    var mainDiv = document.createElement("div");
    mainDiv.className = "message self";

    var imageDiv = document.createElement("div");
    imageDiv.className = "senderImage";

    var image = document.createElement("img");
    image.src = picture;
    image.width = "40";
    image.height = "40";
    image.className = "rounded-circle";

    imageDiv.appendChild(image);
    mainDiv.appendChild(imageDiv);

    var pForText = document.createElement("p");
    pForText.className = "message-text";
    pForText.innerHTML = text;

    mainDiv.appendChild(pForText);

    var pForTime = document.createElement("p");
    pForTime.className = "timesent";
    pForTime.innerHTML = time;

    mainDiv.appendChild(pForTime);

    messageContainer.appendChild(mainDiv);
}

// jQuery part
$(document).ready(function () {

    if (GetIfLogged() === true) {
        InitializeSearchInput();
        CountUnreadedMessages();
    }

    // HOME PART
    $("#uploadButton").click(UploadPost);
    $("#loadMoreButton").click(LoadMorePosts);


    // PROFILE PART
    $("#loadMoreUserButton").click(LoadMoreUserPosts);
    $("#profilePictureChange").change(UploadProfilePicture);
});