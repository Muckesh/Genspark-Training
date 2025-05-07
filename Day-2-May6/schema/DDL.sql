-- Table categories
CREATE TABLE categories(
	id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(30) NOT NULL UNIQUE
);

-- Table countries
CREATE TABLE country(
	id INT PRIMARY KEY IDENTITY(1,1),
	cname VARCHAR(20) NOT NULL UNIQUE
);

-- Table states associated with countries
CREATE TABLE state(
	id INT PRIMARY KEY IDENTITY(1,1),
	sname VARCHAR(30) NOT NULL,
	c_id INT,
	FOREIGN KEY (c_id) REFERENCES country(id)
);

-- Table cities associated with states
CREATE TABLE city(
	id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(30) NOT NULL,
	s_id INT,
	FOREIGN KEY (s_id) REFERENCES state(id)
);

-- Table areas associated with cities (using zipcodes)
CREATE TABLE area(
	zipcode INT PRIMARY KEY,
	name VARCHAR(30) NOT NULL,
	city_id INT,
	FOREIGN KEY (city_id) REFERENCES city(id)
);

-- Table for full address details linked to ZIP codes
CREATE TABLE address(
	add_id INT PRIMARY KEY IDENTITY(1,1),
	doorNo VARCHAR(8) NOT NULL,
	addressLine VARCHAR(50) NOT NULL,
	zipcode INT,
	FOREIGN KEY (zipcode) REFERENCES area(zipcode)
);

-- Table for supplier details
CREATE TABLE supplier(
	id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(20),
	point_of_contact VARCHAR(25),
	phone_number INT NOT NULL,
	email VARCHAR(50) NOT NULL,
	address_id INT,
	FOREIGN KEY (address_id) REFERENCES address(add_id)
);

-- Table for product details
CREATE TABLE products(
	prod_id INT PRIMARY KEY IDENTITY(1,1),
	name VARCHAR(50) NOT NULL,
	price DECIMAL(10, 2) NOT NULL,
	quantity INT,
	description TEXT,
	image TEXT
);

-- Table for recording supply transactions of products from suppliers
CREATE TABLE product_supplier(
	transaction_id INT PRIMARY KEY,
	product_id INT NOT NULL,
	supplier_id INT NOT NULL,
	supplyDate DATE,
	qty INT,
	FOREIGN KEY (product_id) REFERENCES products(prod_id),
	FOREIGN KEY (supplier_id) REFERENCES supplier(id)
);

-- Table for store customer details
CREATE TABLE customer(
	cus_id INT PRIMARY KEY IDENTITY(1,1),
	Fname VARCHAR(30) NOT NULL,
	Lname VARCHAR(30) NOT NULL,
	phone_number INT NOT NULL,
	address_id INT,
	FOREIGN KEY (address_id) REFERENCES address(add_id)
);

-- Table for  customer orders
CREATE TABLE orders (
    order_number INT PRIMARY KEY,
    customer_id INT NOT NULL,
    date_of_order DATE NOT NULL,
    amount DECIMAL(10, 2),
    order_status VARCHAR(50),
	FOREIGN KEY (customer_id) REFERENCES customer(cus_id)
);

-- Table for storing details of products in each order
CREATE TABLE order_details (
    id INT PRIMARY KEY IDENTITY(1, 1),
    order_number INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (order_number) REFERENCES orders(order_number),
	FOREIGN KEY (product_id) REFERENCES products(prod_id)
);