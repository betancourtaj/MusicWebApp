begin
insert into p_album (albumid, title, releasedate) values (:albumID, :albumtitle, :releaseDate);
insert into p_artist_album (artistid, albumid) values (:artistID, :albumID);
end;