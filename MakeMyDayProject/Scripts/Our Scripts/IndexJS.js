// JS part
function GetPosts(logedin) {

    // Getting posts async
    $.ajax({
        type: "GET",
        url: 'http://localhost:60321/api/get-all-posts',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (logedin == true) {

            }
            else {

            }
        },
        error: function () {
            alert('A error');
        }
    });
}


function GeneratePostBody(post) {

}


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
        //contentType: 'application/json; charset=utf-8',
        //dataType: 'json',
        //data: JSON.stringify(post),
        success: function (data) {
            console.log(data);
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



















// jQuery part
$(document).ready(function () {

    $("#uploadButton").click(UploadPost);
});