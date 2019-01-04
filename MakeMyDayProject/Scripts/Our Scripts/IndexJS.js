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
    var loggedin = IsLoggedin();
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
    picture.src = post.creator.profilepictureurl.replace(/~/g, "");
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
    linkToUser.href = '../Manage';          // NEED TO POINTS TO USER PROFILE TODO!!!!!!!!!
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
    content.innerHTML = ReplaceHashtagsAndTags(post.content); // <a> NEED TO REDIRECT TO HASHTAGS PAGE OR TO USER PROFILE TODO!!!!!!!!!!

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
    //likeButton.checked = "";          // NEED TO ADD CHECK IF USER IS LIKED OR DISLIKED POST

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
    //dislikeButton.checked = "";          // NEED TO ADD CHECK IF USER IS LIKED OR DISLIKED POST

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

    var uploadedPicture = $('input[type=file]')[0].files[0];

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
            tempContent = tempContent.replace(hashtagsAndTags.Hashtags[i], " <a href='..'>" + hashtagsAndTags.Hashtags[i] + "</a> ");
        }
    }

    // Replacing tags with link to user profile that is tagged
    if (hashtagsAndTags.Tags.length != 0) {
        for (var i = 0; i < hashtagsAndTags.Tags.length; i++) {
            tempContent = tempContent.replace(hashtagsAndTags.Tags[i], " <a href='..'>" + hashtagsAndTags.Tags[i] + "</a> ");
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
    linkToUser.href = '../Manage';          // NEED TO POINTS TO USER PROFILE TODO!!!!!!!!!
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
function GetUserHashtags(username) {
    console.log(username);

    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-user-hashtags/' + username,
        success: function (data) {
            if (data !== null && data.length != 0) {
                for (var i = 0; i < data.length; i++) {
                    AddUserHastag(data[i]);
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

// Add hashtags to user hashtags list
function AddUserHastag(hashtag) {
    var mainHashtagUl = document.getElementById("sideUserHashtagsUl");

    var hashtagLink = document.createElement("a");
    hashtagLink.href = ".."; // ADD HREF TO ALL POSTS WITH THIS HASHTAG
    hashtagLink.className = "list-group-item list-group-item-action list-group-item-danger";
    hashtagLink.innerHTML = hashtag.text;

    mainHashtagUl.appendChild(hashtagLink);
}

// jQuery part
$(document).ready(function () {

    $("#uploadButton").click(UploadPost);
    $("#loadMoreButton").click(LoadMorePosts);
});