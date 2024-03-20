CREATE DATABASE BazaarBooksDB; 
use BazaarBooksDB;

CREATE TABLE Users( 
UUID varchar(225) primary key not null,
Email varchar(225) not null,
FirstName varchar(225) not null, 
LastName varchar(225) not null, 
password varchar(255) not null ,
Level int default 1 not null); 

Alter table Users drop column password;
Create table book( 
ISBN varchar(225) primary key not null, 
Title varchar(225) not null, 
Description varchar(225) not null, 
Price float default 0 not null, 
Author varchar(225) not null, 
Genre varchar(225) not null,
AvailableQuantity int default 0 not null,
ImageUrl varchar(225) 
); 

Create table Orders( 
OrderID int NOT NULL identity(1,1) primary key, 
UUID varchar(225) not null, 
PurchaseDate date, 
Total float default 0 not null, 
FOREIGN KEY (UUID) REFERENCES Users(UUID) ); 

Alter table Orders drop column PurchaseDate;
Alter table Orders add PurchaseDate DateTime not null;
Create table ShoppingCart( 
CartItemID int not null identity(1,1) primary key, 
UUID varchar(225) not null,
OrderID int, 
ISBN varchar(225), 
Quantity int, 
isPurchased bit default 0 not null,

FOREIGN KEY (OrderID) REFERENCES Orders(OrderID), 
FOREIGN KEY (UUID) REFERENCES Users(UUID),
FOREIGN KEY (ISBN) REFERENCES book(ISBN) ); 

Select * from Orders;
Select * from ShoppingCart;
Select * from Users;
Select * from book;
delete from Orders;

Insert into book (ISBN, Title, Description, Price, Author, Genre, AvailableQuantity, ImageUrl) values
('A1B2C3D4E5', 'The Great Gatsby', 'Follow the enigmatic Jay Gatsby through the eyes of Nick Carraway as he navigates the lavish world of 1920s New York. A tale of love, ambition, and tragedy.', 149.00, 'F. Scott Fitzgerald', 'Classic', 150,'https://exclusivebooks.co.za/cdn/shop/products/9781840227956_5506d11e-3c17-4166-9159-34668eab1be3.jpg?v=1707634992' ),
('F6G7H8I9J10', '1984', 'Dive into George Orwell’s dystopian world of surveillance, propaganda, and thought control. Winston Smith rebels against the oppressive Party in a fight for truth.', 129.00, 'George Orwell', 'Dystopian', 20, 'https://exclusivebooks.co.za/cdn/shop/products/9789357943383.jpg?v=1707789114'),
('K11L12M13N14', 'Pride and Prejudice', 'Experience Jane Austen’s timeless tale of love, manners, and societal expectations in 19th century England. Will Elizabeth Bennet and Mr. Darcy overcome their pride?', 199.99, 'Jane Austen', 'Classic', 213, 'https://exclusivebooks.co.za/cdn/shop/products/9781787556744_53e68916-0a1f-4b33-9d91-a38c0b20ef1b.jpg?v=1707625328'),
('O15P16Q17R18', 'The Alchemist', 'Join Santiago on his quest for treasure and self-discovery. Paulo Coelho weaves a mystical tale of following dreams, finding purpose, and embracing the journey.', 160.50, 'Paulo Coelho', 'Fiction', 54, 'https://exclusivebooks.co.za/cdn/shop/products/9780722532935_76f47ed5-6c4b-49e9-82a6-e6a35b941345.jpg?v=1707314747');
update Users set Level= 0 where Email= 'tanyagovender2003@gmail.com';

