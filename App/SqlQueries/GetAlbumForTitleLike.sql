select title from p_album where title like concat(concat('%', :dataString), '%')