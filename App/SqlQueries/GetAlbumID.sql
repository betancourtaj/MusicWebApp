select al.albumid from p_album al, p_artist_album aa where aa.albumid = al.albumid and al.title = :title and aa.artistid = :artistid