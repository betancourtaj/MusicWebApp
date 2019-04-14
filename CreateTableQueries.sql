create table p_user 
(userid number(8) not null,
bio varchar(50),
email varchar(20) not null,
username varchar(20) not null,
passwd varchar(30) not null,
primary key(userid));

create table p_album
(albumid number(8) not null,
title varchar(20) not null,
releasedate varchar(10) not null,
primary key (albumid));

create table p_artist
(artistid number(8) not null,
primary key (artistid),
foreign key (artistid) references p_user);

create table p_song
(songid number(8) not null,
title varchar(20) not null,
releasedate varchar(20) not null,
albumid number(8) not null,
primary key (songid),
foreign key (albumid) references p_album);

create table p_playlist
(playlistid number(8) not null,
title varchar(20) not null,
userid number(8) not null,
primary key (playlistid),
foreign key (userid) references p_user);

create table p_comment
(commentid number(8) not null,
text varchar(240) not null,
playlistid number(8) not null,
userid number(8) not null,
primary key (commentid),
foreign key (playlistid) references p_playlist,
foreign key (userid) references p_user);

create table p_artist_song
(artistid number(8) not null,
songid number(8) not null,
primary key (artistid, songid),
foreign key (artistid) references p_artist,
foreign key (songid) references p_song);

create table p_playlist_song
(playlistid number(8) not null,
songid number(8) not null,
primary key (playlistid, songid),
foreign key (playlistid) references p_playlist,
foreign key (songid) references p_song);

create table p_artist_album
(artistid number(8) not null,
albumid number(8) not null,
primary key (artistid, albumid),
foreign key (artistid) references p_artist,
foreign key (albumid) references p_album);