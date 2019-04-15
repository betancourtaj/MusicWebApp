begin
insert into p_song (songid, title, releasedate, albumid) values (:songID, :songTitle, :releaseDate, :albumID);
insert into p_artist_song (artistid, songid) values (:artistID, :songID);
end;