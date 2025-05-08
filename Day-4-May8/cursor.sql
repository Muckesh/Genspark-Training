use pubs;
select * from titles;

--cursor
declare @BookName varchar(200);

declare book_cursor cursor for 
select title from titles;

open book_cursor;

fetch next from book_cursor into @BookName;
print @BookName;

while @@FETCH_STATUS = 0
begin
	print @BookName;
	fetch next from book_cursor into @BookName;
end;

close book_cursor;
deallocate book_cursor;

