begin
delete from p_playlist_song where playlistid = :id;
delete from p_playlist where playlistid = :id;
end;