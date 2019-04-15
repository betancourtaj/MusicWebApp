// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var dataString;


$("#searchBar").keyup(function() {
    dataString = this.value;
    $("#content").html(dataString);

    $.ajax({
        type: "GET",
        url: "/SearchSong",
        data: {data: dataString},
        success: function(data)
        {
            //alert(dataString);
        },
        datatype: "json"
    });
});
