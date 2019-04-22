// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var searchString;
var albumsArray;
var songsArray;
var bioCharacterCount;
var commentCharacterCount;

var playlists;

$(function () {
    
})



/*$("#searchBar").keyup(function() {
    searchString = this.value;
    $("#content").html(searchString);
    getUpdatedAlbumArray();
    getUpdatedSongArray();

    $("#result").html(albumsArray[0]);

    // TODO: Changing this to POST might work tbh lol, or not.
    $.ajax({
        type: "GET",
        url: "/SearchSong",
        data: {data: searchString},
        success: function(result)
        {
            //alert(dataString);
        },
        datatype: "json"
    });
});


// TODO: Figure out these for the search functionality.
function getUpdatedAlbumArray() {
    alert("Albums called");
    $.ajax({
        type: "POST",
        url: "/SearchSong/GetAlbumArray",
        success: function(result)
        {
            alert("POSTED ALBUM");
            albumsArray = result;
        },
        datatype: "json"
    });
}


function getUpdatedSongArray() {
    alert("songs called");
    $.ajax({
        type: "POST",
        url: "/SearchSong/GetAlbumArray",
        success: function(result)
        {
            alert("POSTED SONG");
            songsArray = result;
        },
        datatype: "json"
    });
}*/

$("#user-bio-text").keyup(function() {
    bioCharacterCount = this.value.length;
    $("#character-count").html(bioCharacterCount);

    if(bioCharacterCount > 100 || bioCharacterCount === 0)
    {
        $('#bio-submit').attr('disabled','disabled');
    }
    else
    {
        $('#bio-submit').removeAttr('disabled');

    }
});

$("#commentText").keyup(function() {
    commentCharacterCount = this.value.length;
    $("#character-count").html(commentCharacterCount);

    if(commentCharacterCount > 240 || commentCharacterCount === 0)
    {
        $('#comment-submit').attr('disabled','disabled');
    }
    else
    {
        $('#comment-submit').removeAttr('disabled');
    }
});

// The CSHTML file is loaded as soon as you load the SearchSong page (Initially). As soon as you interact with the page,
// (click or type in the search bar) the Server code updates by filling the arrays with the queried data, but the CSHTML file
// does not get updated. Meaning the for loop ran ( with a null array since it was the first time the page loaded) but it doesn't run
// anymore after that because the CSHTML file doesn't live update.