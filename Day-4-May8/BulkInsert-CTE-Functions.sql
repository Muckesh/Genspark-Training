use pubs;
select * from products;

  select * from products where 
  try_cast(json_value(details,'$.spec.processor') as nvarchar(20)) ='i5'

  create or alter proc proc_FileterProducts(@pprocessor varchar(20),@pcount int out)
  as
  begin
	set @pcount = (select count(*) from Products where TRY_CAST(JSON_VALUE(details,'$.spec.processor') as nvarchar(20)) = @pprocessor)
  end

  begin
  declare @count int;
  exec proc_FileterProducts 'i5', @count out
  print concat('The number of computer is : ',@count)
  end

sp_help authors 

create table people
(id int primary key,
name nvarchar(20),
age int)

create proc proc_BulkInsert(@filepath nvarchar(500))
as
begin
	declare @insertQuery nvarchar(max)
	set @insertQuery = 'BULK INSERT people from ''' + @filepath + '''
	with(
	FIRSTROW = 2,
	FIELDTERMINATOR = '','',
	ROWTERMINATOR = ''\n''
	)'
	exec sp_executesql @insertQuery
end

proc_BulkInsert 'C:\Muckesh\Presidio\Genspark-Training\Day-4-May8\Data.csv'

select * from people;

create table BulkInsertLog
(LogId int identity(1,1) primary key,
FilePath nvarchar(1000),
status nvarchar(50) constraint chk_status Check(status in ('Success','Failed')),
message nvarchar(max),
InsertedOn DateTime default GetDate()
)

drop table BulkInsertLog

create or alter proc proc_BulkInsert(@filepath nvarchar(500))
as
begin
  Begin try
	   declare @insertQuery nvarchar(max)

	   set @insertQuery = 'BULK INSERT people from '''+ @filepath +'''
	   with(
	   FIRSTROW =2,
	   FIELDTERMINATOR='','',
	   ROWTERMINATOR = ''\n'')'

	   exec sp_executesql @insertQuery

	   insert into BulkInsertLog(filepath,status,message)
	   values(@filepath,'Success','Bulk insert completed')
  end try
  begin catch
		 insert into BulkInsertLog(filepath,status,message)
		 values(@filepath,'Failed',Error_Message())
  END Catch
end

exec proc_BulkInsert 'C:\Muckesh\Presidio\Genspark-Training\Day-4-May8\Data.csv'

truncate table people
select * from people
select * from BulkInsertLog

with cteAuthors
as 
(select au_id, CONCAT(au_fname,' ',au_lname) author_name from authors)

select * from cteAuthors;


declare @page int = 1, @pageSize int = 10;
with PaginatedBooks 
as
 (select title_id, title, price, ROW_NUMBER() over (order by price desc) as RowNumber
	from titles)
select * from PaginatedBooks where RowNumber between ((@page-1)*@pageSize) and (@page*@pageSize);

-- create a sp that will take the page number and size as param and print the books
create or alter proc proc_PaginateTitles( @page int =1, @pageSize int=10)
as
begin
with PaginatedBooks as
( select  title_id,title, price, ROW_Number() over (order by price desc) as RowNum
  from titles
)
select * from PaginatedBooks where rowNUm between((@page-1)*(@pageSize+1)) and (@page*@pageSize)
end

exec proc_PaginateTitles 1,15

select title_id, title, price
from titles
order by price desc
offset 1 rows fetch next 10 rows only

-- functions must return a value but stored procedures need not
create function fn_CalculateTax(@baseprice float, @tax float)
returns float
as 
begin
	return (@baseprice + (@baseprice*@tax/100))
end

select dbo.fn_CalculateTax(1000,10)

select title, dbo.fn_CalculateTax(price,12) as Price_With_Tax from titles;

create function fn_tableSample(@minprice float)
  returns table
  as

    return select title,price from titles where price>= @minprice


drop function fn_tableSample

	select * from dbo.fn_tableSample(10)

--older and slower but supports more logic
create function fn_tableSampleOld(@minprice float)
  returns @Result table(Book_Name nvarchar(100), price float)
  as
  begin
    insert into @Result select title,price from titles where price>= @minprice
    return 
end
select * from dbo.fn_tableSampleOld(10)
