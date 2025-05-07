use pubs
go

select * from publishers

select * from titles

select title,pub_id from titles

select title, pub_name 
from titles join publishers
on titles.pub_id = publishers.pub_id

--print the publisher deatils of the publisher who has never published
select * from publishers where pub_id not in
(select distinct pub_id from titles)

select title, pub_name 
from titles right outer join publishers
on titles.pub_id = publishers.pub_id

--Select the author_id for all the books. Print the author_id and the book name

select * from titles;

select * from titleauthor;

select * from authors;

select au_id, title from titleauthor join titles on titleauthor.title_id = titles.title_id;

select * from authors;

select CONCAT(au_fname,' ',au_lname) Author_Name, title_id from authors a join titleauthor ta on a.au_id = ta.au_id order by 2;

-- select CONCAT(au_fname,' ',au_lname) Author_Name, title_id, title from authors a join titleauthor ta on a.au_id = ta.au_id join titles t on ta.title_id = t.title_id;
-- to access book name again joinning the joined table. Now we can't select the title_id -> ambiguous column name

select concat(au_fname,au_lname) Author_Name, title from authors a join titleauthor ta on a.au_id = ta.au_id join titles t on ta.title_id = t.title_id order by t.title_id;

-- print  pub name, book name and the order date of the books
select * from titles;
select * from publishers;
select * from sales;

select pub_name, title, ord_date from publishers p join titles t on p.pub_id = t.pub_id join sales s on t.title_id = s.title_id;

-- print pub name and the first book sale date for all the publishers
select pub_name, ord_date from publishers p join titles t on p.pub_id = t.pub_id join sales s on t.title_id = s.title_id;

select pub_name Publisher_Name, min(ord_date) First_Order_Date from publishers p join titles t on p.pub_id = t.pub_id join sales s on t.title_id = s.title_id group by pub_name;

select pub_name Publisher_Name, min(ord_date) First_Order_Date
from publishers p left outer join titles t on p.pub_id = t.pub_id
left outer join sales s on t.title_id = s.title_id 
group by pub_name order by 2 desc;

-- Print the book name and the store addres of the sale

select * from titles;

select * from sales;

select * from stores;

select title BookName, stor_name StoreName, concat(stor_address,'',city,' ',state,' ',zip) StoreAddress from titles t
join sales s on t.title_id = s.title_id join stores st on s.stor_id = st.stor_id order by 2;