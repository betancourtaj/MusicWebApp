// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var dataString;
var albumsArray;
var songsArray;


$("#searchBar").keyup(function() {
    dataString = this.value;
    $("#content").html(dataString);
    getUpdatedAlbumArray();
    getUpdatedSongArray();

    $("#result").html(albumsArray[0]);

    $.ajax({
        type: "GET",
        url: "/SearchSong",
        data: {data: dataString},
        success: function(result)
        {
            //alert(dataString);
        },
        datatype: "json"
    });
});

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
}

// The CSHTML file is loaded as soon as you load the SearchSong page (Initially). As soon as you interact with the page,
// (click or type in the search bar) the Server code updates by filling the arrays with the queried data, but the CSHTML file
// does not get updated. Meaning the for loop ran ( with a null array since it was the first time the page loaded) but it doesn't run
// anymore after that because the CSHTML file doesn't live update.