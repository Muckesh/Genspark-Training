-- stored procedures adhoc queries
-- data as object
-- prevent injection
-- encrypt
-- need not know underlying table
-- encapsulated

--jumps compilation and executaion plan -> improve performance

create procedure proc_FirstProcedure
as
begin
	print 'HELLO WORLD'
end
GO
exec proc_FirstProcedure

create table Products
(id int identity(1,1) constraint pk_productId primary key,
name nvarchar(100) not null,
details nvarchar(max));

go

create proc proc_InsertProduct(@pname nvarchar(100), @pdetails nvarchar(max))
as
begin
	insert into Products (name,details) values (@pname,@pdetails)
end

go

proc_InsertProduct 'Laptop', '{"brand":"Dell","spec":{"ram":"16GB","processor":"i5"}}'

select * from Products;

-- AdHoc query
select name, JSON_QUERY(details, '$.spec' ) Product_Spec from Products;

create proc proc_UpdateProduct_Spec(@pid int, @newValue varchar(20))
as
begin
	update Products set details = JSON_MODIFY(details, '$.spec.ram',@newValue) where id = @pid;
end

proc_UpdateProduct_Spec 1, '24GB'

select id, name, JSON_VALUE(details, '$.brand') BrandName
from Products;

-- bulk insert
declare @jsondata nvarchar(max) = '

[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]
';

exec proc_BulkInsertPosts @jsondata

insert into Posts(user_id,id,title,body)
	  select userId,id,title,body from openjson(@jsondata)
	  with (userId int,id int, title varchar(100), body varchar(max))


  create table Posts
  (id int primary key,
  title nvarchar(100),
  user_id int,
  body nvarchar(max))
Go

  select * from Posts

 create proc proc_BulkInsertPosts(@jsondata nvarchar(max))
 as
 begin
	insert into Posts(user_id, id, title,body)
	select userId,id,title,body from openjson(@jsondata)
	with (userId int, id int, title varchar(100), body varchar(max))
 end

 

  delete from Posts

  proc_BulkInsertPosts '
[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  },
  {
    "userId": 2,
    "id": 12,
    "title": "in quibusdam tempore odit est dolorem",
    "body": "itaque id aut magnam\npraesentium quia et ea odit et ea voluptas et\nsapiente quia nihil amet occaecati quia id voluptatem\nincidunt ea est distinctio odio"
  },
  {
    "userId": 2,
    "id": 13,
    "title": "dolorum ut in voluptas mollitia et saepe quo animi",
    "body": "aut dicta possimus sint mollitia voluptas commodi quo doloremque\niste corrupti reiciendis voluptatem eius rerum\nsit cumque quod eligendi laborum minima\nperferendis recusandae assumenda consectetur porro architecto ipsum ipsam"
  },
  {
    "userId": 2,
    "id": 14,
    "title": "voluptatem eligendi optio",
    "body": "fuga et accusamus dolorum perferendis illo voluptas\nnon doloremque neque facere\nad qui dolorum molestiae beatae\nsed aut voluptas totam sit illum"
  },
  {
    "userId": 3,
    "id": 27,
    "title": "quasi id et eos tenetur aut quo autem",
    "body": "eum sed dolores ipsam sint possimus debitis occaecati\ndebitis qui qui et\nut placeat enim earum aut odit facilis\nconsequatur suscipit necessitatibus rerum sed inventore temporibus consequatur"
  },
  {
    "userId": 3,
    "id": 28,
    "title": "delectus ullam et corporis nulla voluptas sequi",
    "body": "non et quaerat ex quae ad maiores\nmaiores recusandae totam aut blanditiis mollitia quas illo\nut voluptatibus voluptatem\nsimilique nostrum eum"
  },
  {
    "userId": 3,
    "id": 29,
    "title": "iusto eius quod necessitatibus culpa ea",
    "body": "odit magnam ut saepe sed non qui\ntempora atque nihil\naccusamus illum doloribus illo dolor\neligendi repudiandae odit magni similique sed cum maiores"
  }
  ]'

  select * from Products;

  select * from Products where 
  TRY_CAST(JSON_VALUE(details,'$.spec.processor') as nvarchar(20)) = 'i5';

  -- create a procedure that brings post by taking the user_id as parameter

  create proc proc_Get_Post_By_Id(@uid int)
  as
  begin
	select * from Posts where user_id = @uid;
  end

  proc_Get_Post_By_Id '2'